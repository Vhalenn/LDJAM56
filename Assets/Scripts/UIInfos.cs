using UnityEngine;
using TMPro;

public class UIInfos : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private TextMeshProUGUI creatureCount;

    private void Update()
    {
        creatureCount.text = gameDataScriptable.CreatureCount.ToString();
    }
}
