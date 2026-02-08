using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network
{
    public class NetworkRoot
    {
        public static NetworkRoot Instance
        { 
            get 
            { 
                if(mInstance == null)
                    mInstance = new NetworkRoot();
                return mInstance; 
            } 
        }
        public void Init(string ipAddres = "127.0.0.1", ushort port = 7777)
        {
            Debug.Assert(NetworkManager.Singleton != null, "NetworkManager가 씬에 없습니다!");

            mIpAddres = ipAddres;
            mPort = port;

            mTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (mTransport != null)
                mTransport.SetConnectionData(mIpAddres, mPort);
        }

        // 자신의 클라이언트 id를 반환
        public static ulong GetLocalClientId() => NetworkManager.Singleton.LocalClientId;
        public void StartHost() => NetworkManager.Singleton.StartHost();
        public void StartClient() => NetworkManager.Singleton.StartClient();
        public void Shutdown()
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.Shutdown();
            Debug.Log("NetworkRoot: Shutdown 및 싱글톤 인스턴스가 초기화되었습니다.");
        }
        // 클라이언트가 연결되었을 경우 발생할 이벤트 등록
        public void RegistClientConnected(Action<ulong> onClientConnected)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += onClientConnected;
        }
        // 클라이언트가 연결되었을 경우 발생하던 이벤트 해제
        public void UnregistClientConnected(Action<ulong> onClientConnected)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= onClientConnected;
        }
        // 클라이언트가 연결이 끊겼을 경우 발생할 이벤트 등록
        public void RegistClientDisconnected(Action<ulong> onClientDisconnected)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += onClientDisconnected;
        }
        // 클라이언트가 연결이 끊겼을 경우 발생하던 이벤트 해제
        public void UnregistClientDisconnected(Action<ulong> onClientDisconnected)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= onClientDisconnected;
        }
        private NetworkRoot() { }
        private static NetworkRoot mInstance = null;

        private UnityTransport mTransport = null;
        private string mIpAddres = "127.0.0.1";
        private ushort mPort = 7777;
    }
}