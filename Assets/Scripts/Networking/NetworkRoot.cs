using Fusion;
using Photon.Voice.Unity;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    // 씬 전환을 가로질러 유일하게 살아남는 네트워크 핵심 관리자.
    // 로비 씬의 NetworkRoot GameObject에 NetworkRunner, NetworkSceneManagerDefault를 함께 부착할 것.
    public class NetworkRoot : MonoBehaviour
    {
        public static NetworkRoot Instance { get; private set; }

        public NetworkRunner Runner { get; private set; }
        public NetworkSceneManagerDefault SceneManagerDefault { get; private set; }

        public Recorder Recorder { get; private set; }

        private void Awake()
        {
            // 항상 최신 인스턴스를 사용한다.
            // 이전 인스턴스가 Shutdown(destroyGameObject:true)로 제대로 파괴됐다면 Instance는 null이지만,
            // 비정상 종료 경로로 살아남은 구 인스턴스가 있을 경우 제거하고 새것을 사용한다.
            if (Instance != null && Instance != this)
                Destroy(Instance.gameObject);

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Runner = GetComponent<NetworkRunner>();
            SceneManagerDefault = GetComponent<NetworkSceneManagerDefault>();
            Recorder = GetComponent<Recorder>();

            Debug.Assert(Runner != null, "[NetworkRoot] 같은 GameObject에 NetworkRunner가 필요합니다.");
            Debug.Assert(SceneManagerDefault != null, "[NetworkRoot] 같은 GameObject에 NetworkSceneManagerDefault가 필요합니다.");
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // 세션을 종료하고 씬을 전환한다.
        // destroyGameObject: false → Runner를 파괴하지 않으므로 로비에서 동일한 Runner를 재사용 가능.
        // 이 방식으로 로비로 돌아올 때 Runner가 중복 생성되는 문제를 근본적으로 해결한다.
        public async Task LeaveToScene(int sceneIndex)
        {
            if (Runner != null && Runner.IsRunning)
            {
                if (Runner.IsSharedModeMasterClient && Runner.SessionInfo != null)
                    Runner.SessionInfo.IsOpen = false;

                await Runner.Shutdown(destroyGameObject: true);
            }

            SceneManager.LoadScene(sceneIndex);
        }

        public async Task LeaveToLobby()
        {
            if (Runner != null && Runner.IsRunning)
            {
                if (Runner.IsSharedModeMasterClient && Runner.SessionInfo != null)
                    Runner.SessionInfo.IsOpen = false;

                await Runner.Shutdown(destroyGameObject: true);
            }

            SceneManager.LoadScene(SceneNames.Lobby);
        }
    }
}