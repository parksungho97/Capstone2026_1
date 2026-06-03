//using UnityEngine;

//public class CameraController : MonoBehaviour
//{
//    [SerializeField] private Vector3 offset = new Vector3(0f, 9f, -7f);
//    [SerializeField] private Vector3 fixedEulerAngles = new Vector3(40f, 0f, 0f);

//    public void SetTarget(Transform target)
//    {
//        this.target = target;
//    }
//    private void Awake()
//    {
//        targetCamera = GetComponent<Camera>();
//        Debug.Assert(targetCamera);
//    }

//    private void LateUpdate()
//    {
//        if (target == null) 
//            return;

//        transform.position = target.position + offset;
//        transform.rotation = Quaternion.Euler(fixedEulerAngles);
//    }

//    public bool GetMouseWorldPosition(Vector3 mousePosition, out Vector3 mouseWorldPosition)
//    {
//        if (targetCamera == null)
//        {
//            mouseWorldPosition = Vector3.zero;
//            return false;
//        }

//        Ray ray = targetCamera.ScreenPointToRay(mousePosition);

//        Vector3 planeOrigin = (target != null) ? target.position : Vector3.zero;
//        Plane plane = new Plane(Vector3.up, planeOrigin);

//        if (plane.Raycast(ray, out float enter))
//        {
//            mouseWorldPosition = ray.GetPoint(enter);
//            return true;
//        }

//        mouseWorldPosition = Vector3.zero;
//        return false;
//    }

//    private Transform target;
//    private Camera targetCamera;

//}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 9f, -7f);
    [SerializeField] private Vector3 fixedEulerAngles = new Vector3(40f, 0f, 0f);

    [Header("Duck Game Style Camera")]
    [SerializeField] private float maxMouseOffsetDistance = 3.5f;
    [SerializeField] private float cameraSmoothSpeed = 10f;
    [Tooltip("Camera only starts offsetting when the mouse is beyond this distance from the player (world units)")]
    [SerializeField] private float deadZoneRadius = 2f;
    [Tooltip("Offset boost when the mouse points south, to compensate for the camera's built-in northward tilt bias (1 = no boost, 2 = double)")]
    [SerializeField] private float southOffsetMultiplier = 2f;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        Debug.Assert(targetCamera);
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        if (GetMouseWorldPosition(Input.mousePosition, out Vector3 mouseWorldPosition))
        {
            Vector3 playerToMouse = mouseWorldPosition - target.position;
            playerToMouse.y = 0f;

            // Only shift the camera once the mouse exits the dead zone around the player
            float mouseDistance = playerToMouse.magnitude;
            if (mouseDistance > deadZoneRadius)
            {
                float effectiveDistance = mouseDistance - deadZoneRadius;
                Vector3 direction = playerToMouse.normalized;

                // Camera faces north, so southward areas have less natural coverage.
                // Blend a boost based on how much the direction points south (-Z).
                float southness = Mathf.Max(0f, -direction.z);
                float boost = Mathf.Lerp(1f, southOffsetMultiplier, southness);

                Vector3 mouseOffset = direction * (effectiveDistance * 0.5f * boost);
                mouseOffset = Vector3.ClampMagnitude(mouseOffset, maxMouseOffsetDistance * boost);
                targetPosition += mouseOffset;
            }
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSmoothSpeed);

        // ?         
        transform.rotation = Quaternion.Euler(fixedEulerAngles);
    }

    public bool GetMouseWorldPosition(Vector3 mousePosition, out Vector3 mouseWorldPosition)
    {
        if (targetCamera == null)
        {
            mouseWorldPosition = Vector3.zero;
            return false;
        }

        Ray ray = targetCamera.ScreenPointToRay(mousePosition);

        Vector3 planeOrigin = (target != null) ? target.position : Vector3.zero;
        Plane plane = new Plane(Vector3.up, planeOrigin);

        if (plane.Raycast(ray, out float enter))
        {
            mouseWorldPosition = ray.GetPoint(enter);
            return true;
        }

        mouseWorldPosition = Vector3.zero;
        return false;
    }

    private Transform target;
    private Camera targetCamera;
}