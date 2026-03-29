using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5.0f;
    [SerializeField] private float fov = 60.0f;

    public float MaxDistance => maxDistance;
    public float Fov => fov;

    // worldPos 가 이 FOV 콘 안에 있으면 true.
    public bool IsInFOV(Vector3 worldPos)
    {
        Vector3 toTarget = worldPos - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > maxDistance * maxDistance) 
            return false;

        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
        return Vector3.Angle(forward, toTarget) <= fov * 0.5f;
    }

}
