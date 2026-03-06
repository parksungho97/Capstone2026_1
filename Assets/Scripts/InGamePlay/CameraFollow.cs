using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 9f, -7f);
    [SerializeField] private Vector3 fixedEulerAngles = new Vector3(40f, 0f, 0f);

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(fixedEulerAngles);
    }
}