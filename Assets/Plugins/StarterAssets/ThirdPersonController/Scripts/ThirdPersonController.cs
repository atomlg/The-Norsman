//using Game.Character;
//using Game.Equipment;
//using Game.Interactions;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        [Tooltip("Aim movement speed of the character moves in m/s")]
        public float AimSpeed = 1.0f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float AimTimeout = 0.8f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        // public Equipment.LoadoutZone CurrentLoadout = Equipment.LoadoutZone.None;

        [Tooltip("Amount of time player will not be able to move during an attack")]
        public float AttackHoldTime = 0.8f;

        [Tooltip("Amount of time player will not be able to move while picking up objects")]
        public float CollectHoldTime = 1.2f;


        [Header("Cinemachine Look Cam")]
        [Tooltip("The follow target set in the Cinemachine Virtual Follow Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        public GameObject CinemachineCameraAimTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;
        public float AimCameraAngleOffset = 0f;
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;


        [Header("Cinemachine Aim Cam")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraFollow;
        public GameObject CinemachineCameraAim;
        // public AimControl.AimControlState aimControlState;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _aimRotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _aimTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDCollect;
        private int _animIDAim;
        private int _animIDFire;

        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        //private Equipment _equipment;
        //private AimControl _aim;

        private const float _threshold = 0.01f;
        private bool _hasAnimator;
        private bool _canBlock;
        private bool _canAim;
        private bool _aimComplete;
        private bool _aimStarted;
        private bool _aimReset = false;

        public enum MotionState
        {
            None = 0,
            Moving,
            Sprinting,
            Aiming,
            Attacking,
            Blocking,
            Collecting
        }

        public MotionState motionState = MotionState.Moving;
        private float _moveHoldTime = -1.0f;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            //_equipment = GetComponent<Equipment>();
            //_aim = GetComponent<AimControl>();

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            CinemachineCameraFollow.SetActive(true);
            CinemachineCameraAim.SetActive(false);
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            JumpAndGravity();
            GroundedCheck();
            SetMotionState();
            EquipCheck();
            AimFire();
            Collect();
            Move();
            CameraRotation();
        }

        private void LateUpdate()
        {
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDCollect = Animator.StringToHash("Collect");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void SetMotionState()
        {
            if (_moveHoldTime < 0f)
            {
                if (_input.attack)
                {
                    motionState = MotionState.Attacking;
                    _moveHoldTime = AttackHoldTime;
                    _input.attack = false;
                    return;
                }
                else if (_input.fire)
                {
                    motionState = MotionState.Attacking;
                    _moveHoldTime = AttackHoldTime;
                    _input.fire = false;
                    return;
                }
                else if (_input.collect)
                {
                    motionState = MotionState.Collecting;
                    _moveHoldTime = CollectHoldTime;
                    _input.collect = false;
                    return;
                }
                else if (_input.aim > 0.5f && _canAim)
                {
                    motionState = MotionState.Aiming;
                    return;
                }
                else if (_input.block && _canBlock)
                {
                    motionState = MotionState.Blocking;
                    return;
                }
                else if (_input.sprint)
                {
                    motionState = MotionState.Sprinting;
                    return;
                }
                else
                {
                    motionState = MotionState.Moving;
                }
            }
            else
            {
                _moveHoldTime -= Time.deltaTime;
            }
        }

        //private Quaternion _delayedTransformRotation;
        private void CameraRotation()
        {
            if (_moveHoldTime > 0.0f)
            {
                return;
            }

            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                _cinemachineTargetYaw += _input.look.x * Time.deltaTime;
                _cinemachineTargetPitch += _input.look.y * Time.deltaTime;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            if (motionState == MotionState.Aiming)
            {
                transform.rotation = Quaternion.Euler(0.0f, _cinemachineTargetYaw, 0.0f);
                CinemachineCameraAimTarget.transform.rotation =
                    Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw + AimCameraAngleOffset, 0.0f);
            }
            else
            {
                CinemachineCameraTarget.transform.rotation =
                    Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
            }
        }

        private void SetStatesFromEquipAttributes()
        {
            //_canBlock = _equipment.GetAttribute("block");
            //_canAim = _equipment.GetAttribute("aim");
        }

        private void EquipCheck()
        {
            //if (_equipment == null) return;

            if (_input.westLoadout)
            {
                //_equipment.SetLoadout(Equipment.LoadoutZone.West);
                SetStatesFromEquipAttributes();
                _input.westLoadout = false;
            }

            if (_input.northLoadout)
            {
                //_equipment.SetLoadout(Equipment.LoadoutZone.North);
                SetStatesFromEquipAttributes();
                _input.northLoadout = false;
            }

            if (_input.eastLoadout)
            {
                //_equipment.SetLoadout(Equipment.LoadoutZone.East);
                SetStatesFromEquipAttributes();
                _input.eastLoadout = false;
            }

            if (_input.southLoadout)
            {
                //_equipment.SetLoadout(Equipment.LoadoutZone.South);
                SetStatesFromEquipAttributes();
                _input.southLoadout = false;
            }
        }

        public void SetAimComplete()
        {
            //jsv: Called From Animation
            _aimComplete = true;
        }

        private void AimFire()
        {
            if (_aimTimeoutDelta > 0.0f)
                _aimTimeoutDelta -= Time.deltaTime;
            else _aimTimeoutDelta = -1.0f;

            if (!_canAim)
            {
                if (_aimReset)
                {
                    CinemachineCameraFollow.SetActive(true);
                    CinemachineCameraAim.SetActive(false);
                    //aimControlState = AimControl.AimControlState.None;
                    _aimReset = false;
                    _aimStarted = false;
                    //if (_aim) _aim.SetState(aimControlState);
                }
                return;
            }
            else
            {
                _aimReset = true;

                if (_aimTimeoutDelta > 0.0f)
                {
                    _aimStarted = false;
                    //aimControlState = AimControl.AimControlState.Transitioning;
                    //if (_aim) _aim.SetState(aimControlState);
                    return;
                }

                if (_input.aim > 0.5f)
                {
                    if (!CinemachineCameraAim.activeInHierarchy)
                    {
                        CinemachineCameraFollow.SetActive(false);
                        CinemachineCameraAim.SetActive(true);
                    }
                    if (_input.fire && _aimComplete)
                    {
                        _aimTimeoutDelta = AimTimeout;
                        _input.fire = false;
                        // aimControlState = AimControl.AimControlState.Firing;
                        // if (_aim) _aim.SetState(aimControlState);
                    }
                    else
                    {
                        if (!_aimStarted)
                        {
                            _aimComplete = false;
                            _aimStarted = true;
                        }

                        // aimControlState = AimControl.AimControlState.Aiming;
                        // if (_aim) _aim.SetState(aimControlState);
                    }
                }
                else if (_input.aim == 0.0f)
                {
                    if (!CinemachineCameraFollow.activeInHierarchy)
                    {
                        CinemachineCameraFollow.SetActive(true);
                        CinemachineCameraAim.SetActive(false);
                    }
                    // aimControlState = AimControl.AimControlState.Equipped;
                    // if (_aim) _aim.SetState(aimControlState);
                }
            }
        }

        private void Move()
        {
            float targetSpeed = 0f;
            switch (motionState)
            {
                case (MotionState.None):
                case (MotionState.Attacking):
                case (MotionState.Collecting):
                    targetSpeed = 0f;
                    break;
                case (MotionState.Aiming):
                case (MotionState.Blocking):
                    targetSpeed = AimSpeed;
                    break;
                case (MotionState.Sprinting):
                    targetSpeed = SprintSpeed;
                    break;
                case (MotionState.Moving):
                    targetSpeed = MoveSpeed;
                    break;
                default: break;
            }
            targetSpeed *= _input.move == Vector2.zero ? 0.0f : 1.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero && _moveHoldTime < 0f)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f &&
                    (motionState == MotionState.Moving || motionState == MotionState.Sprinting))
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void Collect()
        {
            return;
            /*
            if (motionState == MotionState.Collecting)
            {
                if (!_animator.GetBool(_animIDCollect))
                {
                    var interactable = GetComponent<Interactor>().Interact();
                    if (interactable != null)
                    {
                        _animator.SetBool(_animIDCollect, true);
                    }
                    else
                    {
                        _moveHoldTime = 0f;
                        motionState = MotionState.Moving;
                    }
                }
                _moveHoldTime -= Time.deltaTime;
                if (_moveHoldTime < 0)
                {
                    _animator.SetBool(_animIDCollect, false);
                    _moveHoldTime = -1.0f;
                    motionState = MotionState.Moving;
                }
            }
            else
            {
                _animator.SetBool(_animIDCollect, false);
            }
            */
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
