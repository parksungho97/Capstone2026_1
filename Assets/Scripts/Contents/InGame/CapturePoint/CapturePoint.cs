using Fusion;
using UnityEngine;

public enum ECaptureState : byte
{
    None,
    Red,
    Blue,
}
public class CapturePoint : NetworkBehaviour
{
    public ECaptureState GetCaptureState()
    {
        return mState;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetCaptureStateRpc(ECaptureState captureState)
    {
        Debug.Assert(captureState != ECaptureState.None);
        mState = captureState;

    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ClearStateRpc()
    {
        mState = ECaptureState.None;
    }

    [Networked]
    private ECaptureState mState { get; set; }
}
