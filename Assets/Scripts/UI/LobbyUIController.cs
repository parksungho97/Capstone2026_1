using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class LobbyUIController : MonoBehaviour
    {
        [Header("Create")]
        [SerializeField] private TMP_InputField roomNameInput = null;
        [SerializeField] private Button createButton = null;
        [SerializeField] private uint maxPlayers = 6;

        [Header("Join (outside ScrollView)")]
        [SerializeField] private Button joinButton = null;
        [SerializeField] private TMP_Text selectedRoomText = null; // 선택 표시용(선택)

        [Header("Room List UI")]
        [SerializeField] private GameObject roomListPanel = null;     // 방 있으면 켜짐
        [SerializeField] private Transform listRoot = null;           // ScrollView/Viewport/Content
        [SerializeField] private RoomListItem roomItemPrefab = null;
        [SerializeField] private GameObject emptyHint = null;

        private readonly List<RoomListItem> _spawned = new();
        private RoomManager _roomManager;

        private bool _hasSelection = false;
        private RoomKey _selectedKey;

        private void Awake()
        {
            if (roomListPanel) roomListPanel.SetActive(true);
            if (emptyHint) emptyHint.SetActive(true);

            if (createButton)
            {
                createButton.onClick.RemoveAllListeners();
                createButton.onClick.AddListener(OnClickCreate);
            }

            if (joinButton)
            {
                joinButton.onClick.RemoveAllListeners();
                joinButton.onClick.AddListener(OnClickJoin);
                joinButton.interactable = false; // 선택 전엔 비활성
            }

            if (selectedRoomText) selectedRoomText.text = "";
        }

        private void OnEnable()
        {
            StartCoroutine(BindRoomManagerWhenReady());
        }

        private void OnDisable()
        {
            if (_roomManager != null)
                _roomManager.ActionRoomListChanged -= OnRoomListChanged;
            _roomManager = null;
        }

        private IEnumerator BindRoomManagerWhenReady()
        {
            while (_roomManager == null)
            {
                _roomManager = RoomManager.Instance;
                if (_roomManager != null) break;
                yield return null;
            }

            _roomManager.ActionRoomListChanged -= OnRoomListChanged;
            _roomManager.ActionRoomListChanged += OnRoomListChanged;

            // ✅ 구독 직후 1회 강제 갱신(초기 상태 반영)
            RefreshRoomList();
        }

        public void OnClickCreate()
        {
            Debug.Log("[LobbyUI] Create clicked");

            if (_roomManager == null) _roomManager = RoomManager.Instance;
            if (_roomManager == null) return;

            string roomName = "room";
            if (roomNameInput && !string.IsNullOrWhiteSpace(roomNameInput.text))
                roomName = roomNameInput.text.Trim();

            _roomManager.MakeRoomServerRpc(roomName, maxPlayers);
        }

        private void OnClickJoin()
        {
            if (!_hasSelection) return;
            if (_roomManager == null) _roomManager = RoomManager.Instance;
            if (_roomManager == null) return;

            Debug.Log($"[LobbyUI] Join clicked roomId={_selectedKey.roomId}");
            _roomManager.EnterRoomServerRpc(_selectedKey);
        }

        private void OnRoomListChanged(RoomKey _)
        {
            RefreshRoomList();
        }

        // RoomListItem에서 호출
        public void SelectRoom(Room room)
        {
            _selectedKey = new RoomKey(room.roomId);
            _hasSelection = true;

            if (joinButton) joinButton.interactable = true;
            if (selectedRoomText) selectedRoomText.text = room.roomName.ToString();

            foreach (var item in _spawned)
                if (item) item.SetSelected(item.RoomId == room.roomId);
        }

        private void RefreshRoomList()
        {
            if (_roomManager == null) _roomManager = RoomManager.Instance;
            if (_roomManager == null) return;

            for (int i = 0; i < _spawned.Count; i++)
                if (_spawned[i]) Destroy(_spawned[i].gameObject);
            _spawned.Clear();

            int count = _roomManager.GetRoomCount();
            bool hasRooms = count > 0;

            if (roomListPanel) roomListPanel.SetActive(true);
            if (emptyHint) emptyHint.SetActive(!hasRooms);

            if (!hasRooms)
            {
                _hasSelection = false;
                if (joinButton) joinButton.interactable = false;
                if (selectedRoomText) selectedRoomText.text = "";
                return;
            }

            for (int i = 0; i < count; i++)
            {
                Room room = _roomManager.GetRoomAt(i);
                var item = Instantiate(roomItemPrefab, listRoot);
                item.Bind(this, room);
                _spawned.Add(item);
            }
        }
    }
}