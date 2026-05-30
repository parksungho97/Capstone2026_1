using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VFXEntry
{
    public int id;
    public GameObject prefab;
    public int initialPoolSize;
}

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [SerializeField] private List<VFXEntry> vfxEntries;

    private Dictionary<int, GameObject> prefabMap = new();
    private Dictionary<int, Queue<GameObject>> pool = new();
    private Dictionary<GameObject, ParticleSystem[]> particleCache = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in vfxEntries)
        {
            prefabMap[entry.id] = entry.prefab;
            pool[entry.id] = new Queue<GameObject>();

            for (int i = 0; i < entry.initialPoolSize; i++)
                pool[entry.id].Enqueue(CreateInstance(entry.id, entry.prefab));
        }
    }

    public GameObject Spawn(int id, Vector3 position, Quaternion rotation)
    {
        if (!prefabMap.TryGetValue(id, out GameObject prefab))
            return null;

        GameObject obj = pool[id].Count > 0
            ? pool[id].Dequeue()
            : CreateInstance(id, prefab);

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        foreach (var ps in particleCache[obj])
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }

        StartCoroutine(ReturnWhenDone(id, obj));
        return obj;
    }

    public GameObject Spawn(int id, Vector3 position)
        => Spawn(id, position, Quaternion.identity);

    private GameObject CreateInstance(int id, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        particleCache[obj] = obj.GetComponentsInChildren<ParticleSystem>(true);
        return obj;
    }

    private IEnumerator ReturnWhenDone(int id, GameObject obj)
    {
        yield return null;

        ParticleSystem[] systems = particleCache[obj];
        while (true)
        {
            bool alive = false;
            foreach (var ps in systems)
            {
                if (ps.IsAlive(true)) { alive = true; break; }
            }
            if (!alive) break;
            yield return null;
        }

        obj.SetActive(false);
        pool[id].Enqueue(obj);
    }
}
