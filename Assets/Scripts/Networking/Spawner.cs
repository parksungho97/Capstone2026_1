using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class Spawner : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;

        private readonly Dictionary<ulong, Transform> assignedSpawnPoints = new();
        private readonly List<int> usedIndices = new();

        public Transform GetSpawnPoint(ulong clientId)
        {
            if (assignedSpawnPoints.TryGetValue(clientId, out Transform existingPoint))
                return existingPoint;

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("[Spawner] spawnPoints가 비어 있습니다.");
                return null;
            }

            if (usedIndices.Count >= spawnPoints.Length)
            {
                Debug.LogError("[Spawner] 사용 가능한 spawn point가 부족합니다.");
                return null;
            }

            List<int> availableIndices = new();

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (!usedIndices.Contains(i))
                    availableIndices.Add(i);
            }

            int randomListIndex = Random.Range(0, availableIndices.Count);
            int selectedIndex = availableIndices[randomListIndex];

            usedIndices.Add(selectedIndex);
            assignedSpawnPoints[clientId] = spawnPoints[selectedIndex];

            return spawnPoints[selectedIndex];
        }

        public void ResetSpawner()
        {
            assignedSpawnPoints.Clear();
            usedIndices.Clear();
        }
    }
}