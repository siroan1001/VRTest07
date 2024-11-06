// VR用コントローラの入力を反映 [VRInput.cs]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace StarterAssets
{
    public class VRInput : MonoBehaviour
    {
        private StarterAssetsInputs _input;
        private readonly SteamVR_Action_Vector2 Move = SteamVR_Actions.platformer.Move;
        private readonly SteamVR_Action_Boolean Jump = SteamVR_Actions.platformer.Jump;
        private readonly SteamVR_Action_Boolean GrabGrip = SteamVR_Actions._default.GrabGrip;

        // Start is called before the first frame update
        void Start()
        {
            _input = GetComponent<StarterAssetsInputs>();

            if (SteamVR.instance != null)
            {
                // 左右にplatformerを割り当て
                SteamVR_Actions.platformer.Activate(
                    activateForSource: SteamVR_Input_Sources.Any,
                    priority: 0,
                    disableAllOtherActionSets: false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // [Esc]で終了
            if (Input.GetKey(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                UnityEngine.Application.Quit();
#endif
            }

            if (SteamVR.instance != null)
            {
                Vector2 move = Move.GetAxis(SteamVR_Input_Sources.LeftHand);
                if (move.magnitude < 0.15f)
                {
                    move.x = move.y = 0.0f;
                }
                _input.MoveInput(move);
                bool jump = Jump.GetStateDown(SteamVR_Input_Sources.LeftHand);
                _input.JumpInput(jump);
                bool sprint = GrabGrip.GetState(SteamVR_Input_Sources.LeftHand);
                _input.SprintInput(sprint);

                Vector2 look = Move.GetAxis(SteamVR_Input_Sources.RightHand);
                look.y = 0.0f;
                _input.LookInput(look);
            }
        }
    }
}
