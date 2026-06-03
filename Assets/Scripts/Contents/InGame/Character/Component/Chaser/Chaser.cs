using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : NetworkBehaviour
{
    public bool HasTarget => chaseds.Count > 0;

    public override void Spawned()
    {
        base.Spawned();
        bSpawned = true;
    }

    public Transform NearestTarget
    {
        get
        {
            if (chaseds.Count == 0) return null;
            Transform nearest = null;
            float minSqDist = float.MaxValue;
            foreach (Chased chased in chaseds)
            {
                float sqDist = (chased.transform.position - transform.position).sqrMagnitude;
                if (sqDist < minSqDist) { minSqDist = sqDist; nearest = chased.transform; }
            }
            return nearest;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!bSpawned || !Object.HasStateAuthority) return;

        Chased chased = other.GetComponent<Chased>();
        if (chased && chased.IsActive)
        {
            chaseds.Add(chased);
            Action handler = () => RemoveChased(chased);
            inactiveHandlers[chased] = handler;
            chased.OnBecameInactive += handler;
            Debug.Log("Chased On");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!bSpawned || !Object.HasStateAuthority) return;

        Chased chased = other.GetComponent<Chased>();
        if (chased)
            RemoveChased(chased);
    }

    private void RemoveChased(Chased chased)
    {
        if (!chaseds.Remove(chased)) return;

        if (inactiveHandlers.TryGetValue(chased, out Action handler))
        {
            chased.OnBecameInactive -= handler;
            inactiveHandlers.Remove(chased);
        }

        Debug.Log("Chased Off");
    }

    private readonly List<Chased> chaseds = new List<Chased>();
    private readonly Dictionary<Chased, Action> inactiveHandlers = new Dictionary<Chased, Action>();
    private bool bSpawned = false;
}
