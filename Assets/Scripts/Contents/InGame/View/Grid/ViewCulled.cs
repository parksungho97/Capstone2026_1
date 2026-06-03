using System.Collections.Generic;
using UnityEngine;

public class ViewCulled : MonoBehaviour
{
    // 등록할 때 매번 검색하지 않도록 미리 캐싱
    public Renderer[] CachedRenderers { get; private set; }

    private void Awake()
    {
        CachedRenderers = GetComponentsInChildren<Renderer>(true);
    }
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

        // 캐싱된 렌더러 배열을 바로 가져와 성능 최적화
        Renderer[] renderers = viewCulled.CachedRenderers;
        if (renderers == null || renderers.Length == 0) return;

        entries.Add(new CulledEntry
        {
            root = viewCulled.transform,
            renderers = renderers
        });

        // 초기 상태: 랜더링 강제 종료
        SetEntryVisibility(renderers, false);
    }

    public void Cull(ViewInfo viewInfo)
    {
        if (entries.Count == 0) return;

        // 해시셋을 사용해 이번 프레임에 "보여야 할 렌더러들"을 중복 없이 체크
        HashSet<CulledEntry> visibleEntries = new HashSet<CulledEntry>();

        List<FovInfo> fovInfos = viewInfo.FovInfos;

        foreach (FovInfo fov in fovInfos)
        {
            float cosHalfAngle = Mathf.Cos(fov.angle * 0.5f * Mathf.Deg2Rad);
            float radiusSq = fov.radius * fov.radius;

            foreach (CulledEntry entry in entries)
            {
                if (entry.root == null) continue;
                if (visibleEntries.Contains(entry)) continue; // 이미 보인다고 판정된 건 연산 패스 (진짜 최적화)

                Vector3 targetPos = entry.root.position;
                Vector3 toObj = targetPos - fov.origin;
                float distSq = toObj.sqrMagnitude;

                // [Step 1] 거리 체크
                if (distSq > radiusSq) continue;

                // [Step 2] 각도 체크
                float dist = Mathf.Sqrt(distSq);
                Vector3 dirToObj = toObj / dist;
                float dot = Vector3.Dot(fov.forward, dirToObj);

                if (dot < cosHalfAngle) continue;

                // [Step 3] 장애물 체크
                if (!Physics.Linecast(fov.origin + new Vector3(0.0f, 1.0f, 0.0f), targetPos + new Vector3(0.0f, 1.0f, 0.0f), ObstacleLayer))
                {
                    visibleEntries.Add(entry);
                }
            }
        }

        // 최종 결과 반영: 보인다고 판정된 그룹만 켜고, 나머지는 완벽히 끔
        foreach (CulledEntry entry in entries)
        {
            bool isVisible = visibleEntries.Contains(entry);
            SetEntryVisibility(entry.renderers, isVisible);
        }
    }

    // SkinnedMeshRenderer 컴포넌트 이슈를 원천 차단하는 강제 렌더링 제어 함수
    private void SetEntryVisibility(Renderer[] renderers, bool isVisible)
    {
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;

            // 일반 렌더러와 스킨드 메쉬 렌더러 모두에게 완벽히 먹히는 하드웨어 레벨 제어
            r.forceRenderingOff = !isVisible;
        }
    }

    public void Clear() => entries.Clear();
}