using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class ReelController : MonoBehaviour
{
    /// <summary>
    /// リールを識別する定数
    /// </summary>
    public enum ReelPosition
    {
        E_REEL_POS_L = 0,
        E_REEL_POS_C,
        E_REEL_POS_R,
        E_REEL_POS_MAX,
    }

    /// <summary>
    /// 列を識別する定数
    /// </summary>
    public enum ReelRow
    {
        E_REEL_ROW_D = 0,
        E_REEL_ROW_C,
        E_REEL_ROW_U,
        E_REEL_ROW_MAX,
    }

    public GameObject[] Reel = new GameObject[(int)ReelPosition.E_REEL_POS_MAX];
    [SerializeField] private float RotValue = 0.0f;

    /// <summary>
    /// 滑りコマのデータ
    /// </summary>
    private List<string[]> ReelRollDeta;
    private static readonly string csvReelRollDetaStr = "Reel_L_Control";

    /// <summary>
    /// リール上のコマ数
    /// </summary>
    private const int LayerCount = 21;
    /// <summary>
    /// 1層あたりの角度範囲
    /// </summary>
    private const float LayerAngle = 360.0f / LayerCount;

    /// <summary>
    /// リールの子役の配列データ
    /// </summary>
    private List<string[]> ReelFlagData;
    private static readonly string csvReelFlagDataStr = "Reel_Flag_Position";

    /// <summary>
    /// 有効ライン [どのラインか, どのリールか]
    /// </summary>
    private static readonly List<bool[,]> Lines = new List<bool[,]>
    {
        new bool[3, 3]      //下段
        {
            //左     中     右
            { true , true , true  },        //下段
            { false, false, false },        //中段
            { false, false, false }         //上段
        },
        new bool[3, 3]      //中段
        {
            { false, false, false },
            { true , true , true  },
            { false, false, false }
        },
        new bool[3, 3]      //上段
        {
            { false, false, false },
            { false, false, false },
            { true , true , true  }
        },
        new bool[3, 3]      //左斜め
        {
            { false, false, true  },
            { false, true , false },
            { true , false, false }
        },
        new bool[3, 3]      //右斜め
        {
            { true , false, false },
            { false, true , false },
            { false, false, true  }
        }
    };

    /// <summary>
    /// 当選している子役があるポジションを格納
    /// </summary>
    private ReelRow[] FlagRow = new ReelRow[3];

    /// <summary>
    /// 現状有効な有効ラインの番号を格納
    /// </summary>
    private List<int> LinesNum = new List<int>();

    private FlagLottery.FlagType[,] StopResult = new FlagLottery.FlagType[3, 3];

    private void Start()
    {
        ReelRollDeta = ReadCSVFile.ReadCSV(csvReelRollDetaStr, true, false);
        ReelFlagData = ReadCSVFile.ReadCSV(csvReelFlagDataStr, false, false);
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                StopResult[row, col] = FlagLottery.FlagType.E_FLAG_TYPE_MAX;
            }
        }
    }

    public void StartReel()
    {
        for(int i = 0; i < Reel.Length; i++)
        {
            Reel[i].transform.GetComponent<ReelRotation>().rotFlag = true;
        }
    }

    public void StopReel(ReelPosition ReelPos)
    {
        Reel[(int)ReelPos].transform.GetComponent<ReelRotation>().rotFlag = false;
    }

    /// <summary>
    /// ボタン押した後のコマ滑り制御
    /// </summary>
    /// <param name="ReelPos"> どのリールか </param>
    /// <param name="flagType"> 成立している子役 </param>
    public void ControlReel(ReelPosition ReelPos, FlagLottery.FlagType flagType)
    {
        int Layer = 0;
        int PullIn = 0;
        int Kick = 0;
        switch(ReelPos)
        {
            case ReelPosition.E_REEL_POS_L:
                Layer = GetRotationLayer(ReelPos);
                PullIn = SlimeReel(Layer, flagType);
                break;
            case ReelPosition.E_REEL_POS_C:
            case ReelPosition.E_REEL_POS_R:
                Layer = GetRotationLayer(ReelPos);
                PullIn = PullInControl(ReelPos, Layer, flagType);
                break;
        }
        //Debug.Log("成立役：" + flagType + "(" + (int)flagType + ")" + "　停止コマ：" + Layer + " 滑りコマ数：" + PullIn);

        int l = Layer + PullIn + Kick;
        if (l >= 21)
        {
            l -= 21;
        }

        SetStopResult(ReelPos, l);

        if (ReelPos == ReelPosition.E_REEL_POS_R)
        {
            Kick = KickControl(flagType, l);
        }

        l += Kick;
        if (l >= 21)
        {
            l -= 21;
        }

        Reel[(int)ReelPos].GetComponent<ReelRotation>().ControlRotationReel(LayerAngle * (float)(l));

        if (flagType != FlagLottery.FlagType.E_FLAG_TYPE_MISS)
        {
            FlagRow[(int)ReelPos] = FindFlagPos(ReelPos, l, flagType);
            FindLines(ReelPos, FlagRow[(int)ReelPos], flagType);
        }

    }

    /// <summary>
    /// 停止コマ数検出
    /// </summary>
    /// <param name="ReelPos"> 停止したリール </param>
    /// <returns> 停止しているコマ番号 </returns>
    public int GetRotationLayer(ReelPosition ReelPos)
    {
        float currentAngle = Reel[(int)ReelPos].transform.eulerAngles.z;

        int layer = Mathf.FloorToInt(currentAngle / LayerAngle);

        return layer;
    }

    private void SetStopResult(ReelPosition ReelPos, int layer)
    {
        for (int i = 0; i < (int)ReelRow.E_REEL_ROW_MAX; i++)
        {
            int l = layer + i;
            if (l >= 21)
            {
                l -= 21;
            }

            StopResult[i, (int)ReelPos] = GetFlagType(ReelPos, l);
            //Debug.Log("[" + StopResult[i, (int)ReelPos] + "]" + "  ([" + i + "][" + (int)ReelPos + "])");
        }
    }

    /// <summary>
    /// リールの滑りコマ数計算
    /// </summary>
    /// <returns> 滑らせるコマ数 </returns>
    private int SlimeReel(int layer, FlagLottery.FlagType flagType)
    {
        int suberiKoma;
        if (flagType != FlagLottery.FlagType.E_FLAG_TYPE_REG)
        {
            suberiKoma = int.Parse(ReelRollDeta[(int)flagType][layer]);
        }
        else
        {
            suberiKoma = int.Parse(ReelRollDeta[(int)FlagLottery.FlagType.E_FLAG_TYPE_BIG][layer]);
        }

        return suberiKoma;
    }

    /// <summary>
    /// 停止したリール上に成立している役があるか検索
    /// </summary>
    /// <param name="ReelPos"> 停止したリール </param>
    /// <param name="layer"> 停止しているコマ番号 </param>   
    /// <param name="flagType"> 成立している子役 </param>
    /// <returns></returns>
    private ReelRow FindFlagPos(ReelPosition ReelPos, int layer, FlagLottery.FlagType flagType)
    {
        ReelRow reelRow = ReelRow.E_REEL_ROW_MAX;

        for(int i = 0; i < 3; i++)
        {
            int l = layer + i;
            if (l >= 21)
            {
                l -= 21;
            }

            //Debug.Log(l + "番を検索します");
            int Role = (int)GetFlagType(ReelPos, l);
            //Debug.Log("Roleの値：" + (FlagLottery.FlagType)Role);
            if (flagType != FlagLottery.FlagType.E_FLAG_TYPE_REG)
            {
                if (Role == (int)flagType)
                {
                    reelRow = (ReelRow)i;
                    break;
                }
            }
            else
            {
                if(ReelPos == ReelPosition.E_REEL_POS_R)
                {
                    if (Role == (int)FlagLottery.FlagType.E_FLAG_TYPE_REG)
                    {
                        reelRow = (ReelRow)i;
                        break;
                    }
                }
                else
                {
                    if (Role == (int)FlagLottery.FlagType.E_FLAG_TYPE_BIG)
                    {
                        reelRow = (ReelRow)i;
                        break;
                    }
                }
            }
        }

        //Debug.Log("有効な子役がある場所は" + reelRow + "です");

        return reelRow;
    }

    /// <summary>
    /// 利用できる有効ラインを見つける
    /// </summary>
    /// <param name="pos"> 停止したリール </param>
    /// <param name="row"> 成立している子役のあるライン </param>
    /// <param name="flagType"> 成立している子役 </param>
    private void FindLines(ReelPosition pos, ReelRow row, FlagLottery.FlagType flagType)
    {
        if (row == ReelRow.E_REEL_ROW_MAX) return;

        if (LinesNum.Count <= 0)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                bool[,] line = Lines[i];

                if (line[(int)row, (int)pos])
                {
                    LinesNum.Add(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < LinesNum.Count; i++)
            {
                bool[,] line = Lines[LinesNum[i]];

                if (!line[(int)row, (int)pos])
                {
                    LinesNum.RemoveAt(i);
                }
            }
        }

        //foreach (int i in LinesNum)
        //{
        //    Debug.Log("利用できる有効ライン：" + i);
        //}
    }

    /// <summary>
    /// 引き込み制御
    /// </summary>
    /// <param name="pos"> 停止したリール </param>
    /// <param name="layer"> 停止したコマ番号 </param>
    /// <param name="flagType"> 成立している子役 </param>
    /// <returns></returns>
    private int PullInControl(ReelPosition pos, int layer, FlagLottery.FlagType flagType)
    {
        int num = 0;
        bool[] ValidPos = new bool[3] { false, false, false };      //下中上

        //このリールはどの列なら子役成立に使えるか探す
        for (int i = 0; i < (int)ReelRow.E_REEL_ROW_MAX; i++)
        {
            //Debug.Log("有効なラインの数：" + LinesNum.Count + " i：" + i);
            foreach (int j in LinesNum)
            {
                //Debug.Log("Jの値：" + j);
                bool[,] line = Lines[j];      //使える有効ラインを一つ取り出す
                if (line[i, (int)pos])
                {
                    ValidPos[i] = true;
                    break;
                }
            }
        }
        //Debug.Log("下：" + ValidPos[0] + "　中：" + ValidPos[1] + "　上：" + ValidPos[2]);

        //4コマ滑り以内で子役成立するか確認する
        for(int i = 1; i <= 4; i++)     //滑りコマ数
        {
            for(int j = 0; j < (int)ReelRow.E_REEL_ROW_MAX; j++)    //列
            {
                if(ValidPos[j])     //その列が有効か
                {
                    int l = layer + j + i;
                    if (l >= 21)
                    {
                        l -= 21;
                    }
                    int Role = (int)GetFlagType(pos, l);
                    if (flagType == FlagLottery.FlagType.E_FLAG_TYPE_REG)   //レギュラー
                    {
                        if (pos == ReelPosition.E_REEL_POS_R)
                        {
                            if (Role == (int)FlagLottery.FlagType.E_FLAG_TYPE_REG)
                            {
                                num = i;
                                break;
                            }
                        }
                        else/* if (pos == ReelPosition.E_REEL_POS_C)*/
                        {
                            if (Role == (int)FlagLottery.FlagType.E_FLAG_TYPE_BIG)
                            {
                                num = i;
                                break;
                            }
                        }
                    }
                    else            //それ以外
                    {
                        if (Role == (int)flagType)
                        {
                            num = i;
                            break;
                        }
                    }
                }
            }
        }

        return num;
    }

    private int KickControl(FlagLottery.FlagType flagType, int layer)
    {
        int num = 0;

        for(int i = 0; i < Lines.Count; i++)
        {
            bool[,] line = Lines[i];
            bool b = false;
            FlagLottery.FlagType flag = FlagLottery.FlagType.E_FLAG_TYPE_MAX;

            Debug.Log("Line[" + i + "]の検証");

            for(int col = 0; col < (int)ReelRow.E_REEL_ROW_MAX; col++)
            {
                for (int row = 0; row < (int)ReelPosition.E_REEL_POS_MAX; row++)
                {
                    if(line[row, col])
                    {
                        Debug.Log("列：" + row + "　行：" + col + "　リールの値：" + StopResult[row, col] + "　成立役：" + flagType);
                        if (StopResult[row, col] != flagType)
                        {
                            if (flag == FlagLottery.FlagType.E_FLAG_TYPE_MAX)
                            {
                                flag = StopResult[row, col];
                                break;
                            }
                            else
                            {
                                if (flag == StopResult[row, col])
                                {
                                    break;
                                }
                                else
                                {
                                    b = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            b = true;
                            break;
                        }
                    }
                }

                if (b) break;
                if (col >= 2)
                {
                    Debug.Log("蹴った");
                    num++;
                    int l = layer + num;
                    if (l >= 21)
                    {
                        l -= 21;
                    }
                    SetStopResult(ReelPosition.E_REEL_POS_R, l);
                }
            }
        }

        return num;
    }

    /// <summary>
    /// 番号で指定されたところにある子役を返す
    /// </summary>
    /// <param name="pos"> 停止したリール </param>
    /// <param name="num"> 指定したいリール番号(0-20) </param>
    /// <returns> 子役の定数 </returns>
    private FlagLottery.FlagType GetFlagType(ReelPosition pos, int num)
    {
        FlagLottery.FlagType flag;

        //Debug.Log("[" + (int)pos + "]" + "[" + num + "]");
        flag = (FlagLottery.FlagType)int.Parse(ReelFlagData[(int)pos][num]);

        return flag;
    }

    /// <summary>
    /// 指定したラインとStopResultの一致をチェックする関数
    /// </summary>
    /// <param name="flagType"> 成立している子役 </param>
    /// <returns> 役が成立しているか </returns>
    public bool CheckWinCondition(FlagLottery.FlagType flagType)
    {
        if (flagType == FlagLottery.FlagType.E_FLAG_TYPE_CHERRY)
        {
            for (int col = 0; col < 3; col++)
            {
                if (StopResult[col, 0] == flagType)
                {
                    //Debug.Log("Winning line found(CHERRY)");
                    return true;
                }
            }
        }
        else if(flagType == FlagLottery.FlagType.E_FLAG_TYPE_REG)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                //Debug.Log("Line[" + i + "]の検索");
                bool isWinningLine = true;

                for (int col = 0; col < 3; col++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        if (Lines[i][row, col])
                        {
                            //Debug.Log("列：" + row + "行：" + col + "出目：" + StopResult[row, col]);
                            if(col == (int)ReelPosition.E_REEL_POS_R)
                            {
                                if (StopResult[row, col] == FlagLottery.FlagType.E_FLAG_TYPE_REG)
                                {
                                    //Debug.Log("整合");
                                    break;
                                }
                                else
                                {
                                    //Debug.Log("不整合");
                                    isWinningLine = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (StopResult[row, col] == FlagLottery.FlagType.E_FLAG_TYPE_BIG)
                                {
                                    //Debug.Log("整合");
                                    break;
                                }
                                else
                                {
                                    //Debug.Log("不整合");
                                    isWinningLine = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (!isWinningLine)
                        break;
                }

                if (isWinningLine)
                {
                    //Debug.Log("Winning line found: Lines[" + i + "]");
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                //Debug.Log("Line[" + i + "]の検索");
                bool isWinningLine = true;

                for (int col = 0; col < 3; col++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        if (Lines[i][row, col])
                        {
                            //Debug.Log("列：" + row + "行：" + col + "出目：" + StopResult[row, col]);
                            if (StopResult[row, col] == flagType)
                            {
                                //Debug.Log("整合");
                                break;
                            }
                            else
                            {
                                //Debug.Log("不整合");
                                isWinningLine = false;
                                break;
                            }
                        }
                    }

                    if (!isWinningLine)
                        break;
                }

                if (isWinningLine)
                {
                    //Debug.Log("Winning line found: Lines[" + i + "]");
                    return true;
                }
            }
        }

        //Debug.Log("No winning line found.");
        return false;
    }

    public void ResetReel()
    {
        LinesNum.Clear();
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                StopResult[row, col] = FlagLottery.FlagType.E_FLAG_TYPE_MAX;
            }
        }
    }
}
