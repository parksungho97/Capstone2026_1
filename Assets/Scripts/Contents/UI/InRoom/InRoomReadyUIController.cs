using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class InRoomReadyUIController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button readyButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button redTeamButton;
        [SerializeField] private Button blueTeamButton;

        [SerializeField] private TMP_Text readyButtonText;

        //    private Coroutine _waitCoroutine;

        //    private void OnEnable()
        //    {
        //        if (readyButton != null) readyButton.onClick.AddListener(OnReadyClicked);
        //        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
        //        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        //        if (redTeamButton != null) redTeamButton.onClick.AddListener(OnRedTeamClicked);
        //        if (blueTeamButton != null) blueTeamButton.onClick.AddListener(OnBlueTeamClicked);

        //        if (RoomManager.Instance != null)
        //        {
        //            RoomManager.Instance.ActionRoomReadyChanged += OnAnyRoomChanged;
        //            RoomManager.Instance.ActionRoomMembersChanged += OnAnyRoomChanged;
        //        }

        //        _waitCoroutine = StartCoroutine(CoWaitMappingThenRefresh());
        //    }

        //    private void OnDisable()
        //    {
        //        if (readyButton != null) readyButton.onClick.RemoveListener(OnReadyClicked);
        //        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitClicked);
        //        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
        //        if (redTeamButton != null) redTeamButton.onClick.RemoveListener(OnRedTeamClicked);
        //        if (blueTeamButton != null) blueTeamButton.onClick.RemoveListener(OnBlueTeamClicked);

        //        if (RoomManager.Instance != null)
        //        {
        //            RoomManager.Instance.ActionRoomReadyChanged -= OnAnyRoomChanged;
        //            RoomManager.Instance.ActionRoomMembersChanged -= OnAnyRoomChanged;
        //        }

        //        if (_waitCoroutine != null)
        //        {
        //            StopCoroutine(_waitCoroutine);
        //            _waitCoroutine = null;
        //        }
        //    }

        //    private IEnumerator CoWaitMappingThenRefresh()
        //    {
        //        float timeout = 5f;
        //        float t = 0f;

        //        while (t < timeout)
        //        {
        //            if (RoomManager.Instance != null && RoomManager.Instance.TryGetMyRoomKey(out _))
        //                break;

        //            SetWaitingUI();
        //            t += Time.unscaledDeltaTime;
        //            yield return null;
        //        }

        //        RefreshUI();
        //    }

        //    private void OnAnyRoomChanged(int changedRoomId)
        //    {
        //        if (RoomManager.Instance == null) return;
        //        if (!RoomManager.Instance.TryGetMyRoomId(out int myRoomId)) return;
        //        if (myRoomId != changedRoomId) return;

        //        RefreshUI();
        //    }

        //    private void SetWaitingUI()
        //    {
        //        if (exitButton != null)
        //        {
        //            exitButton.gameObject.SetActive(true);
        //            exitButton.interactable = true;
        //        }

        //        if (readyButton != null)
        //        {
        //            readyButton.gameObject.SetActive(true);
        //            readyButton.interactable = false;
        //        }

        //        if (startButton != null)
        //        {
        //            startButton.gameObject.SetActive(false);
        //            startButton.interactable = false;
        //        }

        //        if (readyButtonText != null) readyButtonText.text = "Loading...";
        //    }

        //    private void RefreshUI()
        //    {
        //        var rm = RoomManager.Instance;
        //        if (rm == null)
        //        {
        //            SetWaitingUI();
        //            return;
        //        }

        //        if (!rm.TryGetMyRoomKey(out RoomKey key))
        //        {
        //            SetWaitingUI();
        //            return;
        //        }

        //        int roomId = key.roomId;

        //        bool isRoomHost = rm.IsLocalRoomHost(roomId);
        //        bool allNonHostReady = rm.AreAllNonHostReady(roomId);

        //        rm.TryGetMyReady(roomId, out bool myReady);

        //        if (exitButton != null)
        //        {
        //            exitButton.gameObject.SetActive(true);
        //            exitButton.interactable = true;
        //        }

        //        if (isRoomHost)
        //        {
        //            if (readyButton != null) readyButton.gameObject.SetActive(false);

        //            if (startButton != null)
        //            {
        //                startButton.gameObject.SetActive(true);
        //                startButton.interactable = allNonHostReady;
        //            }
        //        }
        //        else
        //        {
        //            if (startButton != null) startButton.gameObject.SetActive(false);

        //            if (readyButton != null)
        //            {
        //                readyButton.gameObject.SetActive(true);
        //                readyButton.interactable = true;
        //            }

        //            if (readyButtonText != null)
        //                readyButtonText.text = myReady ? "Cancel" : "Ready";
        //        }
        //    }

        //    private void OnReadyClicked()
        //    {
        //        var rm = RoomManager.Instance;
        //        if (rm == null) return;

        //        if (!rm.TryGetMyRoomKey(out RoomKey key)) return;

        //        rm.TryGetMyReady(key.roomId, out bool current);
        //        bool next = !current;

        //        rm.SetReadyServerRpc(key, next);
        //        RefreshUI();
        //    }

        //    private void OnExitClicked()
        //    {
        //        var rm = RoomManager.Instance;
        //        if (rm == null) return;

        //        rm.LeaveRoomAndGoLobbyServerRpc();
        //    }

        //    private void OnStartClicked()
        //    {
        //        var rm = RoomManager.Instance;
        //        if (rm == null) return;

        //        if (!rm.TryGetMyRoomKey(out RoomKey key)) return;

        //        rm.StartGameServerRpc(key);
        //    }

        //    private void OnRedTeamClicked()
        //    {
        //        var rm = RoomManager.Instance;
        //        if (rm == null) return;

        //        if (!rm.TryGetMyRoomKey(out RoomKey key)) return;

        //        rm.ChangeTeamServerRpc(key, Team.Red);
        //    }

        //    private void OnBlueTeamClicked()
        //    {
        //        var rm = RoomManager.Instance;
        //        if (rm == null) return;

        //        if (!rm.TryGetMyRoomKey(out RoomKey key)) return;

        //        rm.ChangeTeamServerRpc(key, Team.Blue);
        //    }
        //}
    }
}