using Fusion;
using System;
using System.Linq;
using UnityEngine;

public enum ERequestType : byte
{
    Red,
    Blue,
    End
}

public enum ERequestResultType : byte
{
    None,
    RedOnly,
    BlueOnly,
    Both,
}

public enum EActivateStateType : byte
{
    None,
    Red,
    Blue,
    Both,
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
    public int Id { get; private set;  }
}


public class Activater : NetworkBehaviour
{
    public uint GetGreaterProgress()
    {
        return RedProgress > BlueProgress ? RedProgress : BlueProgress;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StartActivateRpc(ObjectId id, ERequestType requestType)
    {
        Debug.Assert(requestType != ERequestType.End);
        
        if(requestType == ERequestType.Red)
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
        //ActivateRequests.Set((int)requestType, ActivateRequests[(int)requestType] + 1);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StopActivateRpc(ObjectId id)
    {
        if (RedActivateRequests.ContainsKey(id.Id))
            RedActivateRequests.Remove(id.Id);
        else if (BlueActivateRequests.ContainsKey(id.Id))
            BlueActivateRequests.Remove(id.Id);
        else
            return;
        Debug.Log("StopActivateRpc Success");
        //if (ActivateRequests[(int)requestType] > 0)
        //    ActivateRequests.Set((int)requestType, ActivateRequests[(int)requestType] - 1);
    }

    public EActivateSuccessType IsActivatePossible()
    {
        return ActivateSuccessType;
    }

    public uint GetRedProgress()
    {
        return RedProgress;
    }

    public uint GetBlueProgress()
    {
        return BlueProgress;
    }

    public void ClearState()
    {
        if (!Object.HasStateAuthority)
            return;

        RedActivateRequests.Clear();
        BlueActivateRequests.Clear();

        RedProgress = 0;
        BlueProgress = 0;

        ActivateType = EActivateStateType.None;
        ActivateSuccessType = EActivateSuccessType.Notyet;
        TimeProgress = 0.0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        ERequestResultType requestResult = ERequestResultType.None;

        // 원래 구조 (2인 이상 필요)
        // bool isRedEnough = mActivateRequests[(int)ERequestType.Red] >= 2;
        // bool isBlueEnough = mActivateRequests[(int)ERequestType.Blue] >= 2;

        // 테스트용 (1인만 눌러도 점령)
        bool isRedEnough = RedActivateRequests.Count > 0;
        bool isBlueEnough = BlueActivateRequests.Count > 0;

        if (isRedEnough && isBlueEnough)
            requestResult = ERequestResultType.Both;
        else if (isRedEnough)
            requestResult = ERequestResultType.RedOnly;
        else if (isBlueEnough)
            requestResult = ERequestResultType.BlueOnly;

        switch (ActivateType)
        {
            case EActivateStateType.None:
                UpdateNoneState(requestResult);
                break;

            case EActivateStateType.Red:
                UpdateRedState(requestResult);
                break;

            case EActivateStateType.Blue:
                UpdateBlueState(requestResult);
                break;

            case EActivateStateType.Both:
                UpdateBothState(requestResult);
                break;
            default:
                Debug.Assert(false);
                break;
        }

        if (RedProgress == 100)
            ActivateSuccessType = EActivateSuccessType.Red;
        else if (BlueProgress == 100)
            ActivateSuccessType = EActivateSuccessType.Blue;
        else
            ActivateSuccessType = EActivateSuccessType.Notyet;
    }

    private void UpdateNoneState(ERequestResultType requestResultType)
    {
        switch (requestResultType)
        {
            case ERequestResultType.None:
                break;

            case ERequestResultType.RedOnly:
                ChangeState(EActivateStateType.Red);
                break;

            case ERequestResultType.BlueOnly:
                ChangeState(EActivateStateType.Blue);
                break;

            case ERequestResultType.Both:
                ChangeState(EActivateStateType.Both);
                break;
        }
    }

    private void UpdateRedState(ERequestResultType requestResultType)
    {
        switch (requestResultType)
        {
            case ERequestResultType.None:
                uint remainProgress = GetRemainProgress(RedProgress);
                RedProgress = remainProgress;
                ChangeState(EActivateStateType.None);
                break;

            case ERequestResultType.RedOnly:
                uint newProgress = IncreaseProgress(RedProgress);
                RedProgress = newProgress;
                break;

            case ERequestResultType.BlueOnly:
                remainProgress = GetRemainProgress(RedProgress);
                RedProgress = remainProgress;
                ChangeState(EActivateStateType.Blue);
                break;

            case ERequestResultType.Both:
                ChangeState(EActivateStateType.Both);
                break;
        }
    }

    private void UpdateBlueState(ERequestResultType requestResultType)
    {
        switch (requestResultType)
        {
            case ERequestResultType.None:
                uint remainProgress = GetRemainProgress(BlueProgress);
                BlueProgress = remainProgress;
                ChangeState(EActivateStateType.None);
                break;

            case ERequestResultType.RedOnly:
                remainProgress = GetRemainProgress(BlueProgress);
                BlueProgress = remainProgress;
                ChangeState(EActivateStateType.Red);
                break;

            case ERequestResultType.BlueOnly:
                uint newProgress = IncreaseProgress(BlueProgress);
                BlueProgress = newProgress;
                break;

            case ERequestResultType.Both:
                ChangeState(EActivateStateType.Both);
                break;
        }
    }

    private void UpdateBothState(ERequestResultType requestResultType)
    {
        switch (requestResultType)
        {
            case ERequestResultType.None:
                uint remainProgress = GetRemainProgress(RedProgress);
                RedProgress = remainProgress;

                remainProgress = GetRemainProgress(BlueProgress);
                BlueProgress = remainProgress;

                ChangeState(EActivateStateType.None);
                break;

            case ERequestResultType.RedOnly:
                remainProgress = GetRemainProgress(BlueProgress);
                BlueProgress = remainProgress;
                ChangeState(EActivateStateType.Red);
                break;

            case ERequestResultType.BlueOnly:
                remainProgress = GetRemainProgress(RedProgress);
                RedProgress = remainProgress;
                ChangeState(EActivateStateType.Blue);
                break;

            case ERequestResultType.Both:
                break;
        }
    }

    private uint IncreaseProgress(uint progress)
    {
        uint result = progress;

        TimeProgress += Runner.DeltaTime;

        if (TimeProgress >= mTimeThreshold)
        {
            result += 1;
            result = Math.Min(result, 100);
            TimeProgress = 0.0f;
        }

        return result;
    }

    private uint GetRemainProgress(uint progress)
    {
        if (progress >= 99)
            return 99;
        else if (progress >= 66)
            return 66;
        else if (progress >= 33)
            return 33;
        else
            return 0;
    }

    private void ChangeState(EActivateStateType activateStateType)
    {
        ActivateType = activateStateType;
        TimeProgress = 0.0f;
    }

    [Networked]
    private NetworkDictionary<int, int> RedActivateRequests => default;

    [Networked]
    private NetworkDictionary<int, int> BlueActivateRequests => default;

    [Networked]
    private EActivateStateType ActivateType { get; set; }

    [Networked]
    private uint RedProgress { get; set; }

    [Networked]
    private uint BlueProgress { get; set; }

    [SerializeField] private float mTimeThreshold = 0.01f;

    [Networked]
    private float TimeProgress { get; set; }

    [Networked]
    private EActivateSuccessType ActivateSuccessType { get; set; }
}
