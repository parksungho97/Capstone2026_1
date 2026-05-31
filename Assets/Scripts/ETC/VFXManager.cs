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
    private Dictionary<GameObject, int> persistentObjects = new();

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
        GameObject obj = Activate(id, position, rotation);
        if (obj != null)
            StartCoroutine(ReturnWhenDone(id, obj));
        return obj;
    }

    public GameObject Spawn(int id, Vector3 position)
        => Spawn(id, position, Quaternion.identity);

    // Spawns without auto-return. Caller must call ReturnToPool when done.
    public GameObject SpawnPersistent(int id, Vector3 position)
    {
        GameObject obj = Activate(id, position, Quaternion.identity);
        if (obj != null)
            persistentObjects[obj] = id;
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null || !persistentObjects.TryGetValue(obj, out int id)) return;
        persistentObjects.Remove(obj);
        foreach (var ps in particleCache[obj])
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        StartCoroutine(ReturnWhenDone(id, obj));
    }

    private GameObject Activate(int id, Vector3 position, Quaternion rotation)
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
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.None;
            ps.Play(true);
        }

        return obj;
    }

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
