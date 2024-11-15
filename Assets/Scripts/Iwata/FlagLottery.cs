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

    //private static readonly int[] FlagValue = { 327, 655, 1310, 436, 436, 13943, 13107};
    private static readonly int[] FlagValue = { 327, 655, 1310, 436, 436, 13943, 13107};
    private static readonly int[] BounsValue = { 0, 0, 1310, 50, 50, 64126, 0};

    private int _randomNumber; // フラグ抽選

    private bool BounsLamp;     //ボーナス中フラグ

    private bool BounsLock;     //ボーナス当選時再度抽選を行わないためのフラグ
    private FlagType BounsFlag;     //ボーナス当選時再度抽選を行わないためのフラグ

    public bool BigFlag;


    // Start is called before the first frame update
    void Start()
    {
        _randomNumber = 0;
        BounsLamp = false;
        BounsFlag = FlagType.E_FLAG_TYPE_MAX;
        BigFlag = false;
    }

    public FlagType RandFlag()
    {
        if (BounsFlag != FlagType.E_FLAG_TYPE_MAX) return BounsFlag;

        _randomNumber = Random.Range(0, 65535); // フラグ抽選
        FlagType type = FlagType.E_FLAG_TYPE_MAX;

        int[] val;
        if (!BounsLamp)
        {
            val = FlagValue;
            Debug.Log("通常抽選");
            if(BigFlag)
            {
                BigFlag = false;
                return FlagType.E_FLAG_TYPE_BIG;
            }
        }
        else
        {
            val = BounsValue;
            Debug.Log("ボーナス抽選");
        }
            

        for(int i = 0; i < val.Length; i++)
        {
            _randomNumber -= val[i];

            if(_randomNumber <= 0)
            {
                type = (FlagType)i;
                if(type == FlagType.E_FLAG_TYPE_BIG || type == FlagType.E_FLAG_TYPE_REG)
                {
                    BounsFlag = type;
                }
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

    public void BounsStart()
    {
        BounsLamp = true;
        BounsFlag = FlagType.E_FLAG_TYPE_MAX;
    }

    public void BounsEnd()
    {
        BounsLamp = false;
    }

    public FlagType Bounsflag
    {
        get { return BounsFlag; }
    }
}