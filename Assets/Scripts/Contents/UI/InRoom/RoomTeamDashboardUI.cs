using UnityEngine;

public class RoomTeamDashboardUI : MonoBehaviour
{
    [SerializeField] private Transform redTeamListParent;
    [SerializeField] private Transform blueTeamListParent;
    [SerializeField] private GameObject playerNameItemPrefab;

    public void AddRedPlayer(string playerName)
    {
        GameObject item = Instantiate(playerNameItemPrefab, redTeamListParent);
        PlayerNameItemUI itemUI = item.GetComponent<PlayerNameItemUI>();

        if (itemUI != null)
        {
            itemUI.SetPlayerName(playerName);
        }
    }

    public void AddBluePlayer(string playerName)
    {
        GameObject item = Instantiate(playerNameItemPrefab, blueTeamListParent);
        PlayerNameItemUI itemUI = item.GetComponent<PlayerNameItemUI>();

        if (itemUI != null)
        {
            itemUI.SetPlayerName(playerName);
        }
    }

    public void ClearRedTeam()
    {
        for (int i = redTeamListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(redTeamListParent.GetChild(i).gameObject);
        }
    }

    public void ClearBlueTeam()
    {
        for (int i = blueTeamListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(blueTeamListParent.GetChild(i).gameObject);
        }
    }

    public void ClearAll()
    {
        ClearRedTeam();
        ClearBlueTeam();
    }

    private void Start()
    {
        ClearAll();

        AddRedPlayer("Player 1");
        AddRedPlayer("Player 2");

        AddBluePlayer("Player 3");
        AddBluePlayer("Player 4");
    }
}