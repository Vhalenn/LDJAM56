using UnityEngine;
using UnityEngine.UI;

public class UIScaleAnim : MonoBehaviour
{
    [SerializeField] private float parentOffset;
    [SerializeField] private AnimationCurve animCurve;

    private void Update()
    {
        transform.eulerAngles = Vector3.zero;

        parentOffset = transform.parent.position.y - transform.position.y;

        transform.localScale = Vector3.one * animCurve.Evaluate(parentOffset);
    }
}
