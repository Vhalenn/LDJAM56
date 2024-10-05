using UnityEngine;

public class BaseCamp : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;

    [Header("Var")]
    [SerializeField] private int campIndex = 0; public int CampIndex => campIndex;
    [SerializeField] private int maxLifePoints = 1000; public int MaxLifePoints => maxLifePoints;

    [Header("Storage")]
    [SerializeField] private int lifePoints = 0; public int LifePoints => lifePoints;


    private void Start()
    {
        if(campIndex == 0) gameDataScriptable.Camp = this;

        lifePoints = maxLifePoints;
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player)) // If this is the player
        {
            gameDataScriptable.Camp = this;
        }
    }
}
