using UnityEngine;

public class GunController : ItemBase 
{
    [Header("Combat References")]
    [SerializeField] private Transform gunMuzzle;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Combat Stats")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float stunDuration = 3f; 
    [SerializeField] private string shootAnimTrigger = "Shoot";

    private GunScaner _gunScaner; 
    private Animator _animator;

    public override void OnEquid() 
    {
        base.OnEquid();
        if (_gunScaner == null) _gunScaner = FindObjectOfType<GunScaner>(true); 
        if (_animator == null) _animator = GetComponentInParent<Animator>();

    
        if (_gunScaner != null)
        {
            _gunScaner.gameObject.SetActive(true);
            if (_gunScaner.whiteCircleUI != null) _gunScaner.whiteCircleUI.gameObject.SetActive(true);
        }
    }

    public override void OnUnequid() 
    {
        base.OnUnequid();
        if (_gunScaner != null)
        {
            if (_gunScaner.whiteCircleUI != null) _gunScaner.whiteCircleUI.gameObject.SetActive(false);
            _gunScaner.gameObject.SetActive(false); 
        }
    }
    
    public override void OnUse() 
    {
        base.OnUse(); 
        Fire();
    }

    private void Fire()
    {
        if (_animator != null) 
         //   _animator.SetTrigger(shootAnimTrigger);
            
        if (muzzleFlash != null) 
            muzzleFlash.Play();

        if (_gunScaner == null) return;
        IDamage target = _gunScaner.GetTarget();
        Vector3 targetPos = _gunScaner.GetTargetPosition(); 
        
        if (target != null)
        {
            // apply knockback
            if (targetPos != Vector3.zero && gunMuzzle != null)
            {
                Vector3 hitDirection = (targetPos - gunMuzzle.position).normalized;
                target.TakeKnockback(hitDirection, knockbackForce);
            }

            // apply stun
            target.TakeStun(stunDuration);
            
            // debug log via mono cast
            if (target is MonoBehaviour monoTarget)
            {
                Debug.Log($"<color=green>[Gun] BỤP! Bắn trúng giữa ngực: {monoTarget.gameObject.name}</color>");
            }
        }
        else
        {
            Debug.Log("<color=yellow>[Gun] khong có địch trong tầm ngắm.</color>");
        }
    }
} 