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

    public int PayOut(FlagLottery.FlagType flagType)
    {
        int payout = 0;

        switch(flagType)
        {
            case FlagLottery.FlagType.E_FLAG_TYPE_CHERRY:
                payout = 2;
                break;
            case FlagLottery.FlagType.E_FLAG_TYPE_PIERROT:
                payout = 15;
                break;
            case FlagLottery.FlagType.E_FLAG_TYPE_BELL:
                payout = 10;
                break;
            case FlagLottery.FlagType.E_FLAG_TYPE_GRAPES:
                payout= 8;
                break;
            default:
                break;
        }

        AddMedal(payout);

        SoundManager.Instance.PlaySound(SoundType.SE_PAYOUT, 1.0f, false, payout);

        return payout;
    }


    public bool BounsCount(int addnum)
    {
        BounsNum += addnum;

        return false;
    }
}
