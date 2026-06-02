using DiasGames.Abilities;
using DiasGames.Components;
using UnityEngine;

public class PlayerAimGun : AbstractAbility
{
    [Header("Animation")]
    [SerializeField] protected string animName = "AimGun";

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;

    protected IMover _mover;
    private GameObject _camera = null;

    protected override void Start()
    {
        base.Start(); 
    }

    protected virtual void Awake()
    {
        _mover = GetComponent<IMover>();
        if (Camera.main != null) _camera = Camera.main.gameObject;
    }
    
    public override bool ReadyToRun()
    {
        if (ItemManager.Instance == null) return false;

        ItemDataSO currentItem = ItemManager.Instance.GetCurrentItem();
        
        // CHỈ SẴN SÀNG KHI ĐANG CẦM SÚNG
        return currentItem != null && currentItem.itemType == ItemType.Gun;
    }

    public override void OnStartAbility()
    {
        if (_animator != null)
            _animator.CrossFadeInFixedTime(animName, 0.1f);
    }

    public override void OnStopAbility()
    {
        base.OnStopAbility();         
        
        if (_animator != null)
            _animator.CrossFadeInFixedTime("Grounded", 0.1f);
    }
    
    public override void UpdateAbility()
    {
        ItemDataSO currentItem = ItemManager.Instance.GetCurrentItem();
        
      
        if (currentItem == null || currentItem.itemType != ItemType.Gun)
        {
            StopAbility();
            return;
        }

        HandleMovement();
    }

    void HandleMovement()
    {
        _mover.Move(_action.move, walkSpeed, false);
        
        if (_camera != null)
            transform.rotation = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0);
    }
}