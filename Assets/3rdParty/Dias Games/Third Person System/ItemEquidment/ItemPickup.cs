using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemDataSO itemData;
    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private Collider col;
    [SerializeField] private MeshRenderer[] meshes;

    public void SetItem(ItemDataSO data) => itemData = data;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        bool added = ItemManager.Instance.TryAddItem(itemData);
        if (added) StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        SetVisible(false);
        yield return new WaitForSeconds(respawnTime);
        SetVisible(true);
    }

    void SetVisible(bool value)
    {
        foreach (var m in meshes) if (m != null) m.enabled = value;
        if (col != null) col.enabled = value;
    }
}