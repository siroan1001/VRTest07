using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxBetControler : MonoBehaviour
{
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
            if (transform.parent.transform.GetComponent<StateController>().CheckCurrentState(StateController.Slot_State.E_SLOT_STATE_MAXBET))
            {
                transform.parent.transform.GetComponent<StateController>().OnClick();
            }
        }
    }
}
