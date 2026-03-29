using TMPro.Examples;
using UnityEngine;

public class DynamicMeshEntryPoint : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private PlayerFOV  _playerFOV;
    [SerializeField] private FOVMesh    _fovMesh;
    [SerializeField] private FOVCulling _fovCulling;
    [SerializeField] private FOVPostProcess _fovPostProcess;
    [SerializeField] private Renderer[] _culledObjects;

    private void Start()
    {
        cameraController.SetTarget(_playerFOV.transform);
        _fovMesh.BindFOV(_playerFOV);

        _fovCulling.AddFOV(_playerFOV);
        foreach (Renderer r in _culledObjects)
            _fovCulling.AddCulledObject(r);

        _fovPostProcess.Initialize(Camera.main);
        _fovPostProcess.SetFOVMesh(_fovMesh.gameObject);
    }

    private void Update()
    {
        _fovMesh.UpdateMesh(_playerFOV.transform);
    }

    private void OnDestroy()
    {
        if (_fovCulling != null)
            _fovCulling.RemoveFOV(_playerFOV);
    }
}
