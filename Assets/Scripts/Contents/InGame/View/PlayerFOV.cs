using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
public class PlayerFOV : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5.0f;
    [SerializeField] private float fov = 60.0f;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        Debug.Assert(meshFilter != null, "PlayerFOV requires a MeshFilter component.");
        Debug.Assert(meshRenderer != null, "PlayerFOV requires a MeshRenderer component.");
    }

    public void SetMesh(Mesh mesh)
    {
        meshFilter.sharedMesh = mesh;
    }

    public void SetMesh(FOVMeshBuilder builder)
    {
        Mesh mesh = builder.MakeFOVMesh(fov, maxDistance);
        SetMesh(mesh);
    }

    // worldPos 가 이 FOV 콘 안에 있으면 true.
    public bool IsInFOV(Vector3 worldPos)
    {
        Vector3 toTarget = worldPos - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > maxDistance * maxDistance) return false;

        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
        return Vector3.Angle(forward, toTarget) <= fov * 0.5f;
    }

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
}
