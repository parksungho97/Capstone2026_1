using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public enum Team : byte { Red = 0, Blue = 1 }

    public struct RoomKey : INetworkSerializable, IEquatable<RoomKey>
    {
        public int roomId;
        public RoomKey(int id) { roomId = id; }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            => serializer.SerializeValue(ref roomId);
        public bool Equals(RoomKey other) => roomId == other.roomId;
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

        public bool Equals(Room other) =>
            roomId == other.roomId &&
            roomName == other.roomName &&
            hostId == other.hostId &&
            maxPlayers == other.maxPlayers &&
            playerCount == other.playerCount;
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
        public bool Equals(RoomPlayerMappingContext other) => playerId == other.playerId && roomId == other.roomId;
    }

    public struct RoomTeamMappingContext : INetworkSerializable, IEquatable<RoomTeamMappingContext>
    {
        public ulong playerId;
        public int roomId;
        public Team team;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref roomId);
            serializer.SerializeValue(ref team);
        }
        public bool Equals(RoomTeamMappingContext other) => playerId == other.playerId && roomId == other.roomId && team == other.team;
    }

    public struct RoomPlayerReadyContext : INetworkSerializable, IEquatable<RoomPlayerReadyContext>
    {
        public ulong playerId;
        public int roomId;
        public bool isReady;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref roomId);
            serializer.SerializeValue(ref isReady);
        }
        public bool Equals(RoomPlayerReadyContext other) => playerId == other.playerId && roomId == other.roomId && isReady == other.isReady;
    }

    public class RoomManager : NetworkBehaviour
    {
        public static RoomManager Instance { get; private set; }

        public event Action ActionRoomsChanged;
        public event Action<int> ActionRoomMembersChanged;
        public event Action<int> ActionRoomReadyChanged;
        public event Action<RoomKey> ActionRoomListChanged;

        public NetworkList<Room> Rooms => mRooms;
        public NetworkList<RoomPlayerMappingContext> RoomPlayers => mRoomPlayers;
        public NetworkList<RoomTeamMappingContext> RoomTeams => mRoomTeams;
        public NetworkList<RoomPlayerReadyContext> RoomReady => mRoomReady;

        private static int sNextRoomId = 0;

        // ✅ 필드에서 즉시 초기화 (NGO 요구사항)
        private NetworkList<Room> mRooms = new();
        private NetworkList<RoomPlayerMappingContext> mRoomPlayers = new();
        private NetworkList<RoomTeamMappingContext> mRoomTeams = new();
        private NetworkList<RoomPlayerReadyContext> mRoomReady = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            mRooms.OnListChanged += OnRoomsListChanged;
            mRoomPlayers.OnListChanged += OnRoomPlayersListChanged;
            mRoomTeams.OnListChanged += OnRoomTeamsListChanged;
            mRoomReady.OnListChanged += OnRoomReadyListChanged;

            ActionRoomsChanged?.Invoke();
            ActionRoomListChanged?.Invoke(default);

            if (IsServer && NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        public override void OnNetworkDespawn()
        {
            if (mRooms != null) mRooms.OnListChanged -= OnRoomsListChanged;
            if (mRoomPlayers != null) mRoomPlayers.OnListChanged -= OnRoomPlayersListChanged;
            if (mRoomTeams != null) mRoomTeams.OnListChanged -= OnRoomTeamsListChanged;
            if (mRoomReady != null) mRoomReady.OnListChanged -= OnRoomReadyListChanged;

            if (IsServer && NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

            // ✅ 네이티브 누수 방지(1순위)
            SafeDisposeLists();

            base.OnNetworkDespawn();
        }

        private void OnDestroy()
        {
            // ✅ OnNetworkDespawn를 못 타는 경우 대비(2순위)
            SafeDisposeLists();
        }

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            // ✅ 에디터 PlayMode 종료 시 남는 케이스 대비
            SafeDisposeLists();
            Instance = null;
        }
#endif

        private void SafeDisposeLists()
        {
            try
            {
                // NetworkList는 내부적으로 NativeCollection을 사용함
                if (mRooms != null) mRooms.Dispose();
                if (mRoomPlayers != null) mRoomPlayers.Dispose();
                if (mRoomTeams != null) mRoomTeams.Dispose();
                if (mRoomReady != null) mRoomReady.Dispose();
            }
            catch { /* ignore */ }
            finally
            {
                // 중복 Dispose/재진입 방지
                mRooms = null;
                mRoomPlayers = null;
                mRoomTeams = null;
                mRoomReady = null;
            }
        }

        // ---------------------------
        // NetworkList -> Event bridge
        // ---------------------------

        private void OnRoomsListChanged(NetworkListEvent<Room> e)
        {
            ActionRoomsChanged?.Invoke();

            int roomId = (e.Type == NetworkListEvent<Room>.EventType.Remove)
                ? e.PreviousValue.roomId
                : e.Value.roomId;

            ActionRoomListChanged?.Invoke(new RoomKey(roomId));
        }

        private void OnRoomPlayersListChanged(NetworkListEvent<RoomPlayerMappingContext> e)
        {
            int roomId = (e.Type == NetworkListEvent<RoomPlayerMappingContext>.EventType.Remove)
                ? e.PreviousValue.roomId
                : e.Value.roomId;

            ActionRoomMembersChanged?.Invoke(roomId);
        }

        private void OnRoomTeamsListChanged(NetworkListEvent<RoomTeamMappingContext> e)
        {
            int roomId = (e.Type == NetworkListEvent<RoomTeamMappingContext>.EventType.Remove)
                ? e.PreviousValue.roomId
                : e.Value.roomId;

            ActionRoomMembersChanged?.Invoke(roomId);
        }

        private void OnRoomReadyListChanged(NetworkListEvent<RoomPlayerReadyContext> e)
        {
            int roomId = (e.Type == NetworkListEvent<RoomPlayerReadyContext>.EventType.Remove)
                ? e.PreviousValue.roomId
                : e.Value.roomId;

            ActionRoomReadyChanged?.Invoke(roomId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (!TryGetPlayerRoomId(clientId, out int roomId)) return;
            RemovePlayerFromRoomInternal(clientId, roomId);
            TryCleanupEmptyRoom(roomId);
        }

        // ---------------------------
        // Public helpers (UI에서 사용)
        // ---------------------------

        public bool TryGetRoom(int roomId, out Room room)
        {
            for (int i = 0; i < mRooms.Count; i++)
            {
                if (mRooms[i].roomId == roomId)
                {
                    room = mRooms[i];
                    return true;
                }
            }
            room = default;
            return false;
        }

        public bool TryGetPlayerRoomId(ulong playerId, out int roomId)
        {
            roomId = -1;
            for (int i = 0; i < mRoomPlayers.Count; i++)
            {
                var mp = mRoomPlayers[i];
                if (mp.playerId == playerId)
                {
                    roomId = mp.roomId;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetMyRoomId(out int roomId)
        {
            roomId = -1;
            if (NetworkManager.Singleton == null) return false;
            return TryGetPlayerRoomId(NetworkManager.Singleton.LocalClientId, out roomId);
        }

        public bool TryGetMyRoomKey(out RoomKey key)
        {
            key = default;
            if (!TryGetMyRoomId(out int roomId)) return false;
            key = new RoomKey(roomId);
            return true;
        }

        public bool IsLocalRoomHost(int roomId)
        {
            if (NetworkManager.Singleton == null) return false;
            if (!TryGetRoom(roomId, out var room)) return false;
            return room.hostId == NetworkManager.Singleton.LocalClientId;
        }

        public bool TryGetMyReady(int roomId, out bool ready)
        {
            ready = false;
            if (NetworkManager.Singleton == null) return false;
            ulong me = NetworkManager.Singleton.LocalClientId;

            for (int i = 0; i < mRoomReady.Count; i++)
            {
                var rc = mRoomReady[i];
                if (rc.roomId == roomId && rc.playerId == me)
                {
                    ready = rc.isReady;
                    return true;
                }
            }
            return false;
        }

        public bool AreAllNonHostReady(int roomId)
        {
            if (!TryGetRoom(roomId, out var room)) return false;
            ulong hostId = room.hostId;

            for (int i = 0; i < mRoomPlayers.Count; i++)
            {
                var mp = mRoomPlayers[i];
                if (mp.roomId != roomId) continue;
                if (mp.playerId == hostId) continue;

                bool found = false;
                bool isReady = false;

                for (int r = 0; r < mRoomReady.Count; r++)
                {
                    var rc = mRoomReady[r];
                    if (rc.roomId == roomId && rc.playerId == mp.playerId)
                    {
                        found = true;
                        isReady = rc.isReady;
                        break;
                    }
                }

                if (!found || !isReady) return false;
            }

            return true;
        }

        // ---------------------------
        // RPCs
        // ---------------------------

        [ServerRpc(RequireOwnership = false)]
        public void MakeRoomServerRpc(string roomName, uint maxPlayers, ServerRpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong sender = rpcParams.Receive.SenderClientId;
            if (TryGetPlayerRoomId(sender, out _)) return;

            int roomId = sNextRoomId++;

            var room = new Room
            {
                roomId = roomId,
                roomName = roomName,
                hostId = sender,
                maxPlayers = maxPlayers,
                playerCount = 0
            };

            mRooms.Add(room);
            EnterRoomInternal(sender, roomId);
            SendGoToRoomSceneTo(sender);
        }

        [ServerRpc(RequireOwnership = false)]
        public void EnterRoomServerRpc(RoomKey roomKey, ServerRpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong sender = rpcParams.Receive.SenderClientId;

            if (TryGetPlayerRoomId(sender, out _)) return;
            if (!TryFindRoomIndex(roomKey.roomId, out int idx)) return;

            var room = mRooms[idx];
            if (room.playerCount >= room.maxPlayers) return;

            bool ok = EnterRoomInternal(sender, roomKey.roomId);
            if (ok) SendGoToRoomSceneTo(sender);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetReadyServerRpc(RoomKey roomKey, bool ready, ServerRpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong sender = rpcParams.Receive.SenderClientId;
            int roomId = roomKey.roomId;

            if (!IsPlayerInRoom(sender, roomId)) return;

            for (int i = 0; i < mRoomReady.Count; i++)
            {
                var rc = mRoomReady[i];
                if (rc.roomId == roomId && rc.playerId == sender)
                {
                    rc.isReady = ready;
                    mRoomReady[i] = rc;
                    return;
                }
            }

            mRoomReady.Add(new RoomPlayerReadyContext
            {
                playerId = sender,
                roomId = roomId,
                isReady = ready
            });
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeTeamServerRpc(RoomKey roomKey, Team newTeam, ServerRpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong sender = rpcParams.Receive.SenderClientId;
            int roomId = roomKey.roomId;

            if (!IsPlayerInRoom(sender, roomId)) return;

            for (int i = 0; i < mRoomTeams.Count; i++)
            {
                var tm = mRoomTeams[i];
                if (tm.roomId == roomId && tm.playerId == sender)
                {
                    tm.team = newTeam;
                    mRoomTeams[i] = tm;
                    break;
                }
            }

            ActionRoomMembersChanged?.Invoke(roomId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void LeaveRoomAndGoLobbyServerRpc(ServerRpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong sender = rpcParams.Receive.SenderClientId;

            if (!TryGetPlayerRoomId(sender, out int roomId))
            {
                SendGoToLobbySceneTo(sender);
                return;
            }

            bool wasHost = false;
            if (TryGetRoom(roomId, out var room))
                wasHost = room.hostId == sender;

            if (wasHost)
            {
                if (TryGetFirstOtherPlayerInRoom(roomId, sender, out ulong newHost))
                    SetRoomHostInternal(roomId, newHost);
            }

            RemovePlayerFromRoomInternal(sender, roomId);
            SendGoToLobbySceneTo(sender);
            TryCleanupEmptyRoom(roomId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void StartGameServerRpc(RoomKey roomKey, ServerRpcParams rpcParams = default)
        {
            if (!IsServer) return;

            ulong sender = rpcParams.Receive.SenderClientId;
            int roomId = roomKey.roomId;

            if (!TryFindRoomIndex(roomId, out int idx)) return;

            var room = mRooms[idx];

            if (room.hostId != sender) return;
            if (!AreAllNonHostReady(roomId)) return;

            Debug.Log($"[RoomManager][Server] StartGame OK roomId={roomId} (TODO: inGame targeted 이동)");
        }

        // ---------------------------
        // Internal
        // ---------------------------

        private bool EnterRoomInternal(ulong playerId, int roomId)
        {
            if (!TryFindRoomIndex(roomId, out int idx)) return false;
            if (TryGetPlayerRoomId(playerId, out _)) return false;

            var room = mRooms[idx];
            if (room.playerCount >= room.maxPlayers) return false;

            int redCount = CountTeamMembers(roomId, Team.Red);
            int blueCount = CountTeamMembers(roomId, Team.Blue);

            Team teamToAssign;
            if (redCount == blueCount) teamToAssign = Team.Red;
            else if (blueCount < redCount) teamToAssign = Team.Blue;
            else teamToAssign = Team.Red;

            room.playerCount += 1;
            mRooms[idx] = room;

            mRoomPlayers.Add(new RoomPlayerMappingContext { playerId = playerId, roomId = roomId });
            mRoomTeams.Add(new RoomTeamMappingContext { playerId = playerId, roomId = roomId, team = teamToAssign });
            mRoomReady.Add(new RoomPlayerReadyContext { playerId = playerId, roomId = roomId, isReady = false });

            return true;
        }

        private void RemovePlayerFromRoomInternal(ulong playerId, int roomId)
        {
            for (int i = mRoomPlayers.Count - 1; i >= 0; i--)
                if (mRoomPlayers[i].roomId == roomId && mRoomPlayers[i].playerId == playerId)
                    mRoomPlayers.RemoveAt(i);

            for (int i = mRoomTeams.Count - 1; i >= 0; i--)
                if (mRoomTeams[i].roomId == roomId && mRoomTeams[i].playerId == playerId)
                    mRoomTeams.RemoveAt(i);

            for (int i = mRoomReady.Count - 1; i >= 0; i--)
                if (mRoomReady[i].roomId == roomId && mRoomReady[i].playerId == playerId)
                    mRoomReady.RemoveAt(i);

            if (TryFindRoomIndex(roomId, out int idx))
            {
                var room = mRooms[idx];
                if (room.playerCount > 0) room.playerCount -= 1;
                mRooms[idx] = room;
            }
        }

        private int CountTeamMembers(int roomId, Team team)
        {
            int count = 0;
            for (int i = 0; i < mRoomTeams.Count; i++)
            {
                var tm = mRoomTeams[i];
                if (tm.roomId == roomId && tm.team == team) count++;
            }
            return count;
        }

        private bool TryFindRoomIndex(int roomId, out int idx)
        {
            idx = -1;
            for (int i = 0; i < mRooms.Count; i++)
            {
                if (mRooms[i].roomId == roomId)
                {
                    idx = i;
                    return true;
                }
            }
            return false;
        }

        private bool IsPlayerInRoom(ulong playerId, int roomId)
        {
            for (int i = 0; i < mRoomPlayers.Count; i++)
            {
                var mp = mRoomPlayers[i];
                if (mp.playerId == playerId && mp.roomId == roomId) return true;
            }
            return false;
        }

        private void TryCleanupEmptyRoom(int roomId)
        {
            if (!TryFindRoomIndex(roomId, out int idx)) return;
            var room = mRooms[idx];
            if (room.playerCount != 0) return;

            mRooms.RemoveAt(idx);
        }

        private bool TryGetFirstOtherPlayerInRoom(int roomId, ulong excludePlayerId, out ulong playerId)
        {
            for (int i = 0; i < mRoomPlayers.Count; i++)
            {
                var mp = mRoomPlayers[i];
                if (mp.roomId != roomId) continue;
                if (mp.playerId == excludePlayerId) continue;

                playerId = mp.playerId;
                return true;
            }

            playerId = 0;
            return false;
        }

        private void SetRoomHostInternal(int roomId, ulong newHostId)
        {
            if (!TryFindRoomIndex(roomId, out int idx)) return;

            var room = mRooms[idx];
            room.hostId = newHostId;
            mRooms[idx] = room;
        }

        // ---------------------------
        // Scene 이동: 대상만 이동
        // ---------------------------

        [ClientRpc]
        private void GoToRoomSceneClientRpc(ClientRpcParams clientRpcParams = default)
        {
            if (SceneFlowManager.Instance != null)
                SceneFlowManager.Instance.LoadWithFade(SceneNames.Room);
            else
                SceneManager.LoadScene(SceneNames.Room);
        }

        [ClientRpc]
        private void GoToLobbySceneClientRpc(ClientRpcParams clientRpcParams = default)
        {
            if (SceneFlowManager.Instance != null)
                SceneFlowManager.Instance.LoadWithFade(SceneNames.Lobby);
            else
                SceneManager.LoadScene(SceneNames.Lobby);
        }

        private void SendGoToRoomSceneTo(ulong clientId)
        {
            var cp = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };
            GoToRoomSceneClientRpc(cp);
        }

        private void SendGoToLobbySceneTo(ulong clientId)
        {
            var cp = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };
            GoToLobbySceneClientRpc(cp);
        }

        // ---------------------------
        // Snapshot helpers
        // ---------------------------

        public System.Collections.Generic.IEnumerable<RoomTeamMappingContext> GetTeamMappingsSnapshot()
        {
            for (int i = 0; i < mRoomTeams.Count; i++)
                yield return mRoomTeams[i];
        }

        public System.Collections.Generic.IEnumerable<RoomPlayerMappingContext> GetPlayerMappingsSnapshot()
        {
            for (int i = 0; i < mRoomPlayers.Count; i++)
                yield return mRoomPlayers[i];
        }

        public System.Collections.Generic.IEnumerable<RoomPlayerReadyContext> GetReadySnapshot()
        {
            for (int i = 0; i < mRoomReady.Count; i++)
                yield return mRoomReady[i];
        }

        public int GetRoomCount() => mRooms?.Count ?? 0;
        public Room GetRoomAt(int index) => mRooms[index];
    }
}