using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameResultManager : NetworkBehaviour
    {
        [SerializeField] private CapturePoint mCapturePointA = null;
        [SerializeField] private CapturePoint mCapturePointB = null;
        [SerializeField] private CapturePoint mCapturePointC = null;

        private bool mIsGameEnded = false;

        private void Update()
        {
            if (!IsServer)
                return;

            if (mIsGameEnded)
                return;

            if (mCapturePointA == null || mCapturePointB == null || mCapturePointC == null)
                return;

            ECaptureState stateA = mCapturePointA.GetCaptureState();
            ECaptureState stateB = mCapturePointB.GetCaptureState();
            ECaptureState stateC = mCapturePointC.GetCaptureState();

            if (stateA == ECaptureState.Red &&
                stateB == ECaptureState.Red &&
                stateC == ECaptureState.Red)
            {
                mIsGameEnded = true;

                if (RoomManager.Instance != null)
                    RoomManager.Instance.SetWinnerTeam(GameWinner.Red);

                Debug.Log("[GameResult] RED TEAM WIN");

                NetworkManager.Singleton.SceneManager.LoadScene(
                    SceneNames.Result,
                    LoadSceneMode.Single
                );
            }
            else if (stateA == ECaptureState.Blue &&
                     stateB == ECaptureState.Blue &&
                     stateC == ECaptureState.Blue)
            {
                mIsGameEnded = true;

                if (RoomManager.Instance != null)
                    RoomManager.Instance.SetWinnerTeam(GameWinner.Blue);

                Debug.Log("[GameResult] BLUE TEAM WIN");

                NetworkManager.Singleton.SceneManager.LoadScene(
                    SceneNames.Result,
                    LoadSceneMode.Single
                );
            }
        }
    }
}