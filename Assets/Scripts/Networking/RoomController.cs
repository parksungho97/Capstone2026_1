using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public enum EReadyStatus : ushort
    {
        NotReady, 
        Ready,
        Error,
    }

    public struct RoomMemberInfo : INetworkSerializable, System.IEquatable<RoomMemberInfo>
    {
        public ulong playerId;
        public EReadyStatus readyStatus;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref readyStatus);
        }

        // NetworkList는 Equals로 변경 사항을 체크하므로 구현 필요
        public bool Equals(RoomMemberInfo other) => playerId == other.playerId && readyStatus == other.readyStatus;
    }

    public class RoomController : NetworkBehaviour
    {
        public Action<RoomMemberInfo> ActionRoomMemberInfoChanged;

        [ServerRpc(RequireOwnership = false)]
        public void BindMemberServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;

            ulong clientId = serverRpcParams.Receive.SenderClientId;
            if (TryFindMemberIndex(clientId, out int index))
                return;

            RoomMemberInfo roomMemberInfo = new RoomMemberInfo
            {
                playerId = clientId,
                readyStatus = EReadyStatus.NotReady,
            };
            mRoomMemberInfos.Add(roomMemberInfo);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReleaseMemberServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;

            ulong clientId = serverRpcParams.Receive.SenderClientId;
            if (TryFindMemberIndex(clientId, out int index) == false)
                return;

            mRoomMemberInfos.RemoveAt(index);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer) return;

            ulong clientId = serverRpcParams.Receive.SenderClientId;
            if (TryFindMemberIndex(clientId, out int index) == false)
                return;

            var info = mRoomMemberInfos[index];
            info.readyStatus = (info.readyStatus == EReadyStatus.NotReady)
                               ? EReadyStatus.Ready
                               : EReadyStatus.NotReady;

            mRoomMemberInfos[index] = info;
        }

        public bool IsReadyToStart()
        {
            foreach(RoomMemberInfo roomMemberInfo in mRoomMemberInfos)
            {
                if (roomMemberInfo.readyStatus == EReadyStatus.NotReady)
                    return false;
            }
            return true;
        }

        public bool TryGetMemberReadyStatus(ulong userId, out EReadyStatus readyStatus)
        {
            if(TryFindMemberIndex(userId, out int index))
            {
                readyStatus = mRoomMemberInfos[index].readyStatus;
                return true;
            }
            readyStatus = EReadyStatus.Error;
            return false;
        }
        
        private bool TryFindMemberIndex(ulong playerId, out int index)
        {
            for(int i = 0;i<mRoomMemberInfos.Count;++i)
            {
                if (mRoomMemberInfos[i].playerId == playerId)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            mRoomMemberInfos.OnListChanged += (NetworkListEvent<RoomMemberInfo> changeEvent) =>
            {
                ActionRoomMemberInfoChanged?.Invoke(changeEvent.Value);
            };
        }
        private void Awake()
        {
            mRoomMemberInfos = new();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            mRoomMemberInfos.Dispose();
        }
        private NetworkList<RoomMemberInfo> mRoomMemberInfos;
    }

}
