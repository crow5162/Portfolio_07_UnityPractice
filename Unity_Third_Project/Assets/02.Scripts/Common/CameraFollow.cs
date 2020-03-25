using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                //추적할 대상.
    public float moveDamping = 15.0f;       //이동속도 계수
    public float rotateDamping = 10.0f;     //회전속도 계수
    public float distance = 5.0f;           //추적 대상과의 거리
    public float height = 4.0f;             //추적 대상과의 높이.
    public float targetOffset = 2.0f;       //추적 좌표의 오프셋.
    private Transform _tr;

    // Start is called before the first frame update
    void Start()
    {
        //CameraRig의 Transform 컴포넌트를 추출
        _tr = GetComponent<Transform>();
    }

    //주인공 캐릭터의 로직이 완료된 후 처리하기위해 LateUpdate에서 구현합니다.
    private void LateUpdate()
    {
        //카메라 높이와 거리를 계산합니다
        var camPos = target.position -
            (target.forward * distance) +
            (target.up * height);

        //이동할 때의 속도계수를 적용합니다.
        _tr.position = Vector3.Slerp(_tr.position,
            camPos,
            Time.deltaTime * moveDamping);

        //회전할 때의 속도계수를 적용합니다.
        _tr.rotation = Quaternion.Slerp(_tr.rotation,
            target.rotation,
            Time.deltaTime * rotateDamping);

        //카메라를 추적 대상으로 Z축을 회전시킵니다.
        _tr.LookAt(target.position + (target.up * targetOffset));
    }

    //추적할 좌표를 시각적으로 표현합니다.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //추적 및 시야를 맞출 위치를 표시
        Gizmos.DrawWireSphere(target.position +
            (target.up * targetOffset), 0.1f);

        //메인카메라와 추적지점간의 선을 표시
        Gizmos.DrawLine(target.position + (target.up * targetOffset),
            transform.position);

    }
}
