using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounsLampController : MonoBehaviour
{
    Transform cube;

    // Start is called before the first frame update
    void Start()
    {
        cube = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LightOn();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            LightOff();
        }
    }

    public void LightOn()
    {
        cube.GetComponent<Renderer>().material.color = Color.blue;
    }

    public void LightOff()
    {
        cube.GetComponent<Renderer>().material.color = Color.gray;
    }
}
