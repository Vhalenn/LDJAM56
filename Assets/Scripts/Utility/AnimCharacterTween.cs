using UnityEngine;

public class AnimCharacterTween : MonoBehaviour
{

    [SerializeField] private float velocityInfluenceOnSpeed = 0.25f;
    [SerializeField] private AnimationCurve rotTiltCurve;
    [SerializeField] private float velocityInfluenceOnRotAmplitude = 0.1f;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float velocityInfluenceOnJumpAmplitude = 0.1f;

    [Header("Storage")]
    [SerializeField] private float velMagnitude; 
    public float VelMagnitude { get => velMagnitude; set => velMagnitude = value; }
    [SerializeField] private float time;

    private void Update()
    {
        time += Time.deltaTime * (1.5f + VelMagnitude * velocityInfluenceOnSpeed);
        transform.localRotation = Quaternion.Euler(0f, 0f, rotTiltCurve.Evaluate(time) * (1f + VelMagnitude * velocityInfluenceOnRotAmplitude));

        transform.localPosition = Vector3.up * jumpCurve.Evaluate(time) * (0.1f + VelMagnitude * velocityInfluenceOnJumpAmplitude);
    }
}
