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
        CapturePoint = GetComponent<CapturePoint>();
        Activater = GetComponent<Activater>();

        Debug.Assert(CapturePoint);
        Debug.Assert(Activater);
    }

    // 호스트에서 매 프레임 돌면서 활성화 가능한 거점이 있는지 확인하고 자동으로 활성화 시켜줌
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        EActivateSuccessType successType = Activater.IsActivatePossible();

        if (successType == EActivateSuccessType.Notyet)
            return;

        if (successType == EActivateSuccessType.Red)
            CapturePoint.SetCaptureStateRpc(ECaptureState.Red);
        else if (successType == EActivateSuccessType.Blue)
            CapturePoint.SetCaptureStateRpc(ECaptureState.Blue);

        Activater.ClearState();
    }

    public override void Spawned()
    {
        base.Spawned();

        Debug.Log($"RoomState: {Runner.SessionInfo.Properties["RoomState"]}");
    }

    public CapturePoint CapturePoint { get; private set; }
    public Activater Activater { get; private set; }
}
