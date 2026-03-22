using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 9f, -7f);
    [SerializeField] private Vector3 fixedEulerAngles = new Vector3(40f, 0f, 0f);

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void Start()
    {
        Debug.Assert(targetCamera = GetComponent<Camera>());
    }

    private void LateUpdate()
    {
        if (target == null) 
            return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(fixedEulerAngles);
    }

    public bool GetMouseWorldPosition(Vector3 mousePosition, out Vector3 mouseWorldPosition)
    {
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
