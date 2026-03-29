using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[System.Serializable]
public struct FovInfo
{
    public Vector3 origin;   // float3 (12 bytes)
    public float angle;      // float  (4 bytes)
    public Vector3 forward;  // float3 (12 bytes)
    public float radius;     // float  (4 bytes)
}

public class ViewInfo
{
    public uint GridX { get; private set; }
    public uint GridY { get; private set; }
    public float WorldX { get; private set; }
    public float WorldY { get; private set; }

    public List<FovInfo> FovInfos {  get; private set; }

    public List<Renderer> Obstacles { get; private set; }

    public ViewInfo(uint gx, uint gy, float wx, float wy)
    {
        FovInfos = new List<FovInfo>();
        Obstacles = new List<Renderer>();

        GridX = gx; GridY = gy;
        WorldX = wx; WorldY = wy;
    }

    public void Clear() { FovInfos.Clear(); }

    public void AddFovInfo(Vector3 origin, Vector3 forward, float angle, float radius)
    {
        FovInfos.Add(new FovInfo { origin = origin, forward = forward, angle = angle, radius = radius });
    }

    public void RegistObstacle(GameObject obstacle)
    {
        Renderer renderer = obstacle.GetComponent<Renderer>();
        Debug.Assert(renderer, $"Obstacle {obstacle.name} has no Renderer component.");

        Obstacles.Add(renderer);
    }
    public Vector3 GetLeftBottom()
    {
        return new Vector3(-WorldX * 0.5f, 0, -WorldY * 0.5f);
    }

    public Vector2Int GetCell(Vector3 worldPos)
    {
        // 1. 좌측 하단(-WorldWidth/2, -WorldHeight/2)을 기준으로 0 ~ 1 정규화
        float normX = (worldPos.x + WorldX * 0.5f) / WorldX;
        float normZ = (worldPos.z + WorldY * 0.5f) / WorldY; // -worldZ 대신 worldZ 사용

        // 2. 인덱스 변환
        int x = (int)(normX * GridX);
        int z = (int)(normZ * GridY);

        // 3. 범위 체크
        if (x < 0 || z < 0 || x >= GridX || z >= GridY)
            return new Vector2Int(-1, -1);

        return new Vector2Int(x, z);
    }
    //public void AddObstacle(GameObject obstacle)
    //{
    //    Bounds aabb = col.bounds;
    //    int gxMin = Mathf.Max(0, Mathf.FloorToInt((aabb.min.x - _worldOrigin.x) / _viewGrid.WorldX * gridXf));
    //    int gxMax = Mathf.Min((int)_viewGrid.GridX - 1, Mathf.CeilToInt((aabb.max.x - _worldOrigin.x) / _viewGrid.WorldX * gridXf));
    //    int gzMin = Mathf.Max(0, Mathf.FloorToInt((aabb.min.z - _worldOrigin.z) / _viewGrid.WorldY * gridYf));
    //    int gzMax = Mathf.Min((int)_viewGrid.GridY - 1, Mathf.CeilToInt((aabb.max.z - _worldOrigin.z) / _viewGrid.WorldY * gridYf));

    //    // 비볼록 MeshCollider는 ClosestPoint가 지원되지 않으므로 AABB 폴백
    //    bool useAabb = col is MeshCollider mc && !mc.convex;

    //    for (int gx = gxMin; gx <= gxMax; gx++)
    //    {
    //        for (int gz = gzMin; gz <= gzMax; gz++)
    //        {
    //            if (useAabb)
    //            {
    //                cells.Add(new ObstacleCell { x = (uint)gx, y = (uint)gz });
    //                continue;
    //            }

    //            float wx = _worldOrigin.x + (gx + 0.5f) / gridXf * _viewGrid.WorldX;
    //            float wz = _worldOrigin.z + (gz + 0.5f) / gridYf * _viewGrid.WorldY;
    //            Vector3 cellCenter = new Vector3(wx, aabb.center.y, wz);

    //            // 셀 중심이 Collider 내부에 있으면 ClosestPoint == cellCenter
    //            Vector3 closest = col.ClosestPoint(cellCenter);
    //            if ((closest - cellCenter).sqrMagnitude < 0.0001f)
    //                cells.Add(new ObstacleCell { x = (uint)gx, y = (uint)gz });
    //        }
    //    }
    //}
    //public void SetObstacles(IEnumerable<Transform> obstacles)
    //{
    //    if (_viewGrid == null) return;

    //    float gridXf = (float)_viewGrid.GridX;
    //    float gridYf = (float)_viewGrid.GridY;

    //    var cells = new List<ObstacleCell>();
    //    foreach (var t in obstacles)
    //    {
    //        Collider col = t.GetComponent<Collider>();
    //        if (col == null) continue;

    //        Bounds aabb = col.bounds;
    //        int gxMin = Mathf.Max(0, Mathf.FloorToInt((aabb.min.x - _worldOrigin.x) / _viewGrid.WorldX * gridXf));
    //        int gxMax = Mathf.Min((int)_viewGrid.GridX - 1, Mathf.CeilToInt((aabb.max.x - _worldOrigin.x) / _viewGrid.WorldX * gridXf));
    //        int gzMin = Mathf.Max(0, Mathf.FloorToInt((aabb.min.z - _worldOrigin.z) / _viewGrid.WorldY * gridYf));
    //        int gzMax = Mathf.Min((int)_viewGrid.GridY - 1, Mathf.CeilToInt((aabb.max.z - _worldOrigin.z) / _viewGrid.WorldY * gridYf));

    //        // 비볼록 MeshCollider는 ClosestPoint가 지원되지 않으므로 AABB 폴백
    //        bool useAabb = col is MeshCollider mc && !mc.convex;

    //        for (int gx = gxMin; gx <= gxMax; gx++)
    //        {
    //            for (int gz = gzMin; gz <= gzMax; gz++)
    //            {
    //                if (useAabb)
    //                {
    //                    cells.Add(new ObstacleCell { x = (uint)gx, y = (uint)gz });
    //                    continue;
    //                }

    //                float wx = _worldOrigin.x + (gx + 0.5f) / gridXf * _viewGrid.WorldX;
    //                float wz = _worldOrigin.z + (gz + 0.5f) / gridYf * _viewGrid.WorldY;
    //                Vector3 cellCenter = new Vector3(wx, aabb.center.y, wz);

    //                // 셀 중심이 Collider 내부에 있으면 ClosestPoint == cellCenter
    //                Vector3 closest = col.ClosestPoint(cellCenter);
    //                if ((closest - cellCenter).sqrMagnitude < 0.0001f)
    //                    cells.Add(new ObstacleCell { x = (uint)gx, y = (uint)gz });
    //            }
    //        }
    //    }
    //}
}
