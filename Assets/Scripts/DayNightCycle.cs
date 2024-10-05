using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameData;
    [SerializeField] private Light sunLight;

    [Header("Variable")]
    [SerializeField] private float dayLength;
    [SerializeField] private AnimationCurve kevlinCurve;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private AnimationCurve ambientIntensityCurve;

    [Header("Storage")]
    [SerializeField] private float currentTime; public float CurrentTime => currentTime;
    [SerializeField] private float totalTime; public float TotalTime => totalTime;
    [SerializeField] private bool night;

    private void Start()
    {
        totalTime = 0.15f;
    }

    private void Update()
    {
        totalTime += Time.deltaTime / dayLength;
        currentTime = Mathf.Repeat(totalTime, 1f);
        night = currentTime >= 0.5f;

        gameData.TotalTime = totalTime;
        gameData.CurrentTime = currentTime;
        gameData.Night = night;

        // Rotate sun
        Vector3 rot;
        rot.x = (Mathf.Repeat(currentTime, 0.5f) * 360.0f);
        rot.y = 147f;
        rot.z = 0f;
        sunLight.transform.rotation = Quaternion.Euler(rot);
        sunLight.colorTemperature = kevlinCurve.Evaluate(currentTime);//night ? 13159 : 5000;
        sunLight.intensity = intensityCurve.Evaluate(currentTime);//night ? 1.0f : 1.5f;
        sunLight.shadowStrength = night ? 0.25f : 1f;

        RenderSettings.ambientIntensity = ambientIntensityCurve.Evaluate(currentTime);
    }
}
