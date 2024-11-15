using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digital7seg_Castam : MonoBehaviour
{
    private GameObject Digital7seg_model;
    private List<GameObject> segs = new List<GameObject>();
    private Seg segmentMap = new Seg();

    [SerializeField] private int Number = 0;

    void Start()
    {
        Digital7seg_model = this.gameObject;
        GetChildrens();
        InitSegment();

        // �e�X�g�p: 0 ��\��
        SetNumber(Number);
    }

    private void Update()
    {
        SetNumber(Number);
    }

    // �q�I�u�W�F�N�g�����X�g�ɒǉ�
    void GetChildrens()
    {
        for (int i = 0; i < 8; i++)
        {
            segs.Add(Digital7seg_model.transform.GetChild(i).gameObject);
        }
    }

    // �S�ẴZ�O�����g���A�N�e�B�u��
    void InitSegment()
    {
        foreach (var seg in segs)
        {
            seg.SetActive(false);
        }
    }

    // �����ɑΉ������Z�O�����g���A�N�e�B�u��
    public void SetNumber(int number)
    {
        // �S�Z�O�����g���A�N�e�B�u��
        InitSegment();

        // �Ή�����Z�O�����g���A�N�e�B�u��
        int[] activeSegments = number switch
        {
            0 => segmentMap.zero,
            1 => segmentMap.one,
            2 => segmentMap.two,
            3 => segmentMap.three,
            4 => segmentMap.four,
            5 => segmentMap.five,
            6 => segmentMap.six,
            7 => segmentMap.seven,
            8 => segmentMap.eight,
            9 => segmentMap.nine,
            _ => new int[0], // �����Ȑ����̏ꍇ�͉����A�N�e�B�u�����Ȃ�
        };


        foreach (var index in activeSegments)
        {
            segs[index].SetActive(true);
        }
    }
}

// �Z�O�����g�f�[�^���Ǘ�����N���X
public class Seg
{
    public int[] zero = { 0, 1, 2, 3, 4, 5 };
    public int[] one = { 1, 2 };
    public int[] two = { 0, 1, 3, 4, 6 };
    public int[] three = { 0, 1, 2, 3, 6 };
    public int[] four = { 1, 2, 5, 6 };
    public int[] five = { 0, 2, 3, 5, 6 };
    public int[] six = { 0, 2, 3, 4, 5, 6 };
    public int[] seven = { 0, 1, 2 };
    public int[] eight = { 0, 1, 2, 3, 4, 5, 6 };
    public int[] nine = { 0, 1, 2, 3, 5, 6 };
    public int[] dot = { 7 };
}

