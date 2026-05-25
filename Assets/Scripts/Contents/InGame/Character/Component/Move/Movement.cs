using Fusion;
using UnityEngine;

// 1. ������ �ֱ⿡ ���߱� ���� NetworkBehaviour�� �����մϴ�.
public class Movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 15.0f;

    // 2. �ٸ� �÷��̾� ȭ�鿡���� �� ĳ���Ͱ� �Ȱ� �ִ��� �� �� �ֵ��� [Networked]�� �ٿ��ݴϴ�.
    // ���� "�� ȭ��"������ üũ�ϸ� �ȴٸ� �׳� �Ϲ� bool�� �μŵ� ������, 
    // ��Ƽ�÷��̾� �ִϸ��̼� ����ȭ�� ���� ��Ʈ��ũ ������ ����� ���� ���� ��õ�մϴ�.
    [Networked] public bool bMove { get; private set; }

    public bool bMovePossible { get; set; } = true;

    public Vector3 MoveDirection => mPendingMove;
    public Vector3 ViewDirection => mPendingLookDir;

    private Vector3 mPendingMove;
    private Vector3 mPendingLookDir;
    private float mPendingTurnCoeff;

    public void Move(Vector3 degree)
    {
        mPendingMove += degree;
    }

    public void RotateTo(Vector3 lookDir, float coeff = 1.0f)
    {
        mPendingLookDir = lookDir;
        mPendingTurnCoeff = coeff;
    }

    public void SetMovePossible(bool possible)
    {
        bMovePossible = possible;
    }

    // 3. LateUpdate�� ������ �����, ���� ���� ������Ʈ�� FixedUpdateNetwork�� �����մϴ�.
    public override void FixedUpdateNetwork()
    {
        // ȣ��Ʈ�� Ŭ���̾�Ʈ ��� �������� �ʱ�ȭ
        bMove = false;

        if (bMovePossible && mPendingMove != Vector3.zero)
        {
            // [�߿�] ����Ƽ�� Time.deltaTime ��� ������ Runner.DeltaTime�� ��� 
            // ��Ʈ��ũ �������� �и��ų� �ѹ�� �� ���� ����� ��Ȯ�����ϴ�.
            transform.position += mPendingMove * moveSpeed * Runner.DeltaTime;
            bMove = true;
        }
        mPendingMove = Vector3.zero;


        if (bMovePossible && mPendingLookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(mPendingLookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSpeed * mPendingTurnCoeff * Runner.DeltaTime // ȸ������ Runner.DeltaTime ����
            );
        }
        mPendingLookDir = Vector3.zero;
        mPendingTurnCoeff = 0f;
    }
}