using Fusion;
using UnityEngine;

namespace Network
{
    public class MyNetworkRoot : MonoBehaviour
    {
        // 1. 싱글톤 인스턴스
        private static MyNetworkRoot _instance;
        public static MyNetworkRoot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MyNetworkRoot>();
                }
                return _instance;
            }
        }

        public int GetLocalPlayerId() { return Runner.LocalPlayer.PlayerId; }

        public NetworkRunner Runner { get; private set; }

        public NetworkSceneManagerDefault SceneManagerDefault { get; private set; }
        private void Awake()
        {
            // --- 싱글톤 중복 방지 (C++의 정적 멤버 초기화와 유사) ---
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            // --- 씬 전환 시 파괴 방지 (핵심!) ---
            DontDestroyOnLoad(gameObject);

            if (Runner != null) return;

            // 런너 컴포넌트를 동적으로 붙입니다.
            Runner = gameObject.AddComponent<NetworkRunner>();

            // 런너 자체도 씬 전환 시 파괴되지 않도록 설정 (Fusion 내부 로직으로도 보장되지만 명시적 처리)
            DontDestroyOnLoad(Runner.gameObject);

            Debug.Log("[MyNetworkRoot] 전역 NetworkRunner가 성공적으로 생성되었습니다.");

            SceneManagerDefault = gameObject.AddComponent<NetworkSceneManagerDefault>();

            DontDestroyOnLoad(SceneManagerDefault);
        }
    }
}