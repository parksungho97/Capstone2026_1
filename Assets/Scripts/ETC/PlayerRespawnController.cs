using Fusion;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class PlayerRespawnController : NetworkBehaviour
{
    [Header("Dead Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deadAnimStateName = "Dead";
    [SerializeField] private float deadAnimDuration = 2.0f;
    [SerializeField] private Chased chased;

    private Respawn respawn;
    private PlayerController playerController;
    private CharacterHealth characterHealth;
    private Rigidbody rb;
    private Collider[] colliders;
    private PlayerAnimationSelector playerAnimationSelector;
    private AudioListener audioListener;
    private AudioSource audioSource;
    private NetworkTransform networkTransform;
    private VoicePlayer voicePlayer;
    

    [SerializeField] private GameObject[] childs;
    [SerializeField] private HitComponent hitComponent;

    private Coroutine deadAnimRoutine;
    private Coroutine disableAudioRoutine;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterHealth = GetComponent<CharacterHealth>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
        playerAnimationSelector = GetComponent<PlayerAnimationSelector>();
        respawn = GetComponent<Respawn>();
        audioListener = GetComponent<AudioListener>();
        audioSource =  GetComponent<AudioSource>();
        voicePlayer = GetComponent<VoicePlayer>();

        if (animator == null)
            animator = GetComponent<Animator>();

        Debug.Assert(playerController);
        Debug.Assert(characterHealth);
        Debug.Assert(hitComponent);
        Debug.Assert(rb);
        Debug.Assert(playerAnimationSelector);
        Debug.Assert(respawn);
        Debug.Assert(animator);
        Debug.Assert(audioListener);
        Debug.Assert(audioSource);
        Debug.Assert(voicePlayer);
        Debug.Assert(chased);

        respawn.ActionDead += Dead;
        respawn.ActionRespawnComplete += Respawn;
    }

    public override void Spawned()
    {
        base.Spawned();
        networkTransform = GetComponent<NetworkTransform>();
        Debug.Assert(networkTransform);
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
        {
            playerController.bInputDisabled = true;
        }

        hitComponent.enabled = false;
        rb.isKinematic = true;

        foreach (Collider c in colliders)
            c.enabled = false;

        voicePlayer.StopFootSteps();

        chased.IsActive = false;

        if (disableAudioRoutine != null)
            StopCoroutine(disableAudioRoutine);
        disableAudioRoutine = StartCoroutine(DisableAudioAfterPlaying());

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
        if (disableAudioRoutine != null)
        {
            StopCoroutine(disableAudioRoutine);
            disableAudioRoutine = null;
        }

        if (deadAnimRoutine != null)
        {
            StopCoroutine(deadAnimRoutine);
            deadAnimRoutine = null;
        }

        hitComponent.enabled = true;
        hitComponent.SetInvincible();
        rb.isKinematic = false;

        foreach (Collider c in colliders)
            c.enabled = true;
        
        characterHealth.ResetStat();

        audioSource.enabled = true;

        chased.IsActive = true;

        if (Object.HasStateAuthority)
            networkTransform.Teleport(RespawnManager.Instance.GetRandomSafeRespawnPosition());

        if (Object.HasInputAuthority)
        {
            playerController.bInputDisabled = false;
        }

        Show(true);
    }

    private void Show(bool bShow)
    {
        if (childs.Length > 0)
        {
            foreach (var c in childs)
                c.SetActive(bShow);
        }
    }

    private IEnumerator DisableAudioAfterPlaying()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.enabled = false;
        disableAudioRoutine = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            GetComponent<CharacterHealth>().RPC_ServeHP(50);
    }
}