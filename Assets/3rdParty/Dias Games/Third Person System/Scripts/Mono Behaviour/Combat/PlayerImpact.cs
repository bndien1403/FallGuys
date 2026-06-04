using System;
using System.Collections;
using UnityEngine;

public class PlayerImpact : MonoBehaviour ,IDamage
{
    [SerializeField] private Animator animator;
    private Rigidbody rb;
    private Coroutine impactRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    public void Damage(int damagePoints)
    {
        throw new System.NotImplementedException();
    }

    public void TakeStun(float duration)
    {
       if(impactRoutine != null) StopCoroutine(impactRoutine);
       impactRoutine = StartCoroutine(StunRoutine(duration));
      
    }
     IEnumerator StunRoutine(float duration)
    {
        if (animator) animator.SetBool("IsStunned", true);
        
        yield return new WaitForSeconds(duration);

        if (animator) animator.SetBool("IsStunned", false);
        impactRoutine = null;
    }

    public void TakeKnockback(Vector3 direction, float force)
    {
       if(impactRoutine != null) StopCoroutine(impactRoutine);
       impactRoutine = StartCoroutine(KnockbackRoutine(direction, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        if (animator) animator.SetTrigger("Hit");

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * force, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1f); // thời gian văng
        impactRoutine = null;
    }
}
