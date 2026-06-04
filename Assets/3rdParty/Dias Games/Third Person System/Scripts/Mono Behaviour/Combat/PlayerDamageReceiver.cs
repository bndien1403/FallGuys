using UnityEngine;
using System.Collections;
using DiasGames;
using DiasGames.Components;

public class PlayerDamageReceiver : MonoBehaviour, IDamage
{
    private IMover _mover;
    private AbilityScheduler _scheduler;
    private Animator _animator;
    private Coroutine _stunCoroutine;

    private void Awake()
    {
        _mover = GetComponent<IMover>();
        _scheduler = GetComponent<AbilityScheduler>();
        _animator = GetComponent<Animator>();
    }

    // ==========================================
    // KHU VỰC DÀNH CHO BÁC TỰ TEST
    // ==========================================
    private void Update()
    {
        // Bấm phím O để tự mô phỏng việc bị một thằng Player khác nã đạn vào người
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Bị văng về phía sau (ngược hướng nhân vật đang nhìn) với lực 5f
            TakeKnockback(-transform.forward, 5f);
            
            // Bị choáng 3 giây
            TakeStun(3f);
        }
    }

    // ==========================================
    // LOGIC XỬ LÝ CHUNG CHO CẢ GAME ONLINE
    // ==========================================
    public void Damage(int damagePoints)
    {
        // Code trừ máu sau này viết ở đây
    }

    public void TakeKnockback(Vector3 direction, float force)
    {
        if (_mover != null)
        {
            Vector3 velocity = direction * force;
            // Tính toán lực hất tung lên trời dựa theo trọng lực của framework
            velocity.y = Mathf.Sqrt(2f * -2f * _mover.GetGravity()); 
            _mover.SetVelocity(velocity);
        }
    }

    public void TakeStun(float duration)
    {
        if (_stunCoroutine != null) StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        // Khóa bộ não điều khiển của Player lại (không cho di chuyển/nhảy)
        if (_scheduler != null) _scheduler.enabled = false;
        
        // Phanh gấp, dừng mọi lực di chuyển
        if (_mover != null) _mover.SetVelocity(Vector3.zero);

        // Bật Anim choáng
        if (_animator != null) _animator.SetBool("IsStunned", true);

        Debug.Log($"<color=red>[Test] PLAYER BỊ BẮN TRÚNG! CHOÁNG {duration} GIÂY!</color>");
     
        yield return new WaitForSeconds(duration);

        // Hết thời gian thì tắt Anim
        if (_animator != null) _animator.SetBool("IsStunned", false);
        
        // Mở khóa lại bộ não cho Player chơi tiếp
        if (_scheduler != null) _scheduler.enabled = true;

        Debug.Log("<color=green>[Test] PLAYER ĐÃ TỈNH LẠI!</color>");
        _stunCoroutine = null;
    }
} 