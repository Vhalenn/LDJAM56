using UnityEngine;
using UnityEngine.UI;

public class UIClock : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameData;
    [SerializeField] private RectTransform aiguilleCenter;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (!gameData || !aiguilleCenter) return;

        float rot = gameData.CurrentTime * -360.0f;
        aiguilleCenter.rotation = Quaternion.Euler(0, 0, rot);
    }
}
