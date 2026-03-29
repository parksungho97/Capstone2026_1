using System.Collections.Generic;
using UnityEngine;

// 등록된 FOV 콘 안에 있는 오브젝트의 Renderer 를 비활성화.
// 매 프레임: 전체 true → FOV 에 걸린 것만 false.
public class FOVCulling : MonoBehaviour
{
    private readonly List<PlayerFOV> _fovList       = new List<PlayerFOV>();
    private readonly List<Renderer>  _culledObjects = new List<Renderer>();

    [SerializeField] private LayerMask obstacleMask;

    public void AddFOV(PlayerFOV fov)          => _fovList.Add(fov);
    public void AddCulledObject(Renderer r)    => _culledObjects.Add(r);
    public void RemoveFOV(PlayerFOV fov)       => _fovList.Remove(fov);
    public void RemoveCulledObject(Renderer r) => _culledObjects.Remove(r);

    private void Update()
    {
        // 만약에 추가를 한다면 장애물 고려를 안하는 놈도 있어야됌
        // 1) 모두 활성화
        foreach (Renderer r in _culledObjects)
            if (r != null) r.enabled = false;

        // 2) 하나라도 FOV 에 걸리면 비활성화
        foreach (Renderer r in _culledObjects)
        {
            foreach (PlayerFOV fov in _fovList)
            {
                if (fov.IsInFOV(r.transform.position) && HasLineOfSight(fov, r.transform.position))
                {
                    r.enabled = true;
                    break;
                }
            }
        }
    }
    // FOV 원점에서 targetPos 사이에 obstacleMask 에 해당하는 장애물이 없으면 true.
    private bool HasLineOfSight(PlayerFOV fov, Vector3 targetPos)
    {
        Vector3 origin = fov.transform.position;
        Vector3 dir    = targetPos - origin;
        return !Physics.Raycast(origin, dir.normalized, dir.magnitude, obstacleMask);
    }
}
