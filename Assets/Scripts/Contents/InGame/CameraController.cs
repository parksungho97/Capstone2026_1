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
    [Tooltip("마우스 방향으로 카메라가 최대 얼마만큼 더 이동할지 결정하는 거리")]
    [SerializeField] private float maxMouseOffsetDistance = 3.5f;
    [Tooltip("카메라 추적 속도 (부드러운 이동을 위한 값)")]
    [SerializeField] private float cameraSmoothSpeed = 10f;

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

        // 1. 기본 카메라 목표 위치 계산 (플레이어 발밑 기준)
        Vector3 targetPosition = target.position + offset;

        // 2. 현재 마우스의 월드 좌표 가져오기
        if (GetMouseWorldPosition(Input.mousePosition, out Vector3 mouseWorldPosition))
        {
            // 3. 플레이어에서 마우스로 향하는 XZ 평면상의 벡터 구하기
            Vector3 playerToMouse = mouseWorldPosition - target.position;
            playerToMouse.y = 0f; // Y축(높이)은 무시

            // 4. 가중치를 계산하여 마우스 오프셋 벡터 생성 (예: 마우스 거리의 절반만큼 이동하되 최대치 제한)
            Vector3 mouseOffset = playerToMouse * 0.5f;
            mouseOffset = Vector3.ClampMagnitude(mouseOffset, maxMouseOffsetDistance);

            // 5. 최종 목표 위치에 마우스 오프셋 추가
            targetPosition += mouseOffset;
        }

        // 6. 부드러운 카메라 이동 (Lerp) - 뎍코프 특유의 쫀득한 카메라 무빙을 줍니다.
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSmoothSpeed);

        // 회전은 고정
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