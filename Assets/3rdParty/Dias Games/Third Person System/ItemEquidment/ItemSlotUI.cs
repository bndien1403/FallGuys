using UnityEngine;
using UnityEngine.UI ;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    //[SerializeField] private GameObject selectedBorder;
    [SerializeField] private int slotIndex;

    public void SetItem(ItemDataSO data, bool isSelected)
    {
        icon.sprite = data.icon;
        icon.enabled = true;
      //  selectedBorder.SetActive(isSelected);
    }

    public void Clear()
    {
        icon.enabled = false; 
      //  selectedBorder.SetActive(false);
    }
    public void OnClick() => ItemManager.Instance.SelectSlot(slotIndex);
}
