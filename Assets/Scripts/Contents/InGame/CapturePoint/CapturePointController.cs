using Fusion;
using UnityEngine;

// State machine for a capture point.
//   Default  → uses jjhConqueror (capturing requires 2 per team)
//   Captured → uses jjhRebel    (uncapturing requires only 1)
// Callers always invoke RequestCaptureRpc / ReleaseCaptureRpc regardless of state.
[RequireComponent(typeof(CapturePoint))]
[RequireComponent(typeof(Conqueror))]
[RequireComponent(typeof(Rebel))]
public class CapturePointController : NetworkBehaviour
{
    // Networked gauge values for UI / other clients to read.
    [Networked] public float RedGauge { get; private set; }
    [Networked] public float BlueGauge { get; private set; }
    [Networked] public float RebelGauge { get; private set; }

    public CapturePoint CapturePoint => mCapturePoint;

    public override void Spawned()
    {
        base.Spawned();
        mCapturePoint = GetComponent<CapturePoint>();
        mConqueror = GetComponent<Conqueror>();
        mRebel = GetComponent<Rebel>();

        if (Object.HasStateAuthority)
        {
            mConqueror.OnCapture += OnConquerorCapture;
            mRebel.OnUncapture += OnRebelUncapture;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (mConqueror != null) mConqueror.OnCapture -= OnConquerorCapture;
        if (mRebel != null) mRebel.OnUncapture -= OnRebelUncapture;
    }

    // ── Public capture interface ────────────────────────────────────────────

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestCaptureRpc(int requesterId, ECaptureState team)
    {
        if (IsCaptured())
        {
            if (team != mCapturePoint.GetCaptureState())
                mRebel.AddRequest(requesterId);
            else
                mRebel.ClearRequest();
        }
        else
            mConqueror.AddRequest(requesterId, team);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReleaseCaptureRpc(int requesterId)
    {
        if (IsCaptured())
            mRebel.RemoveRequest(requesterId);
        else
            mConqueror.RemoveRequest(requesterId);
    }

    // ── Network tick ────────────────────────────────────────────────────────

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        float dt = Runner.DeltaTime;

        if (IsCaptured())
            mRebel.Tick(dt);
        else
            mConqueror.Tick(dt);

        RedGauge = mConqueror.GetRedGauge();
        BlueGauge = mConqueror.GetBlueGauge();
        RebelGauge = mRebel.GetGauge();
    }

    // ── Event handlers ──────────────────────────────────────────────────────

    private void OnConquerorCapture(ECaptureState team)
    {
        mCapturePoint.SetCaptureStateRpc(team);
        mConqueror.Clear();
    }

    private void OnRebelUncapture()
    {
        mCapturePoint.ClearStateRpc();
        mRebel.Clear();
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private bool IsCaptured() => mCapturePoint.GetCaptureState() != ECaptureState.None;

    private CapturePoint mCapturePoint;
    private Conqueror mConqueror;
    private Rebel mRebel;
}
