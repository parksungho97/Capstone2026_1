using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    // 방(Room) 씬에서만 존재하며 런너의 기능을 추상화하는 매니저
    public class RoomSession : MonoBehaviour
    {
        private static RoomSession _instance;
        public static RoomSession Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 씬 내에서 검색 (DontDestroy를 안 쓰므로 씬마다 새로 찾아야 함)
                    _instance = FindObjectOfType<RoomSession>();

                    if (_instance == null)
                        Debug.LogError("[RoomSession] 씬에 RoomSession 객체가 없습니다!");
                }
                return _instance;
            }
        }

        private NetworkRunner _runner;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            _runner = MyNetworkRoot.Instance.Runner;
        }


        // 1. 내가 방장(Host)인지 확인
        public bool IsHost => _runner != null && _runner.IsSharedModeMasterClient;

        // 2. 로컬 플레이어 참조
        public PlayerRef LocalPlayer => _runner != null ? _runner.LocalPlayer : PlayerRef.None;

        // 4. 방 나가기 (Shutdown)
        public async void LeaveRoom(int sceneIndex)
        {
            if (_runner != null)
            {
                await _runner.Shutdown();
                SceneManager.LoadScene(sceneIndex);
            }
        }

        private void OnDestroy()
        {
            // 씬이 파괴될 때 스태틱 참조 해제 (메모리 누수 방지)
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}