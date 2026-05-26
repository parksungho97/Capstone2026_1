using Fusion;
using System.Collections;
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
        if (bSpawned == false)
            return;

        if (Object.HasStateAuthority == false)
            return;

        Chased chased = other.GetComponent<Chased>();
        if(chased)
            chaseds.Add(chased);
    }

    private void OnTriggerExit(Collider other)
    {
        if (bSpawned == false)
            return;

        if (Object.HasStateAuthority == false)
            return;

        Chased chased = other.GetComponent<Chased>();
        if (chased)
            chaseds.Remove(chased);
    }

    private List<Chased> chaseds = new List<Chased>();
    private bool bSpawned = false;
}
