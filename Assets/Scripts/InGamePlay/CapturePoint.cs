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
        public ECaptureState GetCaptureState()
        {
            return mState.Value;
        }

        public void SetCapturedOnServer(ECaptureState captureState)
        {
            if (!IsServer)
                return;

            Debug.Assert(captureState != ECaptureState.None);
            mState.Value = captureState;
        }

        public void ClearOnServer()
        {
            if (!IsServer)
                return;

            mState.Value = ECaptureState.None;
        }

        private NetworkVariable<ECaptureState> mState =
            new NetworkVariable<ECaptureState>(
                ECaptureState.None,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);
    }
}