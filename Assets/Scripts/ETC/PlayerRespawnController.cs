using Fusion;
using UnityEngine;

public class PlayerRespawnController : NetworkBehaviour
{
    private Respawn respawn;
    private PlayerController playerController;
    private CharacterHealth characterHealth;
    private Rigidbody rb;
    private Collider[] colliders;
    private Renderer[] renderers;
    private PlayerAnimationSelector playerAnimationSelector;
    [SerializeField] private HitComponent hitComponent;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterHealth = GetComponent<CharacterHealth>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
        playerAnimationSelector = GetComponent<PlayerAnimationSelector>();
        respawn = GetComponent<Respawn>();

        Debug.Assert(playerController);
        Debug.Assert(characterHealth);
        Debug.Assert(hitComponent);
        Debug.Assert(rb);
        Debug.Assert(playerAnimationSelector);
        Debug.Assert(respawn);

        respawn.ActionDead += Dead;
        respawn.ActionRespawnComplete += Respawn;
    }

    public void Dead()
    {
        playerController.bInputDisabled = true;
        hitComponent.enabled = false;
        rb.isKinematic = true;

        foreach (Collider c in colliders)
            c.enabled = false;

        Show(false);
    }

    public void Respawn()
    {
        playerController.bInputDisabled = false;
        hitComponent.enabled = true;
        rb.isKinematic = false;

        foreach (Collider c in colliders)
            c.enabled = true;

        Show(true);
        characterHealth.RPC_ResetStat();

        if (Object.HasInputAuthority)
        {
            pendingRespawnPosition = RespawnManager.Instance.GetRandomSafeRespawnPosition();
            hasPendingRespawn = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (hasPendingRespawn)
        {
            transform.position = pendingRespawnPosition;
            hasPendingRespawn = false;
        }
    }

    private bool hasPendingRespawn;
    private Vector3 pendingRespawnPosition;

    private void Show(bool bShow)
    {
        foreach (Renderer r in renderers)
            r.enabled = bShow;
    }
}
