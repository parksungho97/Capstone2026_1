using System.Collections.Generic;
using UnityEngine;

public class TeamInfo : MonoBehaviour
{
    public static TeamInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("TeamInfo");
                _instance = go.AddComponent<TeamInfo>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    private static TeamInfo _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Room -> InGame 씬 전환 직전에 RoomEntryPoint가 호출
    public void Set(Dictionary<int, EPlayerTeam> map)
    {
        playerTeamMap = new Dictionary<int, EPlayerTeam>(map);
    }

    public bool TryGetTeam(int playerId, out EPlayerTeam team)
        => playerTeamMap.TryGetValue(playerId, out team);

    public EPlayerTeam GetTeam(int playerId)
    {
        if (playerTeamMap.TryGetValue(playerId, out EPlayerTeam team))
            return team;
        Debug.LogWarning($"TeamInfo: playerId {playerId} not found.");
        return EPlayerTeam.Red;
    }

    public IReadOnlyDictionary<int, EPlayerTeam> All => playerTeamMap;

    private Dictionary<int, EPlayerTeam> playerTeamMap = new Dictionary<int, EPlayerTeam>();
}
