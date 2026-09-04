using TMPro;
using UnityEngine;

public class NicknamePopupUI : MonoBehaviour
{
    [Header("팝업 오브젝트")]
    [SerializeField] private GameObject changeNamePopup;

    [Header("입력창")]
    [SerializeField] private TMP_InputField nicknameInputField;

    [Header("플레이어 이름 텍스트")]
    [SerializeField] private TMP_Text playerNameText;

    public void OpenPopup()
    {
        changeNamePopup.SetActive(true);

        // 기존 이름을 입력창에 미리 넣어줌
        nicknameInputField.text = playerNameText.text;

        // 입력창 선택
        nicknameInputField.ActivateInputField();
    }

    public void ClosePopup()
    {
        changeNamePopup.SetActive(false);
    }

    public void ConfirmNickname()
    {
        string newName = nicknameInputField.text.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            Debug.Log("닉네임이 비어 있습니다.");
            return;
        }

        playerNameText.text = newName;
        changeNamePopup.SetActive(false);
    }
}