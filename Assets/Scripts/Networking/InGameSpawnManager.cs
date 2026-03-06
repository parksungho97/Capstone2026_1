using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class InGameSpawnManager : NetworkBehaviour
    {
        [Header("Spawn References")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Spawner spawner;

        private bool spawnedOnce = false;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            if (NetworkManager.Singleton == null) return;
            if (NetworkManager.Singleton.SceneManager == null) return;

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            }
        }

        private void OnLoadEventCompleted(
            string sceneName,
            LoadSceneMode loadSceneMode,
            List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut)
        {
            if (!IsServer) return;
            if (spawnedOnce) return;
            if (sceneName != SceneNames.InGame) return;

            if (spawner != null)
                spawner.ResetSpawner();

            TrySpawnAllPlayers();
        }

        private void TrySpawnAllPlayers()
        {
            if (spawnedOnce) return;

            if (playerPrefab == null)
            {
                Debug.LogError("[InGameSpawnManager] playerPrefab이 할당되지 않았습니다.");
                return;
            }

            if (spawner == null)
            {
                Debug.LogError("[InGameSpawnManager] spawner가 할당되지 않았습니다.");
                return;
            }

            if (RoomManager.Instance == null)
            {
                Debug.LogError("[InGameSpawnManager] RoomManager.Instance가 null입니다.");
                return;
            }

            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("[InGameSpawnManager] NetworkManager.Singleton이 null입니다.");
                return;
            }

            int roomId = RoomManager.Instance.CurrentInGameRoomId;
            if (roomId < 0)
            {
                Debug.LogError("[InGameSpawnManager] CurrentInGameRoomId가 유효하지 않습니다.");
                return;
            }

            List<ulong> playerIds = RoomManager.Instance.GetPlayerIdsInRoom(roomId);
            if (playerIds == null || playerIds.Count == 0)
            {
                Debug.LogError($"[InGameSpawnManager] roomId={roomId} 에 스폰할 플레이어가 없습니다.");
                return;
            }

            bool allSuccess = true;

            foreach (ulong clientId in playerIds)
            {
                if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
                {
                    Debug.LogWarning($"[InGameSpawnManager] clientId={clientId} 는 ConnectedClients에 없습니다.");
                    allSuccess = false;
                    continue;
                }

                NetworkClient client = NetworkManager.Singleton.ConnectedClients[clientId];
                if (client.PlayerObject != null)
                {
                    Debug.LogWarning($"[InGameSpawnManager] clientId={clientId} 는 이미 PlayerObject가 존재합니다.");
                    continue;
                }

                if (!RoomManager.Instance.TryGetPlayerTeam(clientId, roomId, out Team team))
                {
                    Debug.LogError($"[InGameSpawnManager] clientId={clientId} 의 팀 정보를 찾을 수 없습니다.");
                    allSuccess = false;
                    continue;
                }

                Transform spawnPoint = spawner.GetSpawnPoint(clientId);
                if (spawnPoint == null)
                {
                    Debug.LogError($"[InGameSpawnManager] clientId={clientId} 의 spawnPoint가 null입니다.");
                    allSuccess = false;
                    continue;
                }

                GameObject playerObj = Instantiate(
                    playerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

                NetworkObject netObj = playerObj.GetComponent<NetworkObject>();
                if (netObj == null)
                {
                    Debug.LogError("[InGameSpawnManager] playerPrefab에 NetworkObject가 없습니다.");
                    Destroy(playerObj);
                    allSuccess = false;
                    continue;
                }

                netObj.SpawnAsPlayerObject(clientId, true);

                Debug.Log($"[InGameSpawnManager] Spawn 완료 roomId={roomId}, clientId={clientId}, team={team}");
            }

            if (allSuccess)
                spawnedOnce = true;
        }
    }
}