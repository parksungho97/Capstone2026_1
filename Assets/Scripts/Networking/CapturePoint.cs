using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public enum ECaptureState : byte
    { 
        None,
        Red,
        Blue,
    }
    public class CapturePoint : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void SetCaptureStateServerRpc(ECaptureState captureState, ServerRpcParams serverRpcParams = default)  
        {
            Debug.Assert(captureState != ECaptureState.None);
            mState.Value = captureState;
        }

        public ECaptureState GetCaptureState() { return mState.Value; }

        private NetworkVariable<ECaptureState> mState = new NetworkVariable<ECaptureState>(ECaptureState.None);
    }
}