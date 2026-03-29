using System.Collections.Generic;
using UnityEngine;

public class ViewCulling
{
    private List<GameObject> culledObjects = new List<GameObject>();

    // 장애물로 판정할 레이어 (인스펙터나 외부에서 설정 권장)
    public LayerMask ObstacleLayer;

    public void AddObstacleLayer(LayerMask layer) => ObstacleLayer |= layer;

    public void RegistCulledObject(GameObject obj)
    {
        if (obj != null && !culledObjects.Contains(obj))
        {
            culledObjects.Add(obj);
            obj.SetActive(false);
        }
    }

    public void Cull(ViewInfo viewInfo)
    {
        if (culledObjects.Count == 0) return;

        // 1. 일단 모두 비활성화
        for (int i = 0; i < culledObjects.Count; i++)
        {
            if (culledObjects[i] != null)
                culledObjects[i].SetActive(false);
        }

        List<FovInfo> fovInfos = viewInfo.FovInfos;

        foreach (FovInfo fov in fovInfos)
        {
            float cosHalfAngle = Mathf.Cos(fov.angle * 0.5f * Mathf.Deg2Rad);
            float radiusSq = fov.radius * fov.radius;

            foreach (GameObject obj in culledObjects)
            {
                if (obj == null || obj.activeSelf) continue;

                Vector3 targetPos = obj.transform.position;
                Vector3 toObj = targetPos - fov.origin;
                float distSq = toObj.sqrMagnitude;

                // [Step 1] 거리 체크
                if (distSq > radiusSq)
                    continue;

                // [Step 2] 각도 체크
                float dist = Mathf.Sqrt(distSq);
                Vector3 dirToObj = toObj / dist; // normalized 대용
                float dot = Vector3.Dot(fov.forward, dirToObj);

                if (dot < cosHalfAngle)
                    continue;

                // [Step 3] 장애물 체크 (Raycast/Linecast)
                // 플레이어 눈 높이와 오브젝트 중심 사이에 장애물이 있는지 확인
                // 플레이어 위치(fov.origin)에서 타겟 위치(targetPos)까지 쏩니다.
                if (!Physics.Linecast(fov.origin, targetPos, ObstacleLayer))
                    obj.SetActive(true);
            }
        }
    }

    public void Clear() => culledObjects.Clear();
}