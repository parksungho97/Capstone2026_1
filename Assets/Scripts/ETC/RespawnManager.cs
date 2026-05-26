using Fusion;
using UnityEngine;

public class RespawnManager : NetworkBehaviour
{
    [Header("Respawn Points")]
    [SerializeField] private RespawnPoint[] respawnPoints;

    [Header("Safety Check")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkRadius = 0.6f;
    [SerializeField] private Vector3 checkOffset = Vector3.up * 0.5f;

    public Vector3 GetRandomSafeRespawnPosition()
    {
        if (respawnPoints == null || respawnPoints.Length == 0)
        {
            Debug.LogWarning("[RespawnManager] RespawnPoint가 없습니다.");
            return Vector3.zero;
        }

        int startIndex = Random.Range(0, respawnPoints.Length);

        for (int i = 0; i < respawnPoints.Length; i++)
        {
            int index = (startIndex + i) % respawnPoints.Length;

            if (respawnPoints[index] == null)
                continue;

            Vector3 position = respawnPoints[index].transform.position;

            if (IsSpawnPointSafe(position))
            {
                return position;
            }
        }

        Debug.LogWarning("[RespawnManager] 안전한 리스폰 포인트를 찾지 못했습니다. 첫 번째 포인트를 사용합니다.");
        return respawnPoints[0].transform.position;
    }

    private bool IsSpawnPointSafe(Vector3 position)
    {
        Vector3 checkPosition = position + checkOffset;

        bool hasObstacle = Physics.CheckSphere(
            checkPosition,
            checkRadius,
            obstacleLayer
        );

        return !hasObstacle;
    }

    public void RespawnPlayer(NetworkObject playerObject)
    {
        if (!Object.HasStateAuthority) return;
        if (playerObject == null) return;

        Vector3 respawnPosition = GetRandomSafeRespawnPosition();

        playerObject.transform.position = respawnPosition;

        CharacterHealth playerHealth = playerObject.GetComponent<CharacterHealth>();
        if (playerHealth != null)
        {
            playerHealth.RPC_ResetStat();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (respawnPoints == null) return;

        Gizmos.color = Color.green;

        foreach (RespawnPoint point in respawnPoints)
        {
            if (point == null) continue;

            Vector3 checkPosition = point.transform.position + checkOffset;
            Gizmos.DrawWireSphere(checkPosition, checkRadius);
        }
    }
}