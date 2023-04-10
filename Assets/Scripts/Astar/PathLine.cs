using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLine : MonoBehaviour
{
    LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void DrawPath(GridMap map, List<Vector2Int> path)
    {
        if(map != null && path != null && gameObject.activeSelf)
        {
            lineRenderer.positionCount = path.Count;

            int index = 0;
            foreach(var node in path)
            {
                Vector2 worldPos = map.GriodToWorld(node);
                Vector3 localPos = (Vector3)worldPos - lineRenderer.transform.position;
                lineRenderer.SetPosition(index, localPos);
                index++;
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
