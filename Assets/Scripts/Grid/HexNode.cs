using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class HexNode : MonoBehaviour
{
    [Range(0, 5)]
    public int dir;
    public bool randomizeDir = false;
    public bool lockY = false;

    public Hex hex
    {
        get
        {
            return transform.position.ToHex();
        }
    }

    public Hex localHex
    {
        get
        {
            return transform.localPosition.ToHex();
        }
    }

    public void ApplyTransform()
    {
        if (randomizeDir)
        {
            Hex hex = this.hex;
            int i = hex.q * 100 + hex.r;
            dir = ((i % 6) + 6) % 6;
        }
        float y = lockY ? 0f : transform.localPosition.y;
        Vector3 newPos = this.localHex.ToWorld(y);
        transform.localPosition = newPos;
        transform.localRotation = Quaternion.Euler(0, -60f * dir, 0);
    }

#if UNITY_EDITOR
    protected virtual void Update()
    {
        if (!Application.isPlaying)
        {
            ApplyTransform();
            // Hack to never re-apply dir to instances
            this.dir += 1;
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            this.dir = (dir - 1) % 6;
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.Label(transform.position, hex.ToString());
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        //UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, Hex.RADIUS);

        for (int i = 0; i < Hex.CORNERS.Length; i++)
        {
            int indexB = i + 1;
            if (indexB == Hex.CORNERS.Length) indexB = 0;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos + Hex.CORNERS[i], pos + Hex.CORNERS[indexB]);
        }
    }
#endif

}
