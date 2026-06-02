using Fusion;
using System.Collections;
using UnityEngine;

public class PlayerRespawnController : NetworkBehaviour
{
    [Header("Dead Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deadAnimStateName = "Dead";
    [SerializeField] private float deadAnimDuration = 2.0f;

    private Respawn respawn;
    private PlayerController playerController;
    private CharacterHealth characterHealth;
    private Rigidbody rb;
    private Collider[] colliders;
    private PlayerAnimationSelector playerAnimationSelector;

    [SerializeField] private GameObject[] childs;
    [SerializeField] private HitComponent hitComponent;

    private Coroutine deadAnimRoutine;

    private bool hasPendingRespawn;
    private Vector3 pendingRespawnPosition;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterHealth = GetComponent<CharacterHealth>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
        playerAnimationSelector = GetComponent<PlayerAnimationSelector>();
        respawn = GetComponent<Respawn>();

        if (animator == null)
            animator = GetComponent<Animator>();

        Debug.Assert(playerController);
        Debug.Assert(characterHealth);
        Debug.Assert(hitComponent);
        Debug.Assert(rb);
        Debug.Assert(playerAnimationSelector);
        Debug.Assert(respawn);
        Debug.Assert(animator);

        respawn.ActionDead += Dead;
        respawn.ActionRespawnComplete += Respawn;
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

        if (deadAnimRoutine != null)
            StopCoroutine(deadAnimRoutine);

        deadAnimRoutine = StartCoroutine(DeadAnimRoutine());
    }

    private IEnumerator DeadAnimRoutine()
    {
        if (animator != null)
        {
            animator.SetLayerWeight(1, 0f);

            animator.ResetTrigger("Hit");
            animator.ResetTrigger("PipeAttack");
            animator.ResetTrigger("PistolAttack");
            animator.ResetTrigger("ShotGunAttack");

            animator.Play(deadAnimStateName, 0, 0f);
        }

        yield return new WaitForSeconds(deadAnimDuration);

        Show(false);

        deadAnimRoutine = null;
    }

    public void Respawn()
    {
        RPC_Respawn();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Respawn()
    {
        Debug.Log("Rpc_Respawn");

        if (deadAnimRoutine != null)
        {
            StopCoroutine(deadAnimRoutine);
            deadAnimRoutine = null;
        }

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

    private void Show(bool bShow)
    {
        if (childs.Length > 0)
        {
            foreach (var c in childs)
                c.SetActive(bShow);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            GetComponent<CharacterHealth>().RPC_ServeHP(50);
    }
}