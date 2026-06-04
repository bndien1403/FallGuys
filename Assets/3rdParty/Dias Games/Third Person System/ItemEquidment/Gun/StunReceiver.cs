using System.Collections;
using UnityEngine;

public class StunReceiver : MonoBehaviour, IDamage
{
    [SerializeField] private Animator _animator;
    
    private Coroutine _stunCoroutine;

    // implement IDamage
    public void Damage(int damagePoints)
    {
        // xử lý sát thương nếu cần
    }

    // implement IDamage
    public void TakeKnockback(Vector3 direction, float force)
    {
        // apply physics force
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && force > 0)
        {
            rb.linearVelocity = Vector3.zero; 
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    // implement IDamage
    public void TakeStun(float duration)
    {
        // apply stun state
        if (_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        _stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        if (_animator != null) 
            _animator.SetBool("IsStunned", true);

        Debug.Log($"<color=cyan> {gameObject.name} ĐANG BỊ CHOÁNG TRONG {duration} GIÂY!</color>");
     
        yield return new WaitForSeconds(duration);

        if (_animator != null) 
            _animator.SetBool("IsStunned", false);

        Debug.Log($"<color=yellow> {gameObject.name} ĐÃ TỈNH LẠI!</color>");
        _stunCoroutine = null;
    }
}