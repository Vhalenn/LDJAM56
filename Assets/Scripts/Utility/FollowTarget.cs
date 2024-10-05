using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform parent;
    public bool Disable => disable;
    [SerializeField] private bool disable;
    [SerializeField] private bool roundPos;
    [SerializeField] private Vector2Int roundValues;

    // Storage
    private Transform tr;

    private void Start()
    {
        tr = transform;
    }

    private void Update()
    {
        if (parent && !disable) tr.position = parent.position;
    }


    public void SwitchMode()
    {
        SetMode(!disable);
    }

    public void SetMode(bool state)
    {
        disable = state;
    }
}
