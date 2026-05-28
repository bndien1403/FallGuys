using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemBarUI : MonoBehaviour
{
    [SerializeField] private List<ItemSlotUI> slots;
    [Header("Swap UI")] [SerializeField] private GameObject swapPanel;
    [SerializeField] private ItemSlotUI swapSlot0;
    [SerializeField] private ItemSlotUI swapSlot1;
    [SerializeField] private ItemSlotUI newItemPreview;
    private ItemDataSO pendingNewItem;

    public void Refresh(List<ItemDataSO> inventory, int selectedIndex)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if(i < inventory.Count)
                slots[i].SetItem(inventory[i],i ==selectedIndex);
            else
                slots[i].Clear();
        }
        
    }

    public void ShowSwapUI(ItemDataSO newItem, List<ItemDataSO> currentInventory)
    {
        pendingNewItem = newItem;
        swapPanel.SetActive(true);
        newItemPreview.SetItem(newItem , false);
        swapSlot0.SetItem(currentInventory[0] , false);
        swapSlot1.SetItem(currentInventory[1] , false);
    }

    public void HideSwapUI()
    {
        swapPanel.SetActive(false);
        pendingNewItem = null;
    }
    public void OnConfirmSwapSlot0() => ItemManager.Instance.SwapItem(0 , pendingNewItem);
    public void OnConfirmSwapSlot1() => ItemManager.Instance.SwapItem(1 , pendingNewItem);
    public void OnCancelSwap() => HideSwapUI();
}
