using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static int GetDictionaryQuantity(Dictionary<ResourceType, int> resourcesDico)
    {
        int quantity = 0;
        if (resourcesDico == null || resourcesDico.Count == 0) return quantity;

        foreach (KeyValuePair<ResourceType, int> pair in resourcesDico)
        {
            quantity += pair.Value;
        }

        return quantity;
    }

    public static string GetResourceAmountText(Dictionary<ResourceType, int> resourcesDico, int max)
    {
        int quantity = GetDictionaryQuantity(resourcesDico);
        string text = string.Empty;
        if (quantity != 0)
        {
            foreach (KeyValuePair<ResourceType, int> pair in resourcesDico)
            {
                text += $"\n<sprite name={pair.Key}> {pair.Value}";
            }

            if(max > 0) text += $"\n<size=66%>{quantity}/{max}</size>";
        }

        return text;
    }
}
