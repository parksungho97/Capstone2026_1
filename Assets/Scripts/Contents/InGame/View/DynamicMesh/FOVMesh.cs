using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FOVMesh : MonoBehaviour
{
    [SerializeField] private int       rayStepDeg    = 1;
    [SerializeField] private float     meshY         = 0.05f;
    [SerializeField] private LayerMask _obstacleMask;

    private PlayerFOV  _playerFOV;
    private Mesh       _mesh;
    private MeshFilter _meshFilter;
    private Vector3[]  _vertexBuffer;

    private void Awake()
    {
        _meshFilter            = GetComponent<MeshFilter>();
        _mesh                  = new Mesh { name = "FOVMesh" };
        _mesh.MarkDynamic();
        _meshFilter.sharedMesh = _mesh;
    }

    // EntryPoint에서 한 번 호출: PlayerFOV 주입 + Triangle topology 구성
    public void BindFOV(PlayerFOV playerFOV)
    {
        _playerFOV = playerFOV;

        int steps     = Mathf.Max(1, Mathf.RoundToInt(_playerFOV.Fov / Mathf.Max(1, rayStepDeg)));
        _vertexBuffer = new Vector3[steps + 2];

        var triangles = new int[steps * 3];
        for (int i = 0; i < steps; i++)
        {
            triangles[i * 3]     = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        _mesh.vertices  = _vertexBuffer;
        _mesh.triangles = triangles;
    }

    // 매 프레임 호출: 레이캐스트로 버텍스 위치 갱신
    public void UpdateMesh(Transform source)
    {
        int   steps   = _vertexBuffer.Length - 2;
        float fov     = _playerFOV.Fov;
        float halfFov = fov * 0.5f;
        float maxDist = _playerFOV.MaxDistance;

        _vertexBuffer[0] = new Vector3(0f, meshY, 0f);

        for (int i = 0; i <= steps; i++)
        {
            float angleDeg = halfFov - i * (fov / steps);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 localDir = new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
            Vector3 worldDir = source.TransformDirection(localDir);

            float dist = maxDist;
            if (Physics.Raycast(source.position + Vector3.up * 0.1f, worldDir, out RaycastHit hit, maxDist, _obstacleMask))
                dist = hit.distance;

            _vertexBuffer[i + 1] = new Vector3(localDir.x * dist, meshY, localDir.z * dist);
        }

        _mesh.SetVertices(_vertexBuffer);
        _mesh.RecalculateBounds();
    }
}


