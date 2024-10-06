using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDataScriptable", menuName = "Scriptable Objects/ResourceDataScriptable")]
public class ResourceDataScriptable : ScriptableObject
{
    [SerializeField] private ResourceType type; public ResourceType Type => type;
    [SerializeField] private Vector2Int quantity; public Vector2Int Quantity => quantity;
    public int GetRandomQuantity() => Random.Range(Quantity.x, Quantity.y);
    [SerializeField] private Vector2 respawnChance; public Vector2 RespawnChance => respawnChance;
    public float GetRespawnChance() => Random.Range(respawnChance.x, respawnChance.y);
}
