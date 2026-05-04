using Fusion;
using System;
using UnityEngine;

public enum ERequestType : byte
{
    Red,
    Blue,
    End
}

public enum EActivateSuccessType : byte
{
    Notyet,
    Red,
    Blue,
}

public struct ObjectId : INetworkStruct
{
    public ObjectId(GameObject gameObject)
    {
        Id = gameObject.GetInstanceID();
    }
    public int Id { get; private set; }
}

public class Activater : NetworkBehaviour
{
    public uint GetGreaterProgress()
    {
        return RedState.Progress > BlueState.Progress ? RedState.Progress : BlueState.Progress;
    }

    public ERequestType GetGreaterTeam()
    {
        return RedState.Progress > BlueState.Progress ? ERequestType.Red : ERequestType.Blue;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StartActivateRpc(ObjectId id, ERequestType requestType)
    {
        Debug.Assert(requestType != ERequestType.End);

        if (requestType == ERequestType.Red)
        {
            if (RedActivateRequests.ContainsKey(id.Id))
                return;
            RedActivateRequests.Add(id.Id, id.Id);
        }
        else
        {
            if (BlueActivateRequests.ContainsKey(id.Id))
                return;
            BlueActivateRequests.Add(id.Id, id.Id);
        }
        Debug.Log("StartActivateRpc Success");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StopActivateRpc(ObjectId id)
    {
        if (RedActivateRequests.ContainsKey(id.Id))
            RedActivateRequests.Remove(id.Id);
        else if (BlueActivateRequests.ContainsKey(id.Id))
            BlueActivateRequests.Remove(id.Id);
    }

    public EActivateSuccessType IsActivatePossible()
    {
        return ActivateSuccessType;
    }

    public uint GetRedProgress()
    {
        return RedState.Progress;
    }

    public uint GetBlueProgress()
    {
        return BlueState.Progress;
    }

    public void ClearState()
    {
        if (!Object.HasStateAuthority)
            return;

        RedActivateRequests.Clear();
        BlueActivateRequests.Clear();

        RedState = default;
        BlueState = default;

        ActivateSuccessType = EActivateSuccessType.Notyet;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        bool redActive = RedActivateRequests.Count > 0;
        bool blueActive = BlueActivateRequests.Count > 0;
        bool isStandoff = redActive && blueActive;

        TeamActivateState red = RedState;
        TeamActivateState blue = BlueState;

        float dt = Runner.DeltaTime;

        if (isStandoff)
        {
            // 양 팀 대치 중: 모든 타이머 동결
            red = decayController.OnStandoff(red);
            blue = decayController.OnStandoff(blue);
        }
        else
        {
            // Red 처리
            if (redActive)
                red = ActivateProgressHelper.Increase(decayController.OnActive(red), dt, mTimeThreshold);
            else
                red = decayController.ProcessIdle(red, dt);

            // Blue 처리
            if (blueActive)
                blue = ActivateProgressHelper.Increase(decayController.OnActive(blue), dt, mTimeThreshold);
            else
                blue = decayController.ProcessIdle(blue, dt);
        }

        RedState = red;
        BlueState = blue;

        if (RedState.Progress >= 100)
            ActivateSuccessType = EActivateSuccessType.Red;
        else if (BlueState.Progress >= 100)
            ActivateSuccessType = EActivateSuccessType.Blue;
        else
            ActivateSuccessType = EActivateSuccessType.Notyet;
    }

    private void Awake()
    {
        decayController = new ActivateDecayController(mDecayDelay, mTimeThreshold);
    }

    [Networked]
    private NetworkDictionary<int, int> RedActivateRequests => default;

    [Networked]
    private NetworkDictionary<int, int> BlueActivateRequests => default;

    [Networked]
    private TeamActivateState RedState { get; set; }

    [Networked]
    private TeamActivateState BlueState { get; set; }

    [Networked]
    private EActivateSuccessType ActivateSuccessType { get; set; }

    [SerializeField] private float mTimeThreshold = 0.1f;
    [SerializeField] private float mDecayDelay = 3f;

    private ActivateDecayController decayController;
}
