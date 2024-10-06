using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInfos : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI creatureCount;
    [SerializeField] private Slider foodSlider;
    [SerializeField] private Slider woodSlider;

    private void Update()
    {
        if (creatureCount)
        {
            string text = string.Empty;
            int quantity = gameDataScriptable.Game.QuantityPlayerHas(CreatureType.Leaf);
            if (quantity > 0) text += $"<sprite name=crea_{CreatureType.Leaf}>{quantity}";

            quantity = gameDataScriptable.Game.QuantityPlayerHas(CreatureType.Branch);
            if (quantity > 0) text += $"     <sprite name=crea_{CreatureType.Branch}>{quantity}"; 
            
            quantity = gameDataScriptable.Game.QuantityPlayerHas(CreatureType.Rock);
            if (quantity > 0) text += $"     <sprite name=crea_{CreatureType.Rock}>{quantity}";

            creatureCount.text = text;
        }

        if (foodSlider) foodSlider.value = gameDataScriptable.FoodLevel;
        if (woodSlider) woodSlider.value = gameDataScriptable.CampLevel;
    }
}
