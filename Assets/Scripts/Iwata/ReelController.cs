using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        }
    }

    public void StopReel(ReelPosition ReelPos)
    {
        Reel[(int)ReelPos].transform.GetComponent<ReelRotation>().rotFlag = false;
    }

    public void ControlReel(ReelPosition ReelPos)
    {
        int Layer = 0;
        Layer = Reel[(int)ReelPos].transform.GetComponent<ReelLayer>().GetRotationLayer();
        Debug.Log("í‚é~ÉRÉ}ÅF" + Layer);
    }

}
