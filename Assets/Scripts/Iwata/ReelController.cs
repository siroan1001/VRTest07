using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ReelController : MonoBehaviour
{
    public enum ReelPosition
    {
        E_REEL_POS_L = 0,
        E_REEL_POS_C,
        E_REEL_POS_R,
        E_REEL_POS_MAX,
    }

    public GameObject[] Reel = new GameObject[(int)ReelPosition.E_REEL_POS_MAX];
    [SerializeField] private float RotValue = 0.0f;

    private List<string[]> ReelRollDeta;
    private static readonly string csvDataStr = "Reel_L_Control";

    private const int LayerCount = 21;      //リール上のコマ数
    private const float LayerAngle = 360.0f / LayerCount; // 1層あたりの角度範囲

    private void Start()
    {
        ReelRollDeta = ReadCSVFile.ReadCSV(csvDataStr);
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

    public void ControlReel(ReelPosition ReelPos, FlagLottery.FlagType flagType)
    {
        int Layer = 0;
        int Slime = 0;
        Layer = GetRotationLayer(ReelPos);
        Slime = SlimeReel(Layer, flagType);
        Debug.Log("成立役：" + flagType + "(" + (int)flagType + ")" + "　停止コマ：" + Layer + " 滑りコマ数：" + Slime);
        Reel[(int)ReelPos].GetComponent<ReelRotation>().ControlRotationReel(LayerAngle * (float)(Layer + Slime));
    }

    /// <summary>
    /// 停止コマ数検出
    /// </summary>
    /// <returns></returns>
    public int GetRotationLayer(ReelPosition ReelPos)
    {
        float currentAngle = Reel[(int)ReelPos].transform.eulerAngles.z;

        int layer = Mathf.FloorToInt(currentAngle / LayerAngle);

        return layer;
    }

    /// <summary>
    /// リールの滑りコマ数計算
    /// </summary>
    private int SlimeReel(int layer, FlagLottery.FlagType flagType)
    {
        int suberiKoma = int.Parse(ReelRollDeta[(int)flagType][layer]);

        return suberiKoma;
    }

}
