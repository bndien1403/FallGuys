using System.Collections;
using UnityEngine;
using DiasGames.Components;
using DiasGames;
using DiasGames.Controller; // Import thư viện chứa TPSPlayerController

public class PlayerImpactReceiver : MonoBehaviour, IImpactReciver
{
    private IMover _mover;
    private Rigidbody _rb;
    private Animator _animator;
    private AbilityScheduler _scheduler;
    private CSPlayerController _controller; // Biến khóa điều khiển
    private Coroutine _stunCoroutine;

    private void Awake()
    {
        _mover = GetComponent<IMover>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _scheduler = GetComponent<AbilityScheduler>();
        _controller = GetComponent<CSPlayerController>(); 
    }

    public void TakeKnockback(Vector3 direction, float force)
    {
        if (_controller != null) _controller.enabled = false;
        if (_scheduler != null) 
        {
            _scheduler.characterActions.move = Vector2.zero; 
            _scheduler.enabled = false;
        }

        if (_mover != null) _mover.StopMovement();
        if (_animator != null) _animator.CrossFade("FlyingBackDeath", 0.1f);

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero; 
            direction.y = 0;
            Vector3 pushDir = direction.normalized;
            pushDir.y = 0.25f; 
            
            _rb.AddForce(pushDir * force, ForceMode.VelocityChange);
        }
    }

    public void TakeStun(float duration)
    {
        if (_stunCoroutine != null) StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        yield return new WaitForSeconds(0.6f); 
        
        if (_scheduler != null) _scheduler.enabled = true;

        if (_animator != null) _animator.CrossFade("Stunned", 0.1f);

        float remainingStun = Mathf.Max(0.5f, duration - 0.6f);
        yield return new WaitForSeconds(remainingStun);

        if (_animator != null) _animator.CrossFade("GettingUp", 0.1f);
        
        yield return new WaitForSeconds(1.5f); 

        if (_mover != null) _mover.SetVelocity(Vector3.zero);
        
      
        if (_controller != null) _controller.enabled = true;
        
        _stunCoroutine = null;
    }
}