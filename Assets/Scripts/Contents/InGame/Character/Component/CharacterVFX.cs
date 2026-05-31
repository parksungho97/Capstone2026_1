using UnityEngine;

public class CharacterVFX : MonoBehaviour
{
    [SerializeField] private int healVfxId;

    private CharacterHealth health;
    private GameObject healVfxObject;

    private void Start()
    {
        health = GetComponent<CharacterHealth>();
        Debug.Assert(health);
    }

    private void Update()
    {
        if (health == null) return;

        if (health.IsRecovering)
        {
            if (healVfxObject == null)
            {
                healVfxObject = VFXManager.Instance.SpawnPersistent(healVfxId, transform.position);
                if (healVfxObject != null)
                {
                    foreach (var ps in healVfxObject.GetComponentsInChildren<ParticleSystem>(true))
                    {
                        var main = ps.main;
                        main.loop = true;
                    }
                }
            }
            else
            {
                healVfxObject.transform.position = transform.position;
            }
        }
        else
        {
            if (healVfxObject != null)
            {
                VFXManager.Instance.ReturnToPool(healVfxObject);
                healVfxObject = null;
            }
        }
    }

    private void OnDisable()
    {
        if (healVfxObject != null)
        {
            if (VFXManager.Instance != null)
                VFXManager.Instance.ReturnToPool(healVfxObject);
            healVfxObject = null;
        }
    }
}
