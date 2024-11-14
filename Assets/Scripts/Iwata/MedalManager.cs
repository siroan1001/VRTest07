using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalManager : MonoBehaviour
{
    int MedalNum;
    int BottomPlateNum;
    int BounsNum;

    // Start is called before the first frame update
    void Start()
    {
        MedalNum = 0;
        BottomPlateNum = 0;
        BounsNum = 0;
    }

    public void AddMedal(int num)
    {
        MedalNum += num;
        if(MedalNum > 50)
        {
            int hoge = MedalNum - 50;
            BottomPlateNum += hoge;
            MedalNum -= hoge;
        }
    }

    public void PayOut(FlagLottery.FlagType flagType)
    {
        switch(flagType)
        {
            case FlagLottery.FlagType.E_FLAG_TYPE_CHERRY:
                MedalNum += 2;
                break;
            case FlagLottery.FlagType.E_FLAG_TYPE_PIERROT:
                MedalNum += 15;
                break;
            case FlagLottery.FlagType.E_FLAG_TYPE_BELL:
                MedalNum += 10;
                break;
            case FlagLottery.FlagType.E_FLAG_TYPE_GRAPES:
                MedalNum += 8;
                break;
            default:
                break;
        }
    }


    public bool BounsCount(int addnum)
    {
        BounsNum += addnum;

        return false;
    }
}
