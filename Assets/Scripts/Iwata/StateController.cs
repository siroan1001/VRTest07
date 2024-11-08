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
        //Obj[(int)Slot_Parts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().Test();
        currentState.OnEnter(this, null);
    }


    private void Update()
    {
        currentState.OnUpdate(this);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }

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
            Debug.Log(this.GetType().Name + " に移行しました");
        }

        public override void OnClick(StateController owner)
        {
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateLeverOn);
        }

    }

    public class StateLeverOn : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " に移行しました");
        }



        public override void OnClick(StateController owner)
        {
            owner.LotterResult = Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().RandFlag();
            Debug.Log("成立役：" + owner.LotterResult);
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateWaitTime);
        }

        //public override void OnExit(StateController owner, StateBase nextState)
        //{
        //    Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StartReel();
        //}
    }

    public class StateWaitTime : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " に移行しました");
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
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StartReel();
        }

    }

    public class StateFirstStopButton : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " に移行しました");
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
            Debug.Log(this.GetType().Name + " に移行しました");
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
            Debug.Log(this.GetType().Name + " に移行しました");
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
            Debug.Log(this.GetType().Name + " に移行しました");
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
            Debug.Log(this.GetType().Name + " に移行しました");
            //OnClick(owner);
        }

        public override void OnUpdate(StateController owner)
        {
            OnClick(owner);
        }

        public override void OnClick(StateController owner)
        {
            // ステート変更します。次は stateLeverOn
            owner.ChangeState(stateMaxBet);
        }

    }

    public StateBase CurrentState
    {
        get { return currentState; }
    }
}