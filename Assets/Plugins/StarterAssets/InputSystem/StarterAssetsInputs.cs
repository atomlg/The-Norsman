using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public float aim;
        public bool jump;
        public bool sprint;
        public bool attack;
        public bool collect;
        public bool block;
        public bool fire;
        public bool westLoadout;
        public bool northLoadout;
        public bool eastLoadout;
        public bool southLoadout;


        [Header("Movement Settings")]
        public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnCollect(InputValue value)
        {
            CollectInput(value.isPressed);
        }

        public void OnAttack(InputValue value)
        {
            AttackInput(value.isPressed);
        }

        public void OnBlock(InputValue value)
        {
            BlockInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            AimInput(value.Get<float>());
        }

        public void OnFire(InputValue value)
        {
            FireInput(value.isPressed);
        }

        public void OnWestLoadout(InputValue value)
        {
            WestLoadoutInput(value.isPressed);
        }

        public void OnNorthLoadout(InputValue value)
        {
            NorthLoadoutInput(value.isPressed);
        }

        public void OnEastLoadout(InputValue value)
        {
            EastLoadoutInput(value.isPressed);
        }

        public void OnSouthLoadout(InputValue value)
        {
            SouthLoadoutInput(value.isPressed);
        }
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void CollectInput(bool newCollectState)
        {
            collect = newCollectState;
        }

        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        public void BlockInput(bool newBlockState)
        {
            block = newBlockState;
        }

        public void AimInput(float newAimState)
        {
            aim = newAimState > 0.5f ? 1.0f : 0f;
        }

        public void FireInput(bool newFireState)
        {
            if (aim > 0f)
            {
                fire = newFireState;
            }
            else fire = false;
        }

        public void WestLoadoutInput(bool newLoadoutState)
        {
            westLoadout = newLoadoutState;
        }

        public void NorthLoadoutInput(bool newLoadoutState)
        {
            northLoadout = newLoadoutState;
        }

        public void EastLoadoutInput(bool newLoadoutState)
        {
            eastLoadout = newLoadoutState;
        }
        public void SouthLoadoutInput(bool newLoadoutState)
        {
            southLoadout = newLoadoutState;
        }

#if !UNITY_IOS || !UNITY_ANDROID
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
#endif
    }
}