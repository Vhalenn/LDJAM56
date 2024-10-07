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
    [SerializeField] private float startYSunRot;
    [SerializeField] private float currentTime; public float CurrentTime => currentTime;
    [SerializeField] private float totalTime; public float TotalTime => totalTime;
    [SerializeField] private bool Night => currentTime >= 0.5f;
    [SerializeField] private int previousDay;
    [SerializeField] private bool previousNightState;

    private void Start()
    {
        previousDay = -1;
        totalTime = 0.15f;
        startYSunRot = sunLight.transform.eulerAngles.y;
    }

    private void Update()
    {
        totalTime += Time.deltaTime / dayLength;
        currentTime = Mathf.Repeat(totalTime, 1f);

        gameData.TotalTime = totalTime;
        gameData.CurrentTime = currentTime;
        gameData.Night = Night;

        if(Night != previousNightState)
        {
            previousNightState = Night;

            if(Night)
            {
                gameData.Game.NightStart();
            }
        }


        if (Mathf.Floor(totalTime) > previousDay)
        {
            // New day
            Debug.Log($"Start of day {totalTime}");
            previousDay = Mathf.FloorToInt(totalTime);

            gameData.Game.NewDay();
        }

        // Rotate sun
        Vector3 rot;
        rot.x = (Mathf.Repeat(currentTime, 0.5f) * 360.0f);
        rot.y = startYSunRot;
        rot.z = 0f;
        sunLight.transform.rotation = Quaternion.Euler(rot);
        sunLight.colorTemperature = kevlinCurve.Evaluate(currentTime);//night ? 13159 : 5000;
        sunLight.intensity = intensityCurve.Evaluate(currentTime);//night ? 1.0f : 1.5f;
        sunLight.shadowStrength = Night ? 0.25f : 1f;

        RenderSettings.ambientIntensity = ambientIntensityCurve.Evaluate(currentTime);
    }
}
