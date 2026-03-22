using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Network;

public class InRoomTeamListUI : MonoBehaviour
{
    [SerializeField] private Transform redContent;
    [SerializeField] private Transform blueContent;
    [SerializeField] private TMP_Text playerLabelPrefab;

    //private RoomManager _rm;
    //private int _myRoomId = -1;

    //private Dictionary<ulong, TMP_Text> spawned = new();

    //private void OnEnable()
    //{
    //    StartCoroutine(BindAndRun());
    //}

    //private IEnumerator BindAndRun()
    //{
    //    while (_rm == null)
    //    {
    //        _rm = RoomManager.Instance;
    //        yield return null;
    //    }

    //    while (!_rm.TryGetMyRoomId(out _myRoomId))
    //        yield return null;

    //    while (isActiveAndEnabled)
    //    {
    //        Refresh();
    //        yield return new WaitForSeconds(0.3f);
    //    }
    //}

    //private void Refresh()
    //{
    //    var alive = new HashSet<ulong>();

    //    // 1) alive 수집
    //    foreach (var m in _rm.GetTeamMappingsSnapshot())
    //    {
    //        if (m.roomId != _myRoomId) continue;
    //        alive.Add(m.playerId);
    //    }

    //    // 2) 없는 플레이어 제거
    //    var toRemove = new List<ulong>();
    //    foreach (var kv in spawned)
    //    {
    //        if (!alive.Contains(kv.Key))
    //            toRemove.Add(kv.Key);
    //    }

    //    foreach (var id in toRemove)
    //    {
    //        Destroy(spawned[id].gameObject);
    //        spawned.Remove(id);
    //    }

    //    // 3) 생성 + (추가) 팀 바뀌면 parent 이동
    //    foreach (var m in _rm.GetTeamMappingsSnapshot())
    //    {
    //        if (m.roomId != _myRoomId) continue;

    //        Transform desiredParent = (m.team == Team.Red) ? redContent : blueContent;

    //        if (!spawned.TryGetValue(m.playerId, out var label))
    //        {
    //            label = Instantiate(playerLabelPrefab, desiredParent);
    //            spawned.Add(m.playerId, label);
    //        }
    //        else
    //        {
    //            // ✅ 핵심: 팀이 바뀌었으면 부모(Content) 이동
    //            if (label.transform.parent != desiredParent)
    //                label.transform.SetParent(desiredParent, false);
    //        }

    //        label.text = $"Player {m.playerId}";
    //    }
    //}
}