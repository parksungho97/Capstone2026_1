using Fusion;
using UnityEngine;

[RequireComponent(typeof(CapturePoint))]
[RequireComponent(typeof(Activater))]
[RequireComponent(typeof(SphereCollider))]
public class CapturePointController : NetworkBehaviour
{
    // 거점 활성화를 담당하는 컴포넌트
    private void Start()
    {
        mCapturePoint = GetComponent<CapturePoint>();
        mActivater = GetComponent<Activater>();

        Debug.Assert(mCapturePoint);
        Debug.Assert(mActivater);
    }

    // 호스트에서 매 프레임 돌면서 활성화 가능한 거점이 있는지 확인하고 자동으로 활성화 시켜줌
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        EActivateSuccessType successType = mActivater.IsActivatePossible();

        if (successType == EActivateSuccessType.Notyet)
            return;

        if (successType == EActivateSuccessType.Red)
            mCapturePoint.SetCaptureStateRpc(ECaptureState.Red);
        else if (successType == EActivateSuccessType.Blue)
            mCapturePoint.SetCaptureStateRpc(ECaptureState.Blue);

        mActivater.ClearState();
    }

    public override void Spawned()
    {
        base.Spawned();

        Debug.Log($"RoomState: {Runner.SessionInfo.Properties["RoomState"]}");
    }

    private CapturePoint mCapturePoint = null;
    private Activater mActivater = null;
}
