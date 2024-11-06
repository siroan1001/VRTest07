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
    ///// NH[^jIðIC[pÉÏ·
    ///// </summary>
    //float convertQuaternionToEuler()
    //{
    //    float stopAngle;

    //    Vector3 quaAngle = new Vector3();
    //    quaAngle = _ReelTransform.forward; // NH[^jIðæ¾

    //    if (quaAngle.z >= 0) // ³ÌlÌÍÍÈç
    //    {
    //        float f = _ReelTransform.eulerAngles.x; // IC[pðæ¾

    //        if (f >= 270 && f < 360) // 270Èã 360¢
    //        {
    //            // AÌÍÍ
    //            stopAngle = 360 - f;
    //        }
    //        else if (f >= 0 && f < 90) // 0Èã 90¢
    //        {
    //            // DÌÍÍ
    //            stopAngle = 270 + (90 - f);
    //        }
    //        else
    //        {
    //            Debug.LogError("â~Êu»èG[ IC[p " + f);
    //        }
    //    }


    //    else if (quaAngle.z < 0) // ÌlÌÍÍÈç
    //    {
    //        float f = _ReelTransform.eulerAngles.x; // IC[pðæ¾

    //        if (f >= 270 && f < 360) // 270Èã 360¢
    //        {
    //            // BÌÍÍ
    //            stopAngle = 90 + (f - 270);
    //        }
    //        else if (f >= 0 && f < 90) // 0Èã 90¢
    //        {
    //            // CÌÍÍ
    //            stopAngle = 180 + f;
    //        }
    //        else
    //        {
    //            Debug.LogError("â~Êu»èG[ IC[pªifÍÍO " + f);
    //        }
    //    }

    //    else
    //    {
    //        Debug.LogError("â~Êu»èG[ NH[^jIªifÍÍO " + quaAngle);
    //    }

    //    return stopAngle = Mathf.Round(stopAngle); // ôÛß
    //}
}
