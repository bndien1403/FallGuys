using System.Collections;
using UnityEngine;

public class StunReceiver : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    // Biến nội bộ để tránh việc bị bắn liên tục làm loạn Coroutine
    private Coroutine _stunCoroutine;

    // Súng sẽ gọi thẳng vào hàm này
    public void ApplyStun(Vector3 hitDirection, float knockbackForce, float stunDuration)
    {
       
        if (_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        
        _stunCoroutine = StartCoroutine(StunRoutine(hitDirection, knockbackForce, stunDuration));
    }

    private IEnumerator StunRoutine(Vector3 hitDirection, float knockbackForce, float duration)
    {
       
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && knockbackForce > 0)
        {
            rb.linearVelocity = Vector3.zero; // Xóa lực cũ
            rb.AddForce(hitDirection * knockbackForce, ForceMode.Impulse);
        }

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