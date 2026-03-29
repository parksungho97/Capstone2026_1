using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jjh
{
    // 1. 시야배열 클래스
    // 2. 플레이어 fov로 시야배열 계산
    // 3. 계산하기전에 벽 체크해서 플레이어 시야 시작부터 맨 끝점까지를 구하고 그거에 맞는 배열인덱싱해서 표시
    // 4. 가려져있는 덮개를 표시하는 방식으로 구현
    public class MyPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += new Vector3(0f, 0f, 1f) * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += new Vector3(0f, 0f, -1f) * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += new Vector3(-1f, 0f, 0f) * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += new Vector3(1f, 0f, 0f) * moveSpeed * Time.deltaTime;
            }

            RotateTowardsMouse();
        }

        private void RotateTowardsMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPos = ray.GetPoint(distance);
                Vector3 direction = mouseWorldPos - transform.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

}
