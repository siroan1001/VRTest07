using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State;

public partial class StateController : MonoBehaviour
{
    enum Slot_Parts
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


    public static GameObject[] Obj = new GameObject[(int)Slot_Parts.E_SLOT_PARTS_MAX];


    // �X�e�[�g�̃C���X�^���X  readonly�ɂ���ƒl�̕ύX������Ȃ��ϐ��ɂł���i�R���X�g���N�^���ł̂ݒl��ύX�ł���j
    private static readonly StateMaxBet stateMaxBet = new StateMaxBet();
    private static readonly StateLeverOn stateLeverOn = new StateLeverOn();
    private static readonly StateWaitTime stateWaitTime = new StateWaitTime();
    private static readonly StateFirstStopButton stateFirstStopButton = new StateFirstStopButton();
    private static readonly StateSecondStopButton stateSecondStopButton = new StateSecondStopButton();
    private static readonly StateThirdStopButton stateThirdStopButton = new StateThirdStopButton();
    private static readonly StateWinner stateWinner = new StateWinner();
    private static readonly StatePayOut statePayOut = new StatePayOut();

    /// <summary>
    /// ���݂̃X�e�[�g
    /// </summary>
    private StateBase currentState = stateMaxBet;

    private static float LastWaitTime;

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
        for(int i = 0; i < (int)Slot_Parts.E_SLOT_PARTS_MAX; i++)
        {
            Obj[i] = gameObject.transform.GetChild(i).gameObject;
        }
        LastWaitTime = Time.time;
        Debug.Log(LastWaitTime);
        currentState.OnEnter(this, null);
    }


    private void Update()
    {
        currentState.OnUpdate(this);

        if (Input.GetMouseButtonUp(0))
        {
            OnClick();
        }

    }


    // �N���b�N���ɌĂ΂��
    public void OnClick()
    {
        currentState.OnClick(this);
    }



    // �X�e�[�g�ύX
    public void ChangeState(StateBase nextState)
    {
        currentState.OnExit(this, nextState);
        nextState.OnEnter(this, currentState);
        currentState = nextState;
    }

    /// <summary>
    /// MaxBet�҂��̃X�e�[�g
    /// </summary>
    public class StateMaxBet : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }


        public override void OnClick(StateController owner)
        {
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateLeverOn);
        }

    }

    public class StateLeverOn : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }



        public override void OnClick(StateController owner)
        {
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateWaitTime);
        }

        public override void OnExit(StateController owner, StateBase nextState)
        {
            Obj[(int)Slot_Parts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StartReel();
        }
    }

    public class StateWaitTime : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }

        public override void OnUpdate(StateController owner)
        {
            float currentTime = Time.time;
            if (currentTime - LastWaitTime >= 4.1f)
            {
                LastWaitTime = currentTime;
                OnClick(owner);
            }
        }

        public override void OnClick(StateController owner)
        {
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateFirstStopButton);
        }

    }

    public class StateFirstStopButton : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)Slot_Parts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_L);
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateSecondStopButton);
        }

    }

    public class StateSecondStopButton : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)Slot_Parts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_C);
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateThirdStopButton);
        }

    }

    public class StateThirdStopButton : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)Slot_Parts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_R);
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateWinner);
        }

    }

    public class StateWinner : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
            //OnClick(owner);
        }

        public override void OnUpdate(StateController owner)
        {
            OnClick(owner);
        }


        public override void OnClick(StateController owner)
        {
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(statePayOut);
        }

    }

    public class StatePayOut : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
            //OnClick(owner);
        }

        public override void OnUpdate(StateController owner)
        {
            OnClick(owner);
        }

        public override void OnClick(StateController owner)
        {
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateMaxBet);
        }

    }

    public StateBase CurrentState
    {
        get { return currentState; }
    }
}