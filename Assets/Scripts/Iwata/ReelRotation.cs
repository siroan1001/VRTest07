using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelRotation : MonoBehaviour
{
    private bool RotFlag = false;

    private float rotSpeed = 0.0f;      //��]���x
    public float rotationTime = 0.754f; // ���]�ɂ����鎞�Ԃ�b�Ŏw��


    // Start is called before the first frame update
    void Start()
    {
        RotFlag = false;
        rotSpeed = 360.0f / rotationTime; // ���]�i360�x�j���w�莞�ԂŊ����đ��x��ݒ�
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (RotFlag)
        {
            gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, rotSpeed) * Time.deltaTime);
        }
    }

    public bool rotFlag
    {
        set { RotFlag = value; }
    }

    public void ControlRotationReel(float Angle)
    {
        while(true)
        {
            gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, rotSpeed) * Time.deltaTime);
            if(transform.eulerAngles.z >= Angle)
            {
                Vector3 vec = new Vector3(0.0f, 270.0f, Angle);
                transform.eulerAngles = vec;
                break;
            }
        }
    }
}
