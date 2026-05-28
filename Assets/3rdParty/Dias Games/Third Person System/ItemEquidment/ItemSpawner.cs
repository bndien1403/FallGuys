using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
  [SerializeField] private Transform playerRoot;
  public GameObject currentItemObj;
  private ItemBase currentItemBehaviour;

  public void Equid(ItemDataSO data)
  {
    UnEquid();
    Transform parent = FindBone(playerRoot, data.parentBoneName);
    if (parent == null)
    {
      Debug.LogError("khong tim thay bone" + data.parentBoneName);
    }
    currentItemObj = Instantiate(data.prefab, parent);
    currentItemObj.transform.localPosition = data.positionOffset;
    currentItemObj.transform.localRotation = Quaternion.Euler(data.rotationOffset);
    SetWorldScale(currentItemObj.transform, data.worldScale);

    currentItemBehaviour = currentItemObj.GetComponentInChildren<ItemBase>();
    currentItemBehaviour?.OnEquid();

  }


  
  public void UnEquid()
  {
    if (currentItemObj == null) return;
    currentItemBehaviour?.OnUnequid();
    Destroy(currentItemObj);
    currentItemObj = null;
    currentItemBehaviour = null;
  }

  public ItemBase GetCurrentBehaviour() => currentItemBehaviour;
  private Transform FindBone(Transform root, string boneName)
  {
    if (root.name == boneName) return root;
    foreach (Transform child in root)
    {
      var result = FindBone(child, boneName);
      if(result != null) return result;
    }
    return null;
  }
  private void SetWorldScale(Transform target , Vector3 worldScale)
  {
    if (target == null)
    {
      target.localScale = worldScale;
      return;
    }
    var p = target.parent.lossyScale;
    target.localScale = new Vector3(worldScale.x / p.x, worldScale.y / p.y, worldScale.z / p.z);
  }

}
