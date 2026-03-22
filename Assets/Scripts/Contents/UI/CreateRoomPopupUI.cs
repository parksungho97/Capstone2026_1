using TMPro;
using UnityEngine;

public class CreateRoomPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject popupObject;
    [SerializeField] private TMP_InputField roomNameInputField;

    public void OpenPopup()
    {
        popupObject.SetActive(true);
        roomNameInputField.text = "";
        roomNameInputField.ActivateInputField();
    }

    public void ClosePopup()
    {
        popupObject.SetActive(false);
    }

    public void OnClickCreateRoom()
    {
        string roomName = roomNameInputField.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("방 제목이 비어 있습니다.");
            return;
        }

        Debug.Log("방 생성 요청: " + roomName);

        // 여기서 실제 네트워크 담당자가 만든 방 생성 함수 호출
        // 예시:
        // LobbyManager.Instance.CreateRoom(roomName);

        popupObject.SetActive(false);
    }
}