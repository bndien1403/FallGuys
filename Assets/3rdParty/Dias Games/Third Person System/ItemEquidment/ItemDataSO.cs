using UnityEngine;

public enum ItemType
{
    Gun ,
    Shield ,
    Miner,
    PullMine
 
}
public enum ItemCategory{
Attack ,
Defense ,
Trap  //
}
[CreateAssetMenu(fileName ="ItemData" , menuName = "Item System/Item Data")]
public class ItemDataSO : ScriptableObject
{
    [Header("Infor")] public string itemName;
    public ItemType itemType;
    public ItemCategory itemCategory;
    public Sprite icon;
    public GameObject prefab;
    [Header("Spawn")] public string parentBoneName;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 worldScale = Vector3.one;
    [Header("Stats")] public int maxDurability = 10;

    [Header("Rubber Band")] [Range(0f, 1f)]
    public float baseSpawnWeight = 1f;

}
