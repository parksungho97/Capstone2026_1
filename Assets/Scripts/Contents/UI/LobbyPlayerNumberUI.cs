using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerNumberUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private void Reset()
    {
        label = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(RefreshWhenReady());
    }

    private IEnumerator RefreshWhenReady()
    {
        // NetworkManager 준비될 때까지 대기
        while (NetworkManager.Singleton == null)
            yield return null;

        // 클라이언트로 접속 완료될 때까지 대기
        while (!NetworkManager.Singleton.IsClient)
            yield return null;

        UpdateLabel();

        // (선택) 연결/재연결 상황에서도 갱신하고 싶으면 콜백 등록
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong _)
    {
        // 연결 완료 시점에 UI 갱신
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        ulong id = NetworkManager.Singleton.LocalClientId;
        if (label != null)
            label.text = $"Player {id}";
    }
}