using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionControler : MonoBehaviour
{
    public int Num;

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
            GameObject Parent = FindParentWithName(transform, "Slot");
            bool f = false;
            if (f = Parent.transform.GetComponent<StateController>().CheckCurrentState((StateController.Slot_State)Num))
            {
                Parent.transform.GetComponent<StateController>().OnClick();
            }
        }
    }

    GameObject FindParentWithName(Transform child, string parentName)
    {
        Transform parent = child.parent;

        // 親が存在しない場合はnullを返す
        if (parent == null) return null;

        // 親の名前が"Slot"であればその親オブジェクトを返す
        if (parent.name == parentName)
        {
            return parent.gameObject;
        }

        // 条件に合わなければさらに上の親を検索する
        return FindParentWithName(parent, parentName);
    }
}
