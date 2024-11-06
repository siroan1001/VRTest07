using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void StartReel()
    {
        for(int i = 0; i < Reel.Length; i++)
        {
            Reel[i].transform.GetComponent<ReelRotation>().rotFlag = true;
            Reel[i].transform.GetComponent<ReelRotation>().rotValue = RotValue;
        }
    }

    public void StopReel(ReelPosition ReelPos)
    {
        Reel[(int)ReelPos].transform.GetComponent<ReelRotation>().rotFlag = false;
    }

    ///// <summary>
    ///// クォータニオンをオイラー角に変換
    ///// </summary>
    //float convertQuaternionToEuler()
    //{
    //    float stopAngle;

    //    Vector3 quaAngle = new Vector3();
    //    quaAngle = _ReelTransform.forward; // クォータニオンを取得

    //    if (quaAngle.z >= 0) // 正の値の範囲なら
    //    {
    //        float f = _ReelTransform.eulerAngles.x; // オイラー角を取得

    //        if (f >= 270 && f < 360) // 270以上 360未満
    //        {
    //            // Aの範囲
    //            stopAngle = 360 - f;
    //        }
    //        else if (f >= 0 && f < 90) // 0以上 90未満
    //        {
    //            // Dの範囲
    //            stopAngle = 270 + (90 - f);
    //        }
    //        else
    //        {
    //            Debug.LogError("停止位置判定エラー オイラー角 " + f);
    //        }
    //    }


    //    else if (quaAngle.z < 0) // 負の値の範囲なら
    //    {
    //        float f = _ReelTransform.eulerAngles.x; // オイラー角を取得

    //        if (f >= 270 && f < 360) // 270以上 360未満
    //        {
    //            // Bの範囲
    //            stopAngle = 90 + (f - 270);
    //        }
    //        else if (f >= 0 && f < 90) // 0以上 90未満
    //        {
    //            // Cの範囲
    //            stopAngle = 180 + f;
    //        }
    //        else
    //        {
    //            Debug.LogError("停止位置判定エラー オイラー角がif範囲外 " + f);
    //        }
    //    }

    //    else
    //    {
    //        Debug.LogError("停止位置判定エラー クォータニオンがif範囲外 " + quaAngle);
    //    }

    //    return stopAngle = Mathf.Round(stopAngle); // 偶数丸め
    //}
}
