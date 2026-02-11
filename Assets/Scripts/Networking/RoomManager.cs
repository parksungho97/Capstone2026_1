using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

using System;
using JetBrains.Annotations; // IEquatable를 위해 필요

namespace Network
{
    public struct RoomKey : INetworkSerializable, IEquatable<RoomKey> 
    {
        public int roomId;
        public RoomKey(int roomId) { this.roomId = roomId; }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref roomId);
        }
        public bool Equals(RoomKey other) => roomId == other.roomId;
    }

    public class RoomManager : NetworkBehaviour
    {
        public static RoomManager Instance { get; private set; }
        // 방 정보가 바뀌었을 때 바뀐 방의 키값을 줌
        public event Action<RoomKey> ActionRoomListChanged;

        // 방의 키값으로 방을 조회할 수 있음.
        public bool TryFindRoom(RoomKey roomKey, out Room room)
        {
            if (IsMatchedRoom(roomKey, out int foundRoomIndex))
            {
                room = mRooms[foundRoomIndex];
                return true;
            }
            else
            {
                room = default;
                return false;
            }
        }
        // ServerRpc함수는 서버주도로 호출됌
        // 클라이언트가 호출하면 패킷으로 직렬화되어 서버로 날아가서 서버에서 실행됌
        [ServerRpc(RequireOwnership = false)]
        public void MakeRoomServerRpc(string roomName, uint maxPlayers, ServerRpcParams serverRpcParams = default)
        {
            // 방 생성(리스트 수정)은 서버만 가능
            if (!IsServer)
                return;

            ulong requestClientId = serverRpcParams.Receive.SenderClientId;

            for (int i = 0; i < mRooms.Count; ++i)
            {
                if (mRooms[i].roomName == roomName && mRooms[i].hostId == requestClientId)
                    return;
            }

            int roomId = sNextRoomId++;
            Room newRoom = new Room
            {
                roomId = roomId,
                roomName = roomName,
                hostId = requestClientId,
                maxPlayers = maxPlayers,
                playerCount = 0
            };

            mRooms.Add(newRoom);
        }
        [ServerRpc(RequireOwnership = false)]
        public void EnterRoomServerRpc(RoomKey roomKey, ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;

            ulong requestClientId = serverRpcParams.Receive.SenderClientId;

            // 1. 동일한 방이 있는지 확인
            if (IsMatchedRoom(roomKey, out int foundRoomIndex) == false)
                return;

            // 2. 플레이어가 이미 방과 매핑되어 있는지 확인
            for (int i = 0; i < mRoomPlayerMappingContexts.Count; ++i)
            {
                if (requestClientId == mRoomPlayerMappingContexts[i].playerId)
                    return;
            }

            Room foundRoom = mRooms[foundRoomIndex];
            if (foundRoom.maxPlayers == foundRoom.playerCount)
                return;

            foundRoom.playerCount += 1;
            mRooms[foundRoomIndex] = foundRoom;

            mRoomPlayerMappingContexts.Add(new RoomPlayerMappingContext { playerId = requestClientId, roomId = roomKey.roomId });
        }
        [ServerRpc(RequireOwnership = false)]
        public void ExitRoomServerRpc(RoomKey roomKey, ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;

            ulong requestClientId = serverRpcParams.Receive.SenderClientId;

            if (IsMatchedRoom(roomKey, out int foundRoomIndex) == false)
                return;

            bool bPossibleExit = false;
            foreach (var playerMappingContext in mRoomPlayerMappingContexts)
            {
                if (playerMappingContext.playerId == requestClientId && playerMappingContext.roomId == roomKey.roomId)
                {
                    bPossibleExit = true;
                    break;
                }
            }
            if (bPossibleExit == false)
                return;

            Room matchedRoom = mRooms[foundRoomIndex];
            matchedRoom.playerCount -= 1;
            mRooms[foundRoomIndex] = matchedRoom;

            foreach (var playerMappingContext in mRoomPlayerMappingContexts)
            {
                if (playerMappingContext.playerId == requestClientId)
                {
                    mRoomPlayerMappingContexts.Remove(playerMappingContext);
                    break;
                }
            }
        }
        [ServerRpc(RequireOwnership = false)]
        public void DeleteRoomServerRpc(RoomKey roomKey, ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;

            ulong requestClientId = serverRpcParams.Receive.SenderClientId;

            if (IsMatchedRoom(roomKey, out int foundRoomIndex) == false)
                return;

            Room foundRoom = mRooms[foundRoomIndex];
            if (foundRoom.hostId != requestClientId)
                return;

            int roomId = foundRoom.roomId;
            mRooms.Remove(foundRoom);

            for (int i = mRoomPlayerMappingContexts.Count - 1; i >= 0; i--)
            {
                if (mRoomPlayerMappingContexts[i].roomId == roomId)
                    mRoomPlayerMappingContexts.RemoveAt(i);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public override void OnNetworkSpawn()
        {
            mRooms.OnListChanged += (NetworkListEvent<Room> changeEvent) =>
            {
                int roomId = (changeEvent.Type == NetworkListEvent<Room>.EventType.Remove)
                 ? changeEvent.PreviousValue.roomId
                 : changeEvent.Value.roomId;

                ActionRoomListChanged?.Invoke(new RoomKey(roomId));
            };
        }

        private bool IsMatchedRoom(RoomKey roomKey, out int foundRoomIndex)
        {
            foundRoomIndex = -1;
            for (int i = 0; i < mRooms.Count; ++i)
            {
                if (mRooms[i].roomId == roomKey.roomId)
                {
                    foundRoomIndex = i;
                    return true;
                }
            }
            return false;
        }
        private void Awake()
        {
            // 1. 싱글톤 중복 체크 (중복 인스턴스 파괴)
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);

            mRooms = new NetworkList<Room>();
            mRoomPlayerMappingContexts = new NetworkList<RoomPlayerMappingContext>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (mRooms != null) mRooms.Dispose();
            if (mRoomPlayerMappingContexts != null) mRoomPlayerMappingContexts.Dispose();

            // 3. 인스턴스 해제 (C++의 소멸자 처리와 유사)
            if (Instance == this)
                Instance = null;
        }
        private NetworkList<Room> mRooms = null;
        private NetworkList<RoomPlayerMappingContext> mRoomPlayerMappingContexts = null;
        static int sNextRoomId = 0;
    }

    public struct Room : INetworkSerializable, IEquatable<Room>
    {
        public int roomId;
        public FixedString32Bytes roomName;
        public ulong hostId;
        public uint maxPlayers;
        public uint playerCount;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref roomId);
            serializer.SerializeValue(ref roomName);
            serializer.SerializeValue(ref hostId);
            serializer.SerializeValue(ref maxPlayers);
            serializer.SerializeValue(ref playerCount);
        }

        public bool Equals(Room other)
        {
            return roomId == other.roomId &&
                   roomName == other.roomName &&
                   hostId == other.hostId &&
                   maxPlayers == other.maxPlayers &&
                   playerCount == other.playerCount;
        }
    }

    public struct RoomPlayerMappingContext : INetworkSerializable, IEquatable<RoomPlayerMappingContext>
    {
        public ulong playerId;
        public int roomId;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref roomId);
        }

        public bool Equals(RoomPlayerMappingContext other)
        {
            return playerId == other.playerId && roomId == other.roomId;
        }
    }
}

