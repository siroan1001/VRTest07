using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSlotMove : MonoBehaviour
{
    enum WASD
    {
        UP = 0,
        LEFT,
        DOWN,
        RIGHT,
        MAX
    }


    [SerializeField] KeyCode[] key = new KeyCode[4];
    [SerializeField] float moveValue = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 vec = transform.position;

        if (Input.GetKey(key[(int)WASD.UP]))
        {
            vec.y += moveValue;
        }
        if (Input.GetKey(key[(int)WASD.LEFT]))
        {
            vec.x -= moveValue;
        }
        if (Input.GetKey(key[(int)WASD.DOWN]))
        {
            vec.y -= moveValue;
        }
        if (Input.GetKey(key[(int)WASD.RIGHT]))
        {
            vec.x += moveValue;
        }

        transform.position = vec;
    }
}
