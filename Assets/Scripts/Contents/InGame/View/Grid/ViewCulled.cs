using System.Collections.Generic;
using UnityEngine;

public class ViewCulled : MonoBehaviour
{

}

public class ViewCulling
{
    private class CulledEntry
    {
        public Transform root;
        public Renderer[] renderers;
    }

    private readonly List<CulledEntry> entries = new List<CulledEntry>();

    public LayerMask ObstacleLayer;

    public void AddObstacleLayer(LayerMask layer) => ObstacleLayer |= layer;

    public void RegistCulledObject(ViewCulled viewCulled)
    {
        if (viewCulled == null) return;

        Renderer[] renderers = viewCulled.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;

        entries.Add(new CulledEntry
        {
            root = viewCulled.transform,
            renderers = renderers
        });

        foreach (Renderer r in renderers)
            r.enabled = false;
    }

    public void Cull(ViewInfo viewInfo)
    {
        if (entries.Count == 0) return;

        // 1. 일단 모두 비활성화
        foreach (CulledEntry entry in entries)
            foreach (Renderer r in entry.renderers)
                if (r != null) r.enabled = false;

        List<FovInfo> fovInfos = viewInfo.FovInfos;

        foreach (FovInfo fov in fovInfos)
        {
            float cosHalfAngle = Mathf.Cos(fov.angle * 0.5f * Mathf.Deg2Rad);
            float radiusSq = fov.radius * fov.radius;

            foreach (CulledEntry entry in entries)
            {
                if (entry.root == null) continue;
                if (entry.renderers[0] != null && entry.renderers[0].enabled) continue;

                Vector3 targetPos = entry.root.position;
                Vector3 toObj = targetPos - fov.origin;
                float distSq = toObj.sqrMagnitude;

                // [Step 1] 거리 체크
                if (distSq > radiusSq)
                    continue;

                // [Step 2] 각도 체크
                float dist = Mathf.Sqrt(distSq);
                Vector3 dirToObj = toObj / dist;
                float dot = Vector3.Dot(fov.forward, dirToObj);

                if (dot < cosHalfAngle)
                    continue;

                // [Step 3] 장애물 체크 (Raycast/Linecast)
                if (!Physics.Linecast(fov.origin, targetPos, ObstacleLayer))
                    foreach (Renderer r in entry.renderers)
                        if (r != null) r.enabled = true;
            }
        }
    }

    public void Clear() => entries.Clear();
}