using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDataScriptable", menuName = "Scriptable Objects/ResourceDataScriptable")]
public class ResourceDataScriptable : ScriptableObject
{
    [SerializeField] private ResourceType type; public ResourceType Type => type;
    [SerializeField] private Vector2Int quantity; public Vector2Int Quantity => quantity;
    public int GetRandomQuantity()
    {
        return Random.Range(Quantity.x, Quantity.y);
    }
}
