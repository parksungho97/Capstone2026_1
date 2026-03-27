using UnityEngine;

// fovAngle + maxDist 만으로 원점 기준 +Z 방향 Triangle Fan 메쉬를 생성해 반환.
public class FOVMeshBuilder : MonoBehaviour
{
    public static FOVMeshBuilder Instance { get; private set; }

    [SerializeField] private int       rayStepDeg   = 1;       // 몇 도마다 레이 하나
    [SerializeField] private float     meshY        = 0.05f;   // 메시 Y 오프셋 (지형으로부터 깊이 싸움 방지)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // fovAngle·maxDist를 받아 로컬 공간 +Z 방향 콘 Mesh를 생성해 반환.
    public Mesh MakeFOVMesh(float fovAngle, float maxDist)
    {
        int steps = Mathf.Max(1, Mathf.RoundToInt(fovAngle / Mathf.Max(1, rayStepDeg)));

        var vertices  = new Vector3[steps + 2];
        var triangles = new int[steps * 3];

        float halfFov = fovAngle * 0.5f;
        vertices[0]   = new Vector3(0f, meshY, 0f);

        for (int i = 0; i <= steps; i++)
        {
            float angleDeg = 90f + halfFov - i * (fovAngle / steps);
            float angleRad = angleDeg * Mathf.Deg2Rad;
            Vector3 dir    = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
            vertices[i + 1] = new Vector3(dir.x * maxDist, meshY, dir.z * maxDist);
        }

        for (int i = 0; i < steps; i++)
        {
            triangles[i * 3]     = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        var mesh = new Mesh { name = "FOVMesh" };
        mesh.vertices  = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        return mesh;
    }
}
