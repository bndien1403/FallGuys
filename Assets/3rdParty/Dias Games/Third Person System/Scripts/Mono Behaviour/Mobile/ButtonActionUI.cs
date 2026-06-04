using System;
using UnityEngine;
using UnityEngine.UI; // BẮT BUỘC PHẢI CÓ ĐỂ CHỈNH ẢNH UI
using UnityEngine.EventSystems;
using DiasGames;

public class ActionButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private AbilityScheduler abilityScheduler;

    [Header("UI Icons")]

    [SerializeField] private Image buttonImage;
    
    [SerializeField] private Sprite jumpIcon;

    private bool _isPressed = false;
    
    private ItemBase _lastWeapon; 

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        if (itemSpawner == null) return;

        ItemBase currentWeapon = itemSpawner.GetCurrentBehaviour();
        if (currentWeapon != null)
        {
            currentWeapon.OnUse();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }

    private void Start()
    {
        UpdateIcon();
    }

    private void Update()
    {
        if (abilityScheduler == null || itemSpawner == null) return;

        ItemBase currentWeapon = itemSpawner.GetCurrentBehaviour();
        
        if (currentWeapon != _lastWeapon)
        {
            _lastWeapon = currentWeapon;
            UpdateIcon();
          
        }
        
        if (currentWeapon == null)
        {
            abilityScheduler.characterActions.jump = _isPressed ||Input.GetKey(KeyCode.Space);
            
        }
        else
        {
            abilityScheduler.characterActions.jump = false;
        }
    }

   
    private void UpdateIcon()
    {
        if (buttonImage == null) return;

        if (_lastWeapon == null)
        {
          
            buttonImage.sprite = jumpIcon;
        }
        else
        {
           
            if (ItemManager.Instance != null)
            {
                ItemDataSO currentItemData = ItemManager.Instance.GetCurrentItem();
                if (currentItemData != null && currentItemData.icon != null) 
                {
                    buttonImage.sprite = currentItemData.icon;
                }
            }
        }
    }
}