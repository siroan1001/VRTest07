using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digital7seg : MonoBehaviour
{
    private GameObject Digital7seg_model;
    private List<GameObject> segs = new List<GameObject>();
    void Start()
    {
        Digital7seg_model = this.gameObject;
        GetChildlens();
        InitSegment();
        Seg num = new Seg();
        foreach (var n in num.zero)
        {
            segs[n].SetActive(true);
        }
    }
    void GetChildlens()
    {
        for (int i = 0; i < 8; i++)
        {
            segs.Add(Digital7seg_model.transform.GetChild(i).gameObject);
        }
    }

    void InitSegment()
    {
        for (int i = 0; i < 8; i++)
        {
            segs[i].SetActive(false);
        }
    }

    private class Seg
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
}
