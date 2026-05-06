using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class Npc : NetworkBehaviour
{
    [SerializeField] private VoiceClipManager voiceClipManager;
    private NavMeshAgent _agent;
    private Vector3 _centerPos;
    public float wanderRadius = 10f;

    public override void Spawned()
    {
        _agent = GetComponent<NavMeshAgent>();
        _centerPos = transform.position;
        SetRandomDestination();

        audioSource = GetComponent<AudioSource>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // 목적지 도착하면 다음 포인트
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            SetRandomDestination();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            var clip = voiceClipManager.GetRandomClipOrNull();
            if (clip != null)
                audioSource.PlayOneShot(clip);
        }
    }

    private void SetRandomDestination()
    {
        NavMeshHit hit;
        Vector3 randomPoint = _centerPos + Random.insideUnitSphere * wanderRadius;
        randomPoint.y = _centerPos.y;

        if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }

    private AudioSource audioSource;
}