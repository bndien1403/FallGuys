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
            _animator.SetTrigger(shootAnimTrigger);
            
        if (muzzleFlash != null) 
            muzzleFlash.Play();

        if (_gunScaner == null) return;
        
        StunReceiver target = _gunScaner.GetTarget();
        Transform targetPoint = _gunScaner.GetTargetPoint();

        if (target != null && targetPoint != null && gunMuzzle != null)
        {
            Vector3 hitDirection = (targetPoint.position - gunMuzzle.position).normalized;
            target.ApplyStun(hitDirection, knockbackForce, stunDuration);
            Debug.Log($"<color=green>[Gun] Bắn trúng: {target.gameObject.name}</color>");
        }
    }
}