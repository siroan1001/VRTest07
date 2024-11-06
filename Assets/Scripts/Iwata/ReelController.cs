using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    public enum ReelPosition
    {
        E_REEL_POS_L = 0,
        E_REEL_POS_C,
        E_REEL_POS_R,
        E_REEL_POS_MAX,
    }

    public GameObject[] Reel = new GameObject[(int)ReelPosition.E_REEL_POS_MAX];
    [SerializeField] private float RotValue = 0.0f;

    public void StartReel()
    {
        for(int i = 0; i < Reel.Length; i++)
        {
            Reel[i].transform.GetComponent<ReelRotation>().rotFlag = true;
            Reel[i].transform.GetComponent<ReelRotation>().rotValue = RotValue;
        }
    }

    public void StopReel(ReelPosition ReelPos)
    {
        Reel[(int)ReelPos].transform.GetComponent<ReelRotation>().rotFlag = false;
    }

    ///// <summary>
    ///// �N�H�[�^�j�I�����I�C���[�p�ɕϊ�
    ///// </summary>
    //float convertQuaternionToEuler()
    //{
    //    float stopAngle;

    //    Vector3 quaAngle = new Vector3();
    //    quaAngle = _ReelTransform.forward; // �N�H�[�^�j�I�����擾

    //    if (quaAngle.z >= 0) // ���̒l�͈̔͂Ȃ�
    //    {
    //        float f = _ReelTransform.eulerAngles.x; // �I�C���[�p���擾

    //        if (f >= 270 && f < 360) // 270�ȏ� 360����
    //        {
    //            // A�͈̔�
    //            stopAngle = 360 - f;
    //        }
    //        else if (f >= 0 && f < 90) // 0�ȏ� 90����
    //        {
    //            // D�͈̔�
    //            stopAngle = 270 + (90 - f);
    //        }
    //        else
    //        {
    //            Debug.LogError("��~�ʒu����G���[ �I�C���[�p " + f);
    //        }
    //    }


    //    else if (quaAngle.z < 0) // ���̒l�͈̔͂Ȃ�
    //    {
    //        float f = _ReelTransform.eulerAngles.x; // �I�C���[�p���擾

    //        if (f >= 270 && f < 360) // 270�ȏ� 360����
    //        {
    //            // B�͈̔�
    //            stopAngle = 90 + (f - 270);
    //        }
    //        else if (f >= 0 && f < 90) // 0�ȏ� 90����
    //        {
    //            // C�͈̔�
    //            stopAngle = 180 + f;
    //        }
    //        else
    //        {
    //            Debug.LogError("��~�ʒu����G���[ �I�C���[�p��if�͈͊O " + f);
    //        }
    //    }

    //    else
    //    {
    //        Debug.LogError("��~�ʒu����G���[ �N�H�[�^�j�I����if�͈͊O " + quaAngle);
    //    }

    //    return stopAngle = Mathf.Round(stopAngle); // �����ۂ�
    //}
}
