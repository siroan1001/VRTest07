using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandController : MonoBehaviour
{
    public GameObject Slot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Hand")
        {
            Slot.GetComponent<MedalManager>().AddMedal(50);
            Debug.Log("“ŠŽ‘");
        }
    }
}
