using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelRotation : MonoBehaviour
{
    private float rot = 0.0f;
    private bool RotFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        RotFlag = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(RotFlag)     gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, rot) * Time.deltaTime);
    }

    public bool rotFlag
    {
        set { RotFlag = value; }
    }

    public float rotValue
    {
        set { rot = value; }
    }

}
