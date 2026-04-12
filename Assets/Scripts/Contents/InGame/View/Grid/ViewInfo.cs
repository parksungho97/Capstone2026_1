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
        if(renderer)
            Obstacles.Add(renderer);
        //Debug.Assert(renderer, $"Obstacle {obstacle.name} has no Renderer component.");
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
}
