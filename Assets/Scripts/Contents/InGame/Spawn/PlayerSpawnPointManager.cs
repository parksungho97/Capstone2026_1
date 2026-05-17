using UnityEngine;

public class PlayerSpawnPointManager : MonoBehaviour
{
    [SerializeField] private PlayerSpawnPoint[] spawnPoints;

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[PlayerSpawnPointManager] SpawnPoint가 없습니다.");
            return transform;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].transform;
    }
}