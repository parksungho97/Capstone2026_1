using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPositions;

    public void MappingToSpawnPosition(ulong playerId)
    {
        if (mappingIndices.ContainsKey(playerId))
            return;
        Debug.Assert(currentMappingIndex < spawnPositions.Length);
        mappingIndices.Add(playerId, currentMappingIndex++);
    }
    public Transform GetMappingSpawnPosition(ulong playerId)
    {
        Debug.Assert(mappingIndices.ContainsKey(playerId));
        uint index =  mappingIndices[playerId];

        return spawnPositions[index];
    }
    public void InitializeFromGameContextManager(GameContextManager manager)
    {
        ClearMappingState();
        foreach (PlayerInfo playerInfo in  manager.PlayerInfos)
        {
            MappingToSpawnPosition(playerInfo.playerId);
        }
    }

    private void ClearMappingState()
    {
        mappingIndices.Clear();
        currentMappingIndex = 0;
    }
    private Dictionary<ulong, uint> mappingIndices = new();
    private uint currentMappingIndex;
}
