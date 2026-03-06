using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class ResultManager : MonoBehaviour
    {
        private void Start()
        {
            if (RoomManager.Instance == null)
                return;

            GameWinner winner = RoomManager.Instance.WinnerTeam;

            switch (winner)
            {
                case GameWinner.Red:
                    Debug.Log("RESULT : RED TEAM WIN");
                    break;

                case GameWinner.Blue:
                    Debug.Log("RESULT : BLUE TEAM WIN");
                    break;

                default:
                    Debug.Log("RESULT : UNKNOWN");
                    break;
            }
        }
    }
}