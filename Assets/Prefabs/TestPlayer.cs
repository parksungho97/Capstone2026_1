using Fusion;
using UnityEngine;

public class TestNetworkPlayer : NetworkBehaviour
{
    // 🏃‍♂️ 네트워크 이동 속도 (인스펙터 조절 가능)
    [SerializeField] private float moveSpeed = 5.0f;

    /// <summary>
    /// 유니티 Update 대신, 퓨전의 고정 네트워크 틱(Tick)마다 실행됩니다.
    /// 호스트와 클라이언트 모두에서 실행되며, 예측(Prediction) 연산을 수행합니다.
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        // 🚨 [매우 중요] 이 캐릭터의 "주인(조작 권한자)" 컴퓨터에서만 키보드 입력을 처리합니다.
        // 이 가드가 없으면 다른 사람 캐릭터까지 내가 동시에 조작하게 됩니다.
        if (Object.HasInputAuthority == false) return;

        Vector3 moveDir = Vector3.zero;

        // ⌨️ 오직 내 화면에서만 WASD 키 입력을 직접 체크해서 방향을 누적합니다.
        if (Input.GetKey(KeyCode.W)) moveDir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDir += Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveDir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDir += Vector3.right;

        // 아무 키나 눌려서 움직여야 하는 상황이라면!
        if (moveDir != Vector3.zero)
        {
            // 대각선 속도 뻥튀기 방지
            moveDir = moveDir.normalized;

            // 🎯 [퓨전 전용 물리 공식]
            // 1. 일반 Time.deltaTime 대신 반드시 "Runner.DeltaTime"을 써야 네트워크 틱과 싱크가 맞습니다.
            // 2. 이렇게 transform.position을 변경하면, 퓨전의 NetworkTransform 컴포넌트가 
            //    이 좌표를 전 세계 다른 클라들에게 알아서 복사(동기화)해 줍니다.
            transform.position += moveDir * moveSpeed * Runner.DeltaTime;
        }
    }
}