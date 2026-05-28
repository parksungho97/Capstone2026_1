using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NeutralObjectInitializer : NetworkBehaviour
{
    [SerializeField] private List<int> itemIds;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority == false)
            return;

        ItemDropper[] droppers = FindObjectsByType<ItemDropper>(FindObjectsSortMode.None);
        foreach (ItemDropper dropper in droppers)
        {
            int id = itemIds[Random.Range(0, itemIds.Count)];
            dropper.SetDropData(new ItemId(id), 1);
        }
    }
}
