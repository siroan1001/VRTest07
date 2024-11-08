using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagLottery : MonoBehaviour
{
    public enum FlagType
    {
        E_FLAG_TYPE_BIG = 0,
        E_FLAG_TYPE_REG,
        E_FLAG_TYPE_CHERRY,
        E_FLAG_TYPE_PIERROT,
        E_FLAG_TYPE_BELL,
        E_FLAG_TYPE_GRAPES,
        E_FLAG_TYPE_REPLAY,
        E_FLAG_TYPE_MISS,
        E_FLAG_TYPE_MAX,
    }

    private static readonly int[] FlagValue = { 327, 655, 1310, 436, 436, 13943, 13107};

    private int _randomNumber; // フラグ抽選


    // Start is called before the first frame update
    void Start()
    {
        _randomNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FlagType RandFlag()
    {
        _randomNumber = Random.Range(0, 65535); // フラグ抽選
        FlagType type = FlagType.E_FLAG_TYPE_MAX;

        for(int i = 0; i < FlagValue.Length; i++)
        {
            _randomNumber -= FlagValue[i];

            if(_randomNumber <= 0)
            {
                type = (FlagType)i;
                return type;
            }
        }

        type = FlagType.E_FLAG_TYPE_MISS;
        return type;
    }

    public void Test()
    {
        int TestCase = 65536;
        int[] Case = new int[(int)FlagType.E_FLAG_TYPE_MAX];
        FlagType ans = 0;
        float par = 0.0f;

        for (int i = 0; i < TestCase; i++)
        {
            ans = RandFlag();

            Case[(int)ans] += 1;
        }

        for (int i = 0; i < (int)FlagType.E_FLAG_TYPE_MAX; i++)
        {
            FlagType flag = (FlagType)i;
            par = ((float)Case[i] / TestCase) * 100.0f;
            Debug.Log(flag + " ： " + Case[i] + "(" + par + ")");
        }
    }
}
