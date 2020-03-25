using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    //폭발효과를 저장할 변수
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "BULLET")
        {
            //Destroy(coll.gameObject);
            ShowEffect(coll);
            //플레이어가 발사한 총알을 지워버리면 NullReference 오류가 발생하니까
            //재사용하기위해 단순히 비활성화 처리해버린다.
            coll.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision coll)
    {
        //충돌지점의 정보를 추출
        ContactPoint contact = coll.contacts[0];

        //법선 벡터가이루는 회전각도를 추출합니다.
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        //스파크 효과를 생성합니다.
        GameObject spark = (GameObject)Instantiate(sparkEffect, contact.point + (-contact.normal * 0.05f), rot);

        spark.transform.SetParent(this.transform);
    }

}
