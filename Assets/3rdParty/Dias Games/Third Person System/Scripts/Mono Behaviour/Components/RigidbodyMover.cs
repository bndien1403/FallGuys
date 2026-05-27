using UnityEngine;

namespace DiasGames.Components
{
    public class RigidbodyMover : MonoBehaviour, IMover, ICapsule
    {
        [Header("Player")]
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedCheckDistance = 0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Gravity")]
        [SerializeField] private float Gravity = -15.0f;

        // --- THÊM: BIẾN NHẬN DỮ LIỆU MẶT ĐẤT ---
        [HideInInspector] public float currentAccelerationMultiplier = 1f;
        [HideInInspector] public float currentGripMultiplier = 1f;
        // ---------------------------------------

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _initialCapsuleHeight = 2f;
        private float _initialCapsuleRadius = 0.28f;

        // variables for root motion
        private bool _useRootMotion = false;
        private Vector3 _rootMotionMultiplier = Vector3.one;
        private bool _useRotationRootMotion = false;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDMotionSpeed;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsule;
        private GameObject _mainCamera;

        private bool _hasAnimator;

        private void Awake()
        {
            _mainCamera = Camera.main.gameObject;
            _rigidbody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();

            _initialCapsuleHeight = _capsule.height;
            _initialCapsuleRadius = _capsule.radius;

            Physics.gravity = new Vector3(0, Gravity, 0);
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            AssignAnimationIDs();
        }

        private void FixedUpdate()
        {
            GroundedCheck();
            GravityControl();
        }

