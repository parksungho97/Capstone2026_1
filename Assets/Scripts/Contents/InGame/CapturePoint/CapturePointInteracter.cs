using Fusion;
using UnityEngine;

// Entering a trigger zone caches the capture point.
// Call TryStartCapture() / TryStopCapture() (e.g. on Space press/release) to
// actually send requests. Exiting the zone automatically releases any active request.
// Call SetTeam() once after the player's team is determined (e.g. in Initalize).
public class CapturePointInteracter : MonoBehaviour
{
    public bool IsInsideCaptureZone => mCurrentController != null;
    public bool IsCapturing => mIsCapturing;

    public void SetTeam(ECaptureState team)
    {
        mTeam = team;
        mTeamSet = true;
    }

    public void TryStartCapture()
    {
        if (!IsAuthority() || !mTeamSet) return;
        if (mCurrentController == null || mIsCapturing) return;

        mIsCapturing = true;
        mCurrentController.RequestCaptureRpc(mRequesterId, mTeam);
    }

    public void TryStopCapture()
    {
        if (!IsAuthority() || !mIsCapturing) return;

        mIsCapturing = false;
        mCurrentController?.ReleaseCaptureRpc(mRequesterId);
    }

    private void Awake()
    {
        mNetworkObject = GetComponent<NetworkObject>();
        mRequesterId = gameObject.GetInstanceID();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsAuthority()) return;
        if (mCurrentController != null) return;

        CapturePointController controller = other.GetComponent<CapturePointController>();
        if (controller == null) return;

        mCurrentController = controller;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsAuthority() || mCurrentController == null) return;

        CapturePointController controller = other.GetComponent<CapturePointController>();
        if (controller != mCurrentController) return;

        if (mIsCapturing)
        {
            mIsCapturing = false;
            mCurrentController.ReleaseCaptureRpc(mRequesterId);
        }
        mCurrentController = null;
    }

    private void OnDestroy()
    {
        if (mIsCapturing && mCurrentController != null && IsAuthority())
        {
            mCurrentController.ReleaseCaptureRpc(mRequesterId);
        }
    }

    private bool IsAuthority() => mNetworkObject != null && mNetworkObject.HasStateAuthority;

    private NetworkObject mNetworkObject;
    private CapturePointController mCurrentController;
    private ECaptureState mTeam;
    private bool mTeamSet;
    private bool mIsCapturing;
    private int mRequesterId;
}
