using UnityEngine;

public class ResultUIController : MonoBehaviour
{
    [Header("Result UI")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject defeatScreen;

    private void Awake()
    {
        HideAll();
    }

    /// <summary>
    /// 승리 결과를 표시합니다.
    /// 다른 스크립트에서 승리로 판단했을 때 호출합니다.
    /// </summary>
    public void ShowVictory()
    {
        if (victoryScreen == null)
        {
            Debug.LogError("[ResultUIController] Victory Screen이 연결되지 않았습니다.");
            return;
        }

        if (defeatScreen != null)
            defeatScreen.SetActive(false);

        victoryScreen.SetActive(true);

        Debug.Log("[ResultUIController] 승리 화면 표시");
    }

    /// <summary>
    /// 패배 결과를 표시합니다.
    /// 다른 스크립트에서 패배로 판단했을 때 호출합니다.
    /// </summary>
    public void ShowDefeat()
    {
        if (defeatScreen == null)
        {
            Debug.LogError("[ResultUIController] Defeat Screen이 연결되지 않았습니다.");
            return;
        }

        if (victoryScreen != null)
            victoryScreen.SetActive(false);

        defeatScreen.SetActive(true);

        Debug.Log("[ResultUIController] 패배 화면 표시");
    }

    /// <summary>
    /// 모든 결과 화면을 숨깁니다.
    /// 초기화 또는 결과 화면 종료 시 사용할 수 있습니다.
    /// </summary>
    public void HideAll()
    {
        if (victoryScreen != null)
            victoryScreen.SetActive(false);

        if (defeatScreen != null)
            defeatScreen.SetActive(false);
    }

    /// <summary>
    /// bool 값으로 결과를 전달할 수 있는 편의 함수입니다.
    /// true면 승리, false면 패배 화면을 표시합니다.
    /// </summary>
    public void ShowResult(bool isVictory)
    {
        if (isVictory)
            ShowVictory();
        else
            ShowDefeat();
    }
}
