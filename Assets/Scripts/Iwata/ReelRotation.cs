using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelRotation : MonoBehaviour
{
    private bool RotFlag = false;

    private float rotSpeed = 0.0f;      //‰ñ“]‘¬“x
    public float rotationTime = 0.754f; // ˆê‰ñ“]‚É‚©‚©‚éŽžŠÔ‚ð•b‚ÅŽw’è
    private float LimitRot = 999.9f;    //‰ñ“]‚ð‚â‚ß‚éŠp“x


    // Start is called before the first frame update
    void Start()
    {
        RotFlag = false;
        rotSpeed = 360.0f / rotationTime; // ˆê‰ñ“]i360“xj‚ðŽw’èŽžŠÔ‚ÅŠ„‚Á‚Ä‘¬“x‚ðÝ’è
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (RotFlag)
        {
            gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, rotSpeed) * Time.deltaTime);
            if (transform.eulerAngles.z >= LimitRot)
            {
                Vector3 vec = new Vector3(0.0f, 270.0f, LimitRot);
                transform.eulerAngles = vec;
                LimitRot = 999.9f;
                RotFlag = false;
            }
        }
    }

    public bool rotFlag
    {
        set { RotFlag = value; }
    }

    public void ControlRotationReel(float Angle)
    {
        LimitRot = Angle;
    }
}
