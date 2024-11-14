using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class ReelController : MonoBehaviour
{
    /// <summary>
    /// ���[�������ʂ���萔
    /// </summary>
    public enum ReelPosition
    {
        E_REEL_POS_L = 0,
        E_REEL_POS_C,
        E_REEL_POS_R,
        E_REEL_POS_MAX,
    }

    /// <summary>
    /// ������ʂ���萔
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
    /// ����R�}�̃f�[�^
    /// </summary>
    private List<string[]> ReelRollDeta;
    private static readonly string csvReelRollDetaStr = "Reel_L_Control";

    /// <summary>
    /// ���[����̃R�}��
    /// </summary>
    private const int LayerCount = 21;
    /// <summary>
    /// 1�w������̊p�x�͈�
    /// </summary>
    private const float LayerAngle = 360.0f / LayerCount;

    /// <summary>
    /// ���[���̎q���̔z��f�[�^
    /// </summary>
    private List<string[]> ReelFlagData;
    private static readonly string csvReelFlagDataStr = "Reel_Flag_Position";

    /// <summary>
    /// �L�����C�� [�ǂ̃��C����, �ǂ̃��[����]
    /// </summary>
    private static readonly List<bool[,]> Lines = new List<bool[,]>
    {
        new bool[3, 3]      //���i
        {
            //��     ��     �E
            { true , true , true  },        //���i
            { false, false, false },        //���i
            { false, false, false }         //��i
        },
        new bool[3, 3]      //���i
        {
            { false, false, false },
            { true , true , true  },
            { false, false, false }
        },
        new bool[3, 3]      //��i
        {
            { false, false, false },
            { false, false, false },
            { true , true , true  }
        },
        new bool[3, 3]      //���΂�
        {
            { false, false, true  },
            { false, true , false },
            { true , false, false }
        },
        new bool[3, 3]      //�E�΂�
        {
            { true , false, false },
            { false, true , false },
            { false, false, true  }
        }
    };

    /// <summary>
    /// ���I���Ă���q��������|�W�V�������i�[
    /// </summary>
    private ReelRow[] FlagRow = new ReelRow[3];

    /// <summary>
    /// ����L���ȗL�����C���̔ԍ����i�[
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
    /// �{�^����������̃R�}���萧��
    /// </summary>
    /// <param name="ReelPos"> �ǂ̃��[���� </param>
    /// <param name="flagType"> �������Ă���q�� </param>
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
        //Debug.Log("�������F" + flagType + "(" + (int)flagType + ")" + "�@��~�R�}�F" + Layer + " ����R�}���F" + PullIn);

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
    /// ��~�R�}�����o
    /// </summary>
    /// <param name="ReelPos"> ��~�������[�� </param>
    /// <returns> ��~���Ă���R�}�ԍ� </returns>
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
    /// ���[���̊���R�}���v�Z
    /// </summary>
    /// <returns> ���点��R�}�� </returns>
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
    /// ��~�������[����ɐ������Ă���������邩����
    /// </summary>
    /// <param name="ReelPos"> ��~�������[�� </param>
    /// <param name="layer"> ��~���Ă���R�}�ԍ� </param>   
    /// <param name="flagType"> �������Ă���q�� </param>
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

            //Debug.Log(l + "�Ԃ��������܂�");
            int Role = (int)GetFlagType(ReelPos, l);
            //Debug.Log("Role�̒l�F" + (FlagLottery.FlagType)Role);
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

        //Debug.Log("�L���Ȏq��������ꏊ��" + reelRow + "�ł�");

        return reelRow;
    }

    /// <summary>
    /// ���p�ł���L�����C����������
    /// </summary>
    /// <param name="pos"> ��~�������[�� </param>
    /// <param name="row"> �������Ă���q���̂��郉�C�� </param>
    /// <param name="flagType"> �������Ă���q�� </param>
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
        //    Debug.Log("���p�ł���L�����C���F" + i);
        //}
    }

    /// <summary>
    /// �������ݐ���
    /// </summary>
    /// <param name="pos"> ��~�������[�� </param>
    /// <param name="layer"> ��~�����R�}�ԍ� </param>
    /// <param name="flagType"> �������Ă���q�� </param>
    /// <returns></returns>
    private int PullInControl(ReelPosition pos, int layer, FlagLottery.FlagType flagType)
    {
        int num = 0;
        bool[] ValidPos = new bool[3] { false, false, false };      //������

        //���̃��[���͂ǂ̗�Ȃ�q�𐬗��Ɏg���邩�T��
        for (int i = 0; i < (int)ReelRow.E_REEL_ROW_MAX; i++)
        {
            //Debug.Log("�L���ȃ��C���̐��F" + LinesNum.Count + " i�F" + i);
            foreach (int j in LinesNum)
            {
                //Debug.Log("J�̒l�F" + j);
                bool[,] line = Lines[j];      //�g����L�����C��������o��
                if (line[i, (int)pos])
                {
                    ValidPos[i] = true;
                    break;
                }
            }
        }
        //Debug.Log("���F" + ValidPos[0] + "�@���F" + ValidPos[1] + "�@��F" + ValidPos[2]);

        //4�R�}����ȓ��Ŏq�𐬗����邩�m�F����
        for(int i = 1; i <= 4; i++)     //����R�}��
        {
            for(int j = 0; j < (int)ReelRow.E_REEL_ROW_MAX; j++)    //��
            {
                if(ValidPos[j])     //���̗񂪗L����
                {
                    int l = layer + j + i;
                    if (l >= 21)
                    {
                        l -= 21;
                    }
                    int Role = (int)GetFlagType(pos, l);
                    if (flagType == FlagLottery.FlagType.E_FLAG_TYPE_REG)   //���M�����[
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
                    else            //����ȊO
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

            Debug.Log("Line[" + i + "]�̌���");

            for(int col = 0; col < (int)ReelRow.E_REEL_ROW_MAX; col++)
            {
                for (int row = 0; row < (int)ReelPosition.E_REEL_POS_MAX; row++)
                {
                    if(line[row, col])
                    {
                        Debug.Log("��F" + row + "�@�s�F" + col + "�@���[���̒l�F" + StopResult[row, col] + "�@�������F" + flagType);
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
                    Debug.Log("�R����");
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
    /// �ԍ��Ŏw�肳�ꂽ�Ƃ���ɂ���q����Ԃ�
    /// </summary>
    /// <param name="pos"> ��~�������[�� </param>
    /// <param name="num"> �w�肵�������[���ԍ�(0-20) </param>
    /// <returns> �q���̒萔 </returns>
    private FlagLottery.FlagType GetFlagType(ReelPosition pos, int num)
    {
        FlagLottery.FlagType flag;

        //Debug.Log("[" + (int)pos + "]" + "[" + num + "]");
        flag = (FlagLottery.FlagType)int.Parse(ReelFlagData[(int)pos][num]);

        return flag;
    }

    /// <summary>
    /// �w�肵�����C����StopResult�̈�v���`�F�b�N����֐�
    /// </summary>
    /// <param name="flagType"> �������Ă���q�� </param>
    /// <returns> �����������Ă��邩 </returns>
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
                //Debug.Log("Line[" + i + "]�̌���");
                bool isWinningLine = true;

                for (int col = 0; col < 3; col++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        if (Lines[i][row, col])
                        {
                            //Debug.Log("��F" + row + "�s�F" + col + "�o�ځF" + StopResult[row, col]);
                            if(col == (int)ReelPosition.E_REEL_POS_R)
                            {
                                if (StopResult[row, col] == FlagLottery.FlagType.E_FLAG_TYPE_REG)
                                {
                                    //Debug.Log("����");
                                    break;
                                }
                                else
                                {
                                    //Debug.Log("�s����");
                                    isWinningLine = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (StopResult[row, col] == FlagLottery.FlagType.E_FLAG_TYPE_BIG)
                                {
                                    //Debug.Log("����");
                                    break;
                                }
                                else
                                {
                                    //Debug.Log("�s����");
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
                //Debug.Log("Line[" + i + "]�̌���");
                bool isWinningLine = true;

                for (int col = 0; col < 3; col++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        if (Lines[i][row, col])
                        {
                            //Debug.Log("��F" + row + "�s�F" + col + "�o�ځF" + StopResult[row, col]);
                            if (StopResult[row, col] == flagType)
                            {
                                //Debug.Log("����");
                                break;
                            }
                            else
                            {
                                //Debug.Log("�s����");
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
