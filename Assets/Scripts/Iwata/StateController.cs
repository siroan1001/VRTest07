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

    /// <summary>
    /// ���O�ɃE�F�C�g���I����������
    /// </summary>
    private float LastWaitTime;

    /// <summary>
    /// ���̉�]�ł̐�����
    /// </summary>
    private FlagLottery.FlagType LotterResult;

    /// <summary>
    /// �{�[�i�X�����t���O
    /// </summary>
    private bool BonusLamp;
    private int BounsPayout;
    private FlagLottery.FlagType BounsFlag;

    /// <summary>
    /// �q�𐬗��t���O
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
            Debug.Log("BIG�m��");
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().BigFlag = true;
            SoundManager.Instance.PlaySound(SoundType.SE_GAKO, 1.0f, false, 3);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }

        Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().GetRotationLayer(ReelController.ReelPosition.E_REEL_POS_L);


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
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().ResetReel();
            owner.FlagLamp = false;
            //Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }

        public override void OnClick(StateController owner)
        {
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            MedalManager script = owner.gameObject.GetComponent<MedalManager>();
            int num = script.BetMedal();
            Debug.Log("�q�����_����" +  num);
            if (num >= 3)
            { 
                owner.ChangeState(stateLeverOn);
                owner.GetComponent<MedalManager>().ResetPayout();
            }
            else Debug.Log("���_��������Ȃ�");

        }

    }

    public class StateLeverOn : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            //Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }



        public override void OnClick(StateController owner)
        {
            owner.LotterResult = Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().RandFlag();
            if (owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_BIG || owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_REG) owner.BonusLamp = true;
            Debug.Log("�������F" + owner.LotterResult);
            if(owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_BIG || owner.LotterResult == FlagLottery.FlagType.E_FLAG_TYPE_REG)
            {
                owner.BonusLamp = true;
                owner.BounsFlag = owner.LotterResult;
                Obj[(int)ObjSlotParts.E_SLOT_PARTS_BOUNSLAMP].GetComponent<BounsLampController>().LightOn();
            }
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateWaitTime);
        }
    }

    public class StateWaitTime : StateBase
    {
        public override void OnEnter(StateController owner, StateBase prevState)
        {
            SoundManager.Instance.PlaySound(SoundType.SE_WAIT, 1.0f , true);
            //Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
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
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
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
            //Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_L);
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
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
           // Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_C);
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
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
           // Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
        }


        public override void OnClick(StateController owner)
        {
            Obj[(int)ObjSlotParts.E_SLOT_PARTS_REEL].GetComponent<ReelController>().StopReel(ReelController.ReelPosition.E_REEL_POS_R);
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
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
            Debug.Log(owner.LotterResult + "�@�@" + owner.FlagLamp);
            if(owner.BonusLamp && owner.FlagLamp)
            {
                //�{�[�i�X�������Ă������邱��
                Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().BounsStart();
                Obj[(int)ObjSlotParts.E_SLOT_PARTS_BOUNSLAMP].GetComponent<BounsLampController>().LightOff();
                owner.BonusLamp = false;
            }
            //Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
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
            int payout = 0;
            if(owner.FlagLamp)
            {
                payout = owner.transform.GetComponent<MedalManager>().PayOut(owner.LotterResult);
                if (owner.BounsFlag == FlagLottery.FlagType.E_FLAG_TYPE_BIG || owner.BounsFlag == FlagLottery.FlagType.E_FLAG_TYPE_REG)
                {//�{�[�i�X
                    owner.BounsPayout += payout;
                    Debug.Log("�v" + owner.BounsPayout + "�������o��");
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
                    {//�{�[�i�X�I��
                        Debug.Log("�{�[�i�X�I��");
                        owner.BounsPayout = 0;
                        owner.BounsFlag = FlagLottery.FlagType.E_FLAG_TYPE_MAX;
                        Obj[(int)ObjSlotParts.E_SLOT_PARTS_LEVER].GetComponent<FlagLottery>().BounsEnd();
                    }
                }
            }
            //Debug.Log(this.GetType().Name + " �Ɉڍs���܂���");
            //OnClick(owner);
        }

        public override void OnUpdate(StateController owner)
        {
            OnClick(owner);
        }

        public override void OnClick(StateController owner)
        {
            owner.GetComponent<MedalManager>().ResetBet();
            // �X�e�[�g�ύX���܂��B���� stateLeverOn
            owner.ChangeState(stateMaxBet);
        }
    }

    public StateBase CurrentState
    {
        get { return currentState; }
    }
}