using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalManager : MonoBehaviour
{
    enum SegNumber
    {
        E_CREGIT = 0,
        E_COUNT,
        E_PAYOUT,
        E_MAX,
    }

    int MedalNum;
    int BottomPlateNum;
    int BounsNum;
    int BetNum;
    [SerializeField] private GameObject[] SegObj = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        MedalNum = 0;
        BottomPlateNum = 0;
        BounsNum = 0;
        BetNum = 0;
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
        SetSegNumber(SegNumber.E_CREGIT, MedalNum);
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

        SetSegNumber(SegNumber.E_PAYOUT, payout);

        SoundManager.Instance.PlaySound(SoundType.SE_PAYOUT, 1.0f, false, payout);

        return payout;
    }


    public bool BounsCount(int addnum)
    {
        BounsNum += addnum;

        return false;
    }

    private void SetSegNumber(SegNumber seg, int num)
    {
        //Debug.Log("設定する数字：" + num);
        if (num == 0)
        {
            foreach (Transform child in SegObj[(int)seg].transform)
            {
                Debug.Log("セグの子オブジェクト：" + child.name);
                Digital7seg_Castam scr = child.GetComponent<Digital7seg_Castam>();
                if (scr == null) break;
                scr.SetNumber(0);
            }
            return;
        }
        for (int i = 0; num > 0; i++)
        {
            int ans = num % 10;
            num /= 10;
            Transform child = SegObj[(int)seg].transform.GetChild(i);
            child.GetComponent<Digital7seg_Castam>().SetNumber(ans);
        }
    }

    public int BetMedal()
    {
        for(; BetNum < 3; BetNum++)
        {
            if (MedalNum <= 0) break;
            AddMedal(-1);
        }
        return BetNum;
    }

    public void ResetBet()
    {
        BetNum = 0;
    }

    public void ResetPayout()
    {
        SetSegNumber(SegNumber.E_PAYOUT, 0);
    }
}
