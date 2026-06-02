using System;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    [Header("Setting")] [SerializeField] private int maxSlots = 2;
    [Header("Referece")] [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private ItemBarUI itemBarUI;
    public UnityEvent<ItemDataSO> onItemAdded;
    public UnityEvent<int> onSlotSelected;
    public UnityEvent onInventoryFull;
    private List<ItemDataSO> inventory = new();
    private int currentIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public bool TryAddItem(ItemDataSO newItem)
    {
        if (inventory.Count < maxSlots)
        {
            AddItem(newItem);
            return true;
        }

        itemBarUI.ShowSwapUI(newItem, inventory);
        onInventoryFull?.Invoke();
        return false;
    }

    private void AddItem(ItemDataSO itemDataSo)
    {
        inventory.Add(itemDataSo);
        onItemAdded?.Invoke(itemDataSo);
        itemBarUI.Refresh(inventory, currentIndex);
       // if (currentIndex == -1) SelectSlot(0);
    }

    public void SwapItem(int slotIndex, ItemDataSO newItem)
    {
        if (slotIndex >= 0 || slotIndex < inventory.Count)
        {
            inventory[slotIndex] = newItem;
            if(currentIndex == slotIndex) 
            {
                itemSpawner.UnEquid();
                currentIndex = -1; 
            }
            itemBarUI.Refresh(inventory, currentIndex);
            itemBarUI.HideSwapUI();
        }

    }

    public void SelectSlot(int index)
    {
        if (index >= 0 && index < inventory.Count) 
        {
            
            if (currentIndex == index)
            {
                currentIndex = -1;
                itemSpawner.UnEquid(); 
                onSlotSelected?.Invoke(-1); 
                itemBarUI.Refresh(inventory, currentIndex); 
                return;
            }
            currentIndex = index;
            itemSpawner.Equid(inventory[currentIndex]);
            onSlotSelected?.Invoke(index);
            itemBarUI.Refresh(inventory, currentIndex);
        }
    }

    public void  RemoveCurrentitem()
    {
        if (currentIndex < 0 || currentIndex >= inventory.Count) return;
        inventory.RemoveAt(currentIndex);
        currentIndex = Mathf.Clamp(currentIndex, 0, inventory.Count - 1);
        if (inventory.Count == 0)
        {
            currentIndex = -1;
            itemSpawner.UnEquid();
        }
        else SelectSlot(currentIndex);

        itemBarUI.Refresh(inventory, currentIndex);

    }
    public ItemDataSO GetCurrentItem() => (currentIndex>=0 && currentIndex < inventory.Count) ? inventory[currentIndex] : null;
    public List<ItemDataSO> GetInventory() => inventory;
    public int GetCurrentIndex() => currentIndex;
    public bool IsFull() => inventory.Count >= maxSlots;
}
