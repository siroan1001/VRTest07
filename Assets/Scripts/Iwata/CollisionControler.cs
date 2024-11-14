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

        // �e�����݂��Ȃ��ꍇ��null��Ԃ�
        if (parent == null) return null;

        // �e�̖��O��"Slot"�ł���΂��̐e�I�u�W�F�N�g��Ԃ�
        if (parent.name == parentName)
        {
            return parent.gameObject;
        }

        // �����ɍ���Ȃ���΂���ɏ�̐e����������
        return FindParentWithName(parent, parentName);
    }
}
