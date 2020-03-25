using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControll : MonoBehaviour
{
    public float damage = 20.0f;
    public float speed = 1000.0f;

    //컴포넌트를 저장할 변수
    private Transform tr;
    private Rigidbody rb;
    private TrailRenderer trail;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }

    //ObjectPool에 추가된 Bullet은 비활성화된 상태로 돌아가기때문에
    //Bullet을 가져와 다시 스크립트가 활성화 되었을때 총알을 발사하는 로직을 다시 수행해야 합니다.
    //따라서 스크립트가 활성화 될 때마다 실행되는 OnEnable 함수로 발사 로직을 옮깁니다.
    private void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
    }

    private void OnDisable()
    {
        //비활성화된 총알의 여러 값들을 초기화 합니다.
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }
}
