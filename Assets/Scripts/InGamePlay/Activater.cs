using System;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
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

    public class Activater : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void RegistActivateServerRpc(ERequestType requestType, ServerRpcParams serverRpcParams = default)
        {
            RegistActivateOnServer(requestType);
        }

        [ServerRpc(RequireOwnership = false)]
        public void UnregistActivateServerRpc(ERequestType requestType, ServerRpcParams serverRpcParams = default)
        {
            UnregistActivateOnServer(requestType);
        }

        public void RegistActivateOnServer(ERequestType requestType)
        {
            if (!IsServer)
                return;

            Debug.Assert(ERequestType.Red <= requestType && requestType < ERequestType.End);
            mActivateRequests[(int)requestType] += 1;
        }

        public void UnregistActivateOnServer(ERequestType requestType)
        {
            if (!IsServer)
                return;

            Debug.Assert(ERequestType.Red <= requestType && requestType < ERequestType.End);

            if (mActivateRequests[(int)requestType] > 0)
                mActivateRequests[(int)requestType] -= 1;
        }

        public EActivateSuccessType IsActivatePossible()
        {
            return mActivateSuccessType;
        }

        public uint GetRedProgress()
        {
            return mRedProgress.Value;
        }

        public uint GetBlueProgress()
        {
            return mBlueProgress.Value;
        }

        public void ClearState()
        {
            mActivateRequests[0] = 0;
            mActivateRequests[1] = 0;

            // 진행도는 유지 (100 → 0 초기화 방지)
            // mRedProgress.Value = 0;
            // mBlueProgress.Value = 0;

            mActivateType.Value = EActivateStateType.None;
            mActivateSuccessType = EActivateSuccessType.Notyet;
            mTimeProgress = 0.0f;
        }

        private void Update()
        {
            if (!IsServer)
                return;

            ERequestResultType requestResult = ERequestResultType.None;

            // 원래 구조 (2인 이상 필요)
            // bool isRedEnough = mActivateRequests[(int)ERequestType.Red] >= 2;
            // bool isBlueEnough = mActivateRequests[(int)ERequestType.Blue] >= 2;

            // 테스트용 (1인만 눌러도 점령)
            bool isRedEnough = mActivateRequests[(int)ERequestType.Red] > 0;
            bool isBlueEnough = mActivateRequests[(int)ERequestType.Blue] > 0;

            if (isRedEnough && isBlueEnough)
                requestResult = ERequestResultType.Both;
            else if (isRedEnough)
                requestResult = ERequestResultType.RedOnly;
            else if (isBlueEnough)
                requestResult = ERequestResultType.BlueOnly;

            switch (mActivateType.Value)
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
            }

            if (mRedProgress.Value == 100)
                mActivateSuccessType = EActivateSuccessType.Red;
            else if (mBlueProgress.Value == 100)
                mActivateSuccessType = EActivateSuccessType.Blue;
            else
                mActivateSuccessType = EActivateSuccessType.Notyet;

            // 진행도 로그 (변화 있을 때만 출력)
            if (mLastDebugRed != mRedProgress.Value || mLastDebugBlue != mBlueProgress.Value)
            {
                mLastDebugRed = mRedProgress.Value;
                mLastDebugBlue = mBlueProgress.Value;

                Debug.Log($"[{gameObject.name}] Progress  Red:{mRedProgress.Value}%  Blue:{mBlueProgress.Value}%");
            }
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
                    uint remainProgress = GetRemainProgress(mRedProgress.Value);
                    mRedProgress.Value = remainProgress;
                    ChangeState(EActivateStateType.None);
                    break;

                case ERequestResultType.RedOnly:
                    uint newProgress = IncreaseProgress(mRedProgress.Value);
                    mRedProgress.Value = newProgress;
                    break;

                case ERequestResultType.BlueOnly:
                    remainProgress = GetRemainProgress(mRedProgress.Value);
                    mRedProgress.Value = remainProgress;
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
                    uint remainProgress = GetRemainProgress(mBlueProgress.Value);
                    mBlueProgress.Value = remainProgress;
                    ChangeState(EActivateStateType.None);
                    break;

                case ERequestResultType.RedOnly:
                    remainProgress = GetRemainProgress(mBlueProgress.Value);
                    mBlueProgress.Value = remainProgress;
                    ChangeState(EActivateStateType.Red);
                    break;

                case ERequestResultType.BlueOnly:
                    uint newProgress = IncreaseProgress(mBlueProgress.Value);
                    mBlueProgress.Value = newProgress;
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
                    uint remainProgress = GetRemainProgress(mRedProgress.Value);
                    mRedProgress.Value = remainProgress;

                    remainProgress = GetRemainProgress(mBlueProgress.Value);
                    mBlueProgress.Value = remainProgress;

                    ChangeState(EActivateStateType.None);
                    break;

                case ERequestResultType.RedOnly:
                    remainProgress = GetRemainProgress(mBlueProgress.Value);
                    mBlueProgress.Value = remainProgress;
                    ChangeState(EActivateStateType.Red);
                    break;

                case ERequestResultType.BlueOnly:
                    remainProgress = GetRemainProgress(mRedProgress.Value);
                    mRedProgress.Value = remainProgress;
                    ChangeState(EActivateStateType.Blue);
                    break;

                case ERequestResultType.Both:
                    break;
            }
        }

        private uint IncreaseProgress(uint progress)
        {
            uint result = progress;

            mTimeProgress += Time.deltaTime;

            if (mTimeProgress >= mTimeThreshold)
            {
                result += 1;
                result = Math.Min(result, 100);
                mTimeProgress = 0.0f;
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
            mActivateType.Value = activateStateType;
            mTimeProgress = 0.0f;
        }

        private uint[] mActivateRequests = new uint[(int)ERequestType.End];

        private NetworkVariable<EActivateStateType> mActivateType =
            new NetworkVariable<EActivateStateType>(
                EActivateStateType.None,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

        private NetworkVariable<uint> mRedProgress =
            new NetworkVariable<uint>(
                0,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

        private NetworkVariable<uint> mBlueProgress =
            new NetworkVariable<uint>(
                0,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

        private uint mLastDebugRed = 0;
        private uint mLastDebugBlue = 0;

        [SerializeField] private float mTimeThreshold = 0.01f;
        private float mTimeProgress = 0.0f;

        private EActivateSuccessType mActivateSuccessType = EActivateSuccessType.Notyet;
    }
}