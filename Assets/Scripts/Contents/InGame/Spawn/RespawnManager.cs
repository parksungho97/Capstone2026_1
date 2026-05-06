using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public RespawnArea respawnArea;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    public float checkRadius = 1.0f;
    public int maxTry = 30;

    public Vector3 GetSafePosition()
    {
        for (int i = 0; i < maxTry; i++)
        {
            Vector3 randomPos = respawnArea.GetRandomPoint();

            // 1. 바닥 찾기
            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 50f, groundLayer))
            {
                Vector3 groundPos = hit.point;

                // 2. 장애물 검사
                if (!Physics.CheckSphere(groundPos, checkRadius, obstacleLayer))
                {
                    return groundPos;
                }
            }
        }

        // 실패 시 fallback
        Debug.LogWarning("안전한 스폰 위치 못 찾음");
        return respawnArea.transform.position;
    }
}
