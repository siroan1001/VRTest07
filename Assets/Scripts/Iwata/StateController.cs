using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State;

public partial class StateController : MonoBehaviour
{
    enum ObjSlotParts
    {
        E_SLOT_PARTS_BUTTON = 0, 
        E_SLOT_PARTS_JUGGLER, 
        E_SLOT_PARTS_LEVER, 
        E_SLOT_PARTS_REEL, 
        E_SLOT_PARTS_MAXBET, 
        E_SLOT_PARTS_CREDITSEG, 
        E_SLOT_PARTS_COUNTSEG, 
        E_SLOT_PARTS_PAYOUTSEG, 
        E_SLOT_PARTS_BOUNSLAMP, 
        E_SLOT_PARTS_MAX 
    }

    public enum Slot_State
    {
        E_SLOT_STATE_MAXBET = 0,
        E_SLOT_STATE_LEVERON,
        E_SLOT_STATE_WAITTIME,
        E_SLOT_STATE_FBUTTON,
        E_SLOT_STATE_SBUTTON,
        E_SLOT_STATE_TBUTTON,
        E_SLOT_STATE_WINNER,
        E_SLOT_STATE_PAYOUT,
        E_SLOT_STATE_MAX,
    }


    public static GameObject[] Obj = new GameObject[(int)ObjSlotParts.E_SLOT_PARTS_MAX];


    // ステートのインスタンス  readonlyにすると値の変更がされない変数にできる（コンストラクタ内でのみ値を変更できる）
    private static readonly StateMaxBet stateMaxBet = new StateMaxBet();
    private static readonly StateLeverOn stateLeverOn = new StateLeverOn();
    private static readonly StateWaitTime stateWaitTime = new StateWaitTime();
    private static readonly StateFirstStopButton stateFirstStopButton = new StateFirstStopButton();
    private static readonly StateSecondStopButton stateSecondStopButton = new StateSecondStopButton();
    private static readonly StateThirdStopButton stateThirdStopButton = new StateThirdStopButton();
    private static readonly StateWinner stateWinner = new StateWinner();
    private static readonly StatePayOut statePayOut = new StatePayOut();

    /// <summary>
    /// 現在のステート
    /// </summary>
    private StateBase currentState = stateMaxBet;

    /// <summary>
    /// 直前にウェイトが終了した時間
    /// </summary>
    private float LastWaitTime;

    /// <summary>
    /// その回転での成立役
    /// </summary>
    private FlagLottery.FlagType LotterResult;

    /// <summary>
    /// ボーナス成立フラグ
    /// </summary>
    private bool BonusLamp;
    private int BounsPayout;
    private FlagLottery.FlagType BounsFlag;

    /// <summary>
    /// 子役成立フラグ
    /// </summary>
    private bool FlagLamp;

    public bool CheckCurrentState(Slot_State state)
    {
        switch(state)
        {
            case Slot_State.E_SLOT_STATE_MAXBET:
                if (currentState == stateMaxBet) return true;
                break;
            case Slot_State.E_SLOT_STATE_LEVERON:
                if (currentState == stateLeverOn) return true;
                break;
            case Slot_State.E_SLOT_STATE_WAITTIME:
                if (currentState == stateWaitTime) return true;
                break;
            case Slot_State.E_SLOT_STATE_FBUTTON:
                if (currentState == stateFirstStopButton) return true;
                break;
            case Slot_State.E_SLOT_STATE_SBUTTON:
                if (currentState == stateSecondStopButton) return true;
                break;
            case Slot_State.E_SLOT_STATE_TBUTTON:
                if (currentState == stateThirdStopButton) return true;
                break;
            case Slot_State.E_SLOT_STATE_WINNER:
                if (currentState == stateWinner) return true;
                break;
            case Slot_State.E_SLOT_STATE_PAYOUT:
                if (currentState == statePayOut) return true;
                break;
            case Slot_State.E_SLOT_STATE_MAX:
                break;
        }

        return false;
    }

    private void Start()
    {
        for(int i = 0; i < (int)ObjSlotParts.E_SLOT_PARTS_MAX; i++)
        {
            Obj[i] = gameObject.transform.GetChild(i).gameObject;
        }
        LastWaitTime = Time.time;
        LotterResult = FlagLottery.FlagType.E_FLAG_TYPE_MAX;
        BonusLamp = false;
        BounsPayout = 0;
        BounsFlag = FlagLottery.FlagType.E_FLAG_TYPE_MAX;
        FlagLamp = false;
        //Obj[(int)Slot_Parts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().Test();
        currentState.OnEnter(this, null);
    }