        private void OnAnimatorMove()
        {
            if (!_useRootMotion) return;

            Vector3 velocity = Vector3.Scale(_animator.deltaPosition / Time.deltaTime, _rootMotionMultiplier);
            if (_rigidbody.useGravity)
                velocity.y = _rigidbody.linearVelocity.y;

            _rigidbody.linearVelocity = velocity;
            transform.rotation *= _animator.deltaRotation;
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("Motion Speed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = transform.position + Vector3.up * GroundedRadius * 2;
            RaycastHit groundHit;
            Grounded = Physics.SphereCast(spherePosition, GroundedRadius, Vector3.down, out groundHit,
                GroundedCheckDistance + GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        public Collider GetGroundCollider()
        {
            if (!Grounded) return null;

            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedCheckDistance, transform.position.z);
            Collider[] grounds = Physics.OverlapSphere(spherePosition, _capsule.radius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (grounds.Length > 0)
                return grounds[0];

            return null;
        }

        public void Move(Vector2 moveInput, float targetSpeed, bool rotateCharacter = true)
        {
            Move(moveInput, targetSpeed, _mainCamera.transform.rotation, rotateCharacter);
        }

        public void Move(Vector2 moveInput, float targetSpeed, Quaternion cameraRotation, bool rotateCharacter = true)
        {
            if (moveInput == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_rigidbody.linearVelocity.x, 0.0f, _rigidbody.linearVelocity.z).magnitude;
            float speedOffset = 0.1f;
            
            // Giữ lại Analog để dáng chạy mượt mà, ta sẽ ép số 0 ở khâu Animator bên dưới
            float inputMagnitude = moveInput.magnitude; 
            if (inputMagnitude > 1) inputMagnitude = 1f;

            float actualSpeedChangeRate = SpeedChangeRate * currentAccelerationMultiplier;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * actualSpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            
            _animationBlend =targetSpeed * inputMagnitude;
            Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

            if (moveInput != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraRotation.eulerAngles.y;
                
                // ÉP ĐỘ BÁM (GRIP) ĐỂ BẺ LÁI VĂNG ĐUÔI
                float actualRotationSmoothTime = RotationSmoothTime / Mathf.Max(0.01f, currentGripMultiplier);
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, actualRotationSmoothTime);

                if (rotateCharacter && !_useRotationRootMotion)
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

           
            if (_hasAnimator)
            {
                if (currentAccelerationMultiplier < 0.1f)
                {
                   
                    float iceSpeed = (moveInput != Vector2.zero) ? targetSpeed : 0f;
                    float iceMotion = (moveInput != Vector2.zero) ? 1f : 0f;
                    
                    _animator.SetFloat(_animIDSpeed, iceSpeed);
                    _animator.SetFloat(_animIDMotionSpeed, iceMotion);
                }
                else
                {
                
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                    _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                }
            }

            // LẤY GÓC XOAY CỦA THÂN NHÂN VẬT ĐỂ TẠO LỰC VĂNG
            Vector3 targetDirection = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f) * Vector3.forward;

            if (!_useRootMotion)
            {
                Vector3 velocity = targetDirection.normalized * _speed;
                velocity.y = _rigidbody.linearVelocity.y;
                _rigidbody.linearVelocity = velocity;
            }
        }

        public void Move(Vector3 velocity)
        {
            if (_rigidbody.useGravity)
                velocity.y = _rigidbody.linearVelocity.y;

            _rigidbody.linearVelocity = velocity;
        }

        private void GravityControl()
        {
            if (_rigidbody.useGravity)
            {
                if (Grounded)
                {
                    if (_rigidbody.linearVelocity.y < 0.0f)
                    {
                        Vector3 velocity = _rigidbody.linearVelocity;
                        velocity.y = Mathf.Clamp(velocity.y, -2, 0); 
                        _rigidbody.linearVelocity = velocity;
                    }
                }
            }
        }

        public Quaternion GetRotationFromDirection(Vector3 direction)
        {
            float yaw = Mathf.Atan2(direction.x, direction.z);
            return Quaternion.Euler(0, yaw * Mathf.Rad2Deg, 0);
        }

        public void SetPosition(Vector3 newPosition)
        {
            _rigidbody.position = newPosition + _rigidbody.linearVelocity * Time.fixedDeltaTime;
        }

        public void DisableCollision() { _capsule.enabled = false; }
        public void EnableCollision() { _capsule.enabled = true; }

        public void SetCapsuleSize(float newHeight, float newRadius)
        {
            _capsule.height = newHeight;
            _capsule.center = new Vector3(0, newHeight * 0.5f, 0);
            if (newRadius > newHeight * 0.5f) newRadius = newHeight * 0.5f;
            _capsule.radius = newRadius;
        }

        public void ResetCapsuleSize() { SetCapsuleSize(_initialCapsuleHeight, _initialCapsuleRadius); }
        public void SetVelocity(Vector3 velocity) { _rigidbody.linearVelocity = velocity; }
        public Vector3 GetVelocity() { return _rigidbody.linearVelocity; }
        public float GetGravity() { return Gravity; }

        public void ApplyRootMotion(Vector3 multiplier, bool applyRotation = false)
        {
            _useRootMotion = true;
            _rootMotionMultiplier = multiplier;
            _useRotationRootMotion = applyRotation;
        }

        public void StopRootMotion()
        {
            _useRootMotion = false;
            _useRotationRootMotion = false;
        }

        public float GetCapsuleHeight() { return _capsule.height; }
        public float GetCapsuleRadius() { return _capsule.radius; }
        public void EnableGravity() { _rigidbody.useGravity = true; }
        public void DisableGravity() { _rigidbody.useGravity = false; }
        bool IMover.IsGrounded() { return Grounded; }

        public void StopMovement()
        {
            if (currentAccelerationMultiplier < 0.5f)
            {
                // NẾU ĐANG TRÊN BĂNG: Từ chối phanh gấp, để thân người tự lướt đi chậm dần
                Vector3 currentVel = _rigidbody.linearVelocity;
                currentVel.x = Mathf.Lerp(currentVel.x, 0, Time.deltaTime * SpeedChangeRate * currentAccelerationMultiplier);
                currentVel.z = Mathf.Lerp(currentVel.z, 0, Time.deltaTime * SpeedChangeRate * currentAccelerationMultiplier);
                _rigidbody.linearVelocity = currentVel;
            }
            else
            {
                // ĐẤT THƯỜNG: Khựng lại ngay lập tức
                _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
            }
            
            _speed = 0;
            _animator.SetFloat(_animIDSpeed, 0);
            _animator.SetFloat(_animIDMotionSpeed, 0);
        }

        public Vector3 GetRelativeInput(Vector2 input)
        {
            Vector3 relative = _mainCamera.transform.right * input.x +
                   Vector3.Scale(_mainCamera.transform.forward, new Vector3(1, 0, 1)) * input.y;
            return relative;
        }
    }
}