using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    // 방(Room) 씬에서만 존재하며 런너의 기능을 추상화하는 매니저
    public class RoomSession : MonoBehaviour
    {
        private NetworkRunner _runner;

        private void Awake()
        {
            _runner = MyNetworkRoot.Instance.Runner;
            Debug.Assert(_runner);

            Debug.Log($"RoomState: {_runner.SessionInfo.Properties["RoomState"]}");
        }


        // 1. 내가 방장(Host)인지 확인
        public bool IsHost => _runner != null && _runner.IsSharedModeMasterClient;

        // 2. 로컬 플레이어 참조
        public PlayerRef LocalPlayer => _runner != null ? _runner.LocalPlayer : PlayerRef.None;

        // 4. 방 나가기 (Shutdown)
        public async void LeaveRoom(int sceneIndex)
        {
            await _runner.Shutdown();
            SceneManager.LoadScene(sceneIndex);
        }

        public void ChangeRoomState(string roomState)
        {
            Debug.Assert(roomState == "Room" || roomState == "InGame");
            Debug.Assert(_runner.SessionInfo.Properties.ContainsKey("RoomState"));

            var newProperties = new Dictionary<string, SessionProperty>();

            newProperties["RoomState"] = roomState;

            _runner.SessionInfo.UpdateCustomProperties(newProperties);
        }
    }
}