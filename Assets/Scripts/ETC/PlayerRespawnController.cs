using Fusion;
using UnityEngine;

public class PlayerRespawnController : NetworkBehaviour
{
    private Respawn respawn;
    private PlayerController playerController;
    private CharacterHealth characterHealth;
    private Rigidbody rb;
    private Collider[] colliders;
    private PlayerAnimationSelector playerAnimationSelector;
    [SerializeField] private GameObject[] childs;
    [SerializeField] private HitComponent hitComponent;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterHealth = GetComponent<CharacterHealth>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
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

        playerAnimationSelector.ActionDeadAnimEnd += () =>
        {
            Show(false);
        };
    }

    public void Dead()
    {
        RPC_Dead();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Dead()
    {
        Debug.Log("Rpc_Dead");
        if (Object.HasInputAuthority)
            playerController.bInputDisabled = true;

        hitComponent.enabled = false;
        rb.isKinematic = true;

        foreach (Collider c in colliders)
            c.enabled = false;
    }

    public void Respawn()
    {
        RPC_Respawn();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Respawn()
    {
        Debug.Log("Rpc_Respawn");
        hitComponent.enabled = true;
        rb.isKinematic = false;

        foreach (Collider c in colliders)
            c.enabled = true;

        characterHealth.ResetStat();

        if (Object.HasInputAuthority)
        {
            playerController.bInputDisabled = false;
            pendingRespawnPosition = RespawnManager.Instance.GetRandomSafeRespawnPosition();
            hasPendingRespawn = true;
        }

        Show(true);
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
        if (childs.Length > 0)
            foreach (var c in childs)
                c.SetActive(bShow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            GetComponent<CharacterHealth>().RPC_ServeHP(50);
    }
}