    private void Update()
    {
        currentState.OnUpdate(this);

        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("BIG確定");
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().BigFlag = true;
            SoundManager.Instance.PlaySound(SoundType.SE_GAKO, 1.0f, false, 3);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }

        Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().GetRotationLayer(ReelController.ReelPosition.E_REEL_POS_L);


    }

    // クリック時に呼ばれる
    public void OnClick()
    {
        currentState.OnClick(this);
    }

    // ステート変更
    public void ChangeState(StateBase nextState)
    {
        currentState.OnExit(this, nextState);
        nextState.OnEnter(this, currentState);
        currentState = nextState;
    }

    /// <summary>
    /// MaxBet待ちのステート
    /// </summary>
    public class StateMaxBet : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().ResetReel();
            owner.FlagLamp = false;
            //Debug.Log(this.GetType().Name + " に移行しました");
        }

        public override void OnClick(StateController owner)
        {
            // ステート変更します。次は stateLeverOn
            MedalManager script = owner.gameObject.GetComponent<MedalManager>();
            int num = script.BetMedal();
            Debug.Log("賭けメダル数" +  num);
            if (num >= 3)
            { 
                owner.ChangeState(stateLeverOn);
                owner.GetComponent<MedalManager>().ResetPayout();
            }
            else Debug.Log("メダルが足らない");

        }

    }

    public class StateLeverOn : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            //Debug.Log(this.GetType().Name + " に移行しました");
        }



        public override void OnClick(StateController owner)
        {
            owner.LotterResult = Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().RandFlag();
            if (owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_BIG || owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_REG) owner.BonusLamp = true;
            Debug.Log("成立役：" + owner.LotterResult);
            if(owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_BIG || owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_REG)
            {
                owner.BonusLamp = true;
                owner.BounsFlag = owner.LotterResult;
                Obj[(int)ObjSlotParts.E_SLOT_PARTS_BOUNSLAMP].GetComponent<BounsLampController>().LightOn();
            }
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateWaitTime);
        }
    }

    public class StateWaitTime : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            SoundManager.Instance.PlaySound(SoundType.SE_WAIT, 1.0f , true);
            //Debug.Log(this.GetType().Name + " に移行しました");
        }

        public override void OnUpdate(StateController owner)
        {
            float currentTime = Time.time;
            if (currentTime - owner.LastWaitTime >= 4.1f)
            {
                owner.LastWaitTime = currentTime;
                OnClick(owner);
            }
        }

        public override void OnClick(StateController owner)
        {
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateFirstStopButton);
        }

        public override void OnExit(StateController owner, StateBase nextState)
        {
            SoundManager.Instance.StopSound(SoundType.SE_WAIT);
            SoundManager.Instance.PlaySound(SoundType.SE_ROLL);
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StartReel();
        }

    }

    public class StateFirstStopButton : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            //Debug.Log(this.GetType().Name + " に移行しました");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_L);
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateSecondStopButton);
        }

        public override void OnExit(StateController owner, StateBase nextState)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().ControlReel(ReelController.ReelPosition.E_REEL_POS_L, owner.LotterResult);
        }

    }

    public class StateSecondStopButton : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
           // Debug.Log(this.GetType().Name + " に移行しました");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_C);
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateThirdStopButton);
        }
        public override void OnExit(StateController owner, StateBase nextState)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().ControlReel(ReelController.ReelPosition.E_REEL_POS_C, owner.LotterResult);
        }

    }

    public class StateThirdStopButton : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
           // Debug.Log(this.GetType().Name + " に移行しました");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_R);
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateWinner);
        }

        public override void OnExit(StateController owner, StateBase nextState)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().ControlReel(ReelController.ReelPosition.E_REEL_POS_R, owner.LotterResult);
        }

    }

    public class StateWinner : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            owner.FlagLamp = Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().CheckWinCondition(owner.LotterResult);
            Debug.Log(owner.LotterResult + "　　" + owner.FlagLamp);
            if(owner.BonusLamp && owner.FlagLamp)
            {
                //ボーナス成立していたらやること
                Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().BounsStart();
                Obj[(int)ObjSlotParts.E_SLOT_PARTS_BOUNSLAMP].GetComponent<BounsLampController>().LightOff();
                owner.BonusLamp = false;
            }
            //Debug.Log(this.GetType().Name + " に移行しました");
            //OnClick(owner);
        }

        public override void OnUpdate(StateController owner)
        {
            OnClick(owner);
        }

        public override void OnClick(StateController owner)
        {
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(statePayOut);
        }

    }

    public class StatePayOut : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            int payout = 0;
            if(owner.FlagLamp)
            {
                payout = owner.transform.GetComponent<MedalManager>().PayOut(owner.LotterResult);
                if (owner.BounsFlag == FlagLottery.FlagType.E_FLAG_TYPE_BIG || owner.BounsFlag == FlagLottery.FlagType.E_FLAG_TYPE_REG)
                {//ボーナス
                    owner.BounsPayout += payout;
                    Debug.Log("計" + owner.BounsPayout + "枚払い出し");
                    int max = 0;
                    switch(owner.BounsFlag)
                    {
                        case FlagLottery.FlagType.E_FLAG_TYPE_BIG:
                            max = 266;
                            break;
                        case FlagLottery.FlagType.E_FLAG_TYPE_REG:
                            max = 98;
                            break;
                        default:
                            break;
                    }
                    if(owner.BounsPayout >= max)
                    {//ボーナス終了
                        Debug.Log("ボーナス終了");
                        owner.BounsPayout = 0;
                        owner.BounsFlag = FlagLottery.FlagType.E_FLAG_TYPE_MAX;
                        Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().BounsEnd();
                    }
                }
            }
            //Debug.Log(this.GetType().Name + " に移行しました");
            //OnClick(owner);
        }

        public override void OnUpdate(StateController owner)
        {
            OnClick(owner);
        }

        public override void OnClick(StateController owner)
        {
            owner.GetComponent<MedalManager>().ResetBet();
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateMaxBet);
        }
    }

    public StateBase CurrentState
    {
        get { return currentState; }
    }
}