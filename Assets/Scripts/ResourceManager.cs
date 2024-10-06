using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private Resource[] resourceArray;

    private void Start()
    {
        resourceArray = GetComponentsInChildren<Resource>();
    }

    private void Update()
    {
        
    }

    public void NewDay()
    {
        float chance = Random.value;

        for (int i = 0; i < resourceArray.Length; i++)
        {
            if (resourceArray[i].RespawnChance >= chance)
            {
                resourceArray[i].SetState(null, false);
            }
        }
    }
}
