using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(CapturePoint))]
    [RequireComponent(typeof(Activater))]
    [RequireComponent(typeof(SphereCollider))]
    public class CaptureZoneController : NetworkBehaviour
    {
        private CapturePoint mCapturePoint = null;
        private Activater mActivater = null;

        private readonly HashSet<ulong> mInsidePlayers = new HashSet<ulong>();
        private readonly Dictionary<ulong, ERequestType> mRequestingPlayers = new Dictionary<ulong, ERequestType>();

        private void Awake()
        {
            mCapturePoint = GetComponent<CapturePoint>();
            mActivater = GetComponent<Activater>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void StartCaptureServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;

            ulong clientId = serverRpcParams.Receive.SenderClientId;

            if (!mInsidePlayers.Contains(clientId))
                return;

            if (mRequestingPlayers.ContainsKey(clientId))
                return;

            if (TryGetRequestTypeFromRoomManager(clientId, out ERequestType requestType) == false)
                return;

            mRequestingPlayers.Add(clientId, requestType);
            mActivater.RegistActivateOnServer(requestType);
        }

        [ServerRpc(RequireOwnership = false)]
        public void StopCaptureServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;

            ulong clientId = serverRpcParams.Receive.SenderClientId;

            if (!mRequestingPlayers.TryGetValue(clientId, out ERequestType requestType))
                return;

            mActivater.UnregistActivateOnServer(requestType);
            mRequestingPlayers.Remove(clientId);
        }

        private void Update()
        {
            if (!IsServer)
                return;

            TryApplyCaptureResult();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            NetworkObject networkObject = other.GetComponentInParent<NetworkObject>();
            if (networkObject == null || !networkObject.IsPlayerObject)
                return;

            mInsidePlayers.Add(networkObject.OwnerClientId);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer)
                return;

            NetworkObject networkObject = other.GetComponentInParent<NetworkObject>();
            if (networkObject == null || !networkObject.IsPlayerObject)
                return;

            ulong clientId = networkObject.OwnerClientId;

            if (mRequestingPlayers.TryGetValue(clientId, out ERequestType requestType))
            {
                mActivater.UnregistActivateOnServer(requestType);
                mRequestingPlayers.Remove(clientId);
            }

            mInsidePlayers.Remove(clientId);
        }

        private void TryApplyCaptureResult()
        {
            EActivateSuccessType successType = mActivater.IsActivatePossible();

            switch (successType)
            {
                case EActivateSuccessType.Notyet:
                    break;

                case EActivateSuccessType.Red:
                    if (mCapturePoint.GetCaptureState() == ECaptureState.Red)
                        return;

                    Debug.Log($"[{gameObject.name}] RED TEAM CAPTURE COMPLETE");
                    mCapturePoint.SetCapturedOnServer(ECaptureState.Red);
                    ResetCaptureRequests();
                    break;

                case EActivateSuccessType.Blue:
                    if (mCapturePoint.GetCaptureState() == ECaptureState.Blue)
                        return;

                    Debug.Log($"[{gameObject.name}] BLUE TEAM CAPTURE COMPLETE");
                    mCapturePoint.SetCapturedOnServer(ECaptureState.Blue);
                    ResetCaptureRequests();
                    break;
            }
        }
        private void ResetCaptureRequests()
        {
            mRequestingPlayers.Clear();
            mActivater.ClearState();
        }
        private bool TryGetRequestTypeFromRoomManager(ulong clientId, out ERequestType requestType)
        {
            requestType = ERequestType.End;

            RoomManager roomManager = FindFirstObjectByType<RoomManager>();
            if (roomManager == null)
                return false;

            if (roomManager.TryGetPlayerTeam(clientId, out Team team) == false)
                return false;

            switch (team)
            {
                case Team.Red:
                    requestType = ERequestType.Red;
                    return true;

                case Team.Blue:
                    requestType = ERequestType.Blue;
                    return true;

                default:
                    return false;
            }
        }
    }
}