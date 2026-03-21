using UnityEngine;

// 플레이어가 가질 수 있는 거점 활성화 컴포넌트
public class CaptureInteractor : MonoBehaviour
{
    // 만약 현재 충돌중인 거점이 있으면 활성화 시작(딱 한번만 호출됌)
    public void TryStartActivateCapturePoint(ERequestType requestType)
    {
        if (capturePoint && activater && bAlreadyStartActivate == false)
        {
            activater.StartActivateRpc(requestType);
            bAlreadyStartActivate = true;
        }
    }

    // 만약 현재 활성화 중인 거점의 활성화를 중지함
    public void TryStopActivateCapturePoint(ERequestType requestType)
    {
        if (capturePoint && activater && bAlreadyStartActivate)
        {
            activater.StopActivateRpc(requestType);
            bAlreadyStartActivate = false;
        }
    }

    // 현재 활성화 가능한 거점이 있는지 확인
    public bool IsReadyToActivate()
    {
        return capturePoint && activater && bAlreadyStartActivate == false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Assert(this.capturePoint == null && this.activater == null);

        CapturePoint capturePoint = other.gameObject.GetComponent<CapturePoint>();
        Activater activater = other.gameObject.GetComponent<Activater>();

        if (capturePoint && activater)
        {
            this.capturePoint = capturePoint;
            this.activater = activater;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CapturePoint capturePoint = other.gameObject.GetComponent<CapturePoint>();
        Activater activater = other.gameObject.GetComponent<Activater>();

        if (capturePoint && activater)
        {
            Debug.Assert(capturePoint && activater);

            this.capturePoint = null;
            this.activater = null;
        }
    }

    private CapturePoint capturePoint;
    private Activater activater;
    private bool bAlreadyStartActivate = false;
}
