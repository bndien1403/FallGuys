using UnityEngine;
using DiasGames.Components;

namespace DiasGames.Abilities
{
    public enum MovementStyle
    {
        HoldToWalk, HoldToRun, DoNothing
    }

    public class Locomotion : AbstractAbility
    {
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float sprintSpeed = 5.3f;
        [Tooltip("Determine how to use extra key button to handle movement. If shift is hold, tells system if it should walk, run, or do nothing")]
        [SerializeField] private MovementStyle movementByKey = MovementStyle.HoldToWalk;
        [SerializeField] private string groundedAnimBlendState = "Grounded";

        [Header("Surface Detections")] 
        [SerializeField] private SurfaceData defaultSurface;
        [SerializeField] private Transform groundCheckpoint; // check ground
        private SurfaceData currentSurface;
        
        
        
        private IMover _mover = null;
        private int _animIDSpeed;

        private void Awake()
        {
            _mover = GetComponent<IMover>();

            _animIDSpeed = Animator.StringToHash("Speed");

            currentSurface = defaultSurface;
        }

        public override bool ReadyToRun()
        {
            return _mover.IsGrounded();
        }

        public override void OnStartAbility()
        {
            SetAnimationState(groundedAnimBlendState, 0.25f);

            if(_action.move.magnitude < 0.1f)
            {
                // reset movement parameters
                _animator.SetFloat(_animIDSpeed, 0, 0, Time.deltaTime);
            }
        }

        public override void UpdateAbility()
        {
            CheckGroundSurface();
            float targetSpeed = 0;
            switch (movementByKey)
            {
                case MovementStyle.HoldToWalk:
                    targetSpeed = _action.walk ? walkSpeed : sprintSpeed;
                    break;
                case MovementStyle.HoldToRun:
                    targetSpeed = _action.walk ? sprintSpeed : walkSpeed;
                    break;
                case MovementStyle.DoNothing:
                    targetSpeed = sprintSpeed;
                    break;
            }

            float finalSpeed = targetSpeed * currentSurface.maxSpeed;
            Debug.Log($"[Locomotion Gốc] Mặt đất: {currentSurface.name} | Accel: {currentSurface.accelerationMultiplier} | Grip: {currentSurface.gripMultiplier}");
            if (_mover is RigidbodyMover concreteMover)
            {
                concreteMover.currentAccelerationMultiplier = currentSurface.accelerationMultiplier;
                concreteMover.currentGripMultiplier = currentSurface.gripMultiplier;
            }
            else
            {
                // Nếu ép kiểu xịt, nó sẽ văng dòng lỗi đỏ này ra cùng với tên thật của script di chuyển!
                Debug.LogError($"[LỖI CHÍ MẠNG] Script di chuyển không phải là 'Mover'! Tên class thật của nó là: {_mover.GetType().Name}");
            }
            _mover.Move(_action.move, finalSpeed);
        }

        private void CheckGroundSurface()
        {
            if (groundCheckpoint == null) return;
    
            // Vẽ tia laser ĐỎ trong cửa sổ Scene để kiểm tra độ dài bằng mắt thường
            Debug.DrawRay(groundCheckpoint.position, Vector3.down * 1.5f, Color.red);

            RaycastHit hit;
            // Đổi 0.5f thành 1.5f để tia laser bắn sâu xuống tận 1.5 mét
            if (Physics.Raycast(groundCheckpoint.position, Vector3.down, out hit, 1.5f))
            {
                // LOG 1: Xem chính xác tia đang đâm trúng Object tên là gì
                Debug.Log("[Raycast Trúng] Tên Object: " + hit.collider.gameObject.name);
        
                // GIẢI PHÁP 1: Đổi sang GetComponentInParent để quét ngược lên cha nếu con không có script
                SurfaceModifier floor = hit.collider.GetComponentInParent<SurfaceModifier>();
        
                if (floor != null)
                {
                    currentSurface = floor.surfaceData;
                    Debug.Log("[Thành công] Tìm thấy Script! Đất hiện tại: " + currentSurface.name);
                }
                else
                {
                    currentSurface = defaultSurface;
                    // LOG 2: Chạm trúng sàn rồi nhưng script bị null
                    Debug.Log("[Thất bại] Đạp trúng sàn nhưng sàn này KHÔNG CÓ script SurfaceModifier!");
                }
            }
            else
            {
                currentSurface = defaultSurface;
                // LOG 3: Tia không chạm được vào cái gì cả
                Debug.Log("[Hụt] Tia Raycast quá ngắn hoặc gót chân đặt quá cao, ĐÉO CHẠM ĐƯỢC XUỐNG SÀN!");
            }
        }
    }
}