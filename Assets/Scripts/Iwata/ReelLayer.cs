using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelLayer : MonoBehaviour
{
    private const int LayerCount = 21;
    private const float LayerAngle = 360.0f / LayerCount; // 1‘w‚ ‚½‚è‚ÌŠp“x”ÍˆÍ

    // Start is called before the first frame update
    void Start()
    {

    }

    //public int GetRotationLayer()
    //{
    //    float currentAngle = transform.eulerAngles.z;

    //    int layer = Mathf.FloorToInt(currentAngle / LayerAngle);
        
    //    //transform.GetComponent<ReelRotation>().ControlRotationReel(LayerAngle * (float)layer);

    //    return layer;
    //}
}
