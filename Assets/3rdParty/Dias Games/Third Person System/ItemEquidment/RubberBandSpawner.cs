using System.Collections.Generic;
using UnityEngine;

public class RubberBandSpawner : MonoBehaviour
{
    [SerializeField] private List<ItemDataSO> allItems;
    [SerializeField] private int totalPlayers = 4;
    [SerializeField] private AnimationCurve boostByRank;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject pickupPrefab;

    void Start()
    {
        foreach (var point in spawnPoints)
            SpawnItemAt(point);
    }

    public ItemDataSO RollItem(int playerRank)
    {
        float boost = boostByRank.Evaluate((float)playerRank / totalPlayers);

        float totalWeight = 0f;
        var weights = new List<float>();

        foreach (var item in allItems)
        {
            float multiplier = item.itemCategory switch
            {
                ItemCategory.Attack  => 1f,
                ItemCategory.Defense => 1f + boost * 2f,
                ItemCategory.Trap    => 1f + boost * 1.5f,
                _                    => 1f
            };

            float w = item.baseSpawnWeight * multiplier;
            weights.Add(w);
            totalWeight += w;
        }

        float rand = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < allItems.Count; i++)
        {
            current += weights[i];
            if (rand <= current) return allItems[i];
        }

        return allItems[^1];
    }

    public void SpawnItemAt(Transform point)
    {
        var item = RollItem(totalPlayers / 2);
        var pickup = Instantiate(pickupPrefab, point.position, point.rotation);
        pickup.GetComponent<ItemPickup>()?.SetItem(item);
    }
}