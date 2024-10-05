using UnityEngine;

using DG.Tweening;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType type;
    [SerializeField] private Vector2Int quantity;

    [Header("Elements")]
    [SerializeField] private GameObject model;

    [Header("Storage")]
    [SerializeField] private bool used;
    private Tween tween;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if(other.TryGetComponent(out Player player))
        {
            UseResource(true);
        }
    }

    public void UseResource(bool state)
    {
        tween?.Kill();
        tween = model.transform.DOScale(state ? 0.01f : 1f, 0.15f);

        used = state;
    }
}

public enum ResourceType
{
    grass, // To feed the creatures
    metal, // To protect the shelter
    wood,  // To build bridges to go further
    rocks, // To build staircase
}