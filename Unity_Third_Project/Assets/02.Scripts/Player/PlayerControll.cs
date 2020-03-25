using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//클래스는 System.Serializable  이라는 Attribute를 명시해야 인스펙터 뷰에 노출이됩니다.
//Inspector뷰에 노출이됩니다.
[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}

public class PlayerControll : MonoBehaviour
{
    private float v = 0.0f;
    private float h = 0.0f;
    private float r = 0.0f;

    //접근해야하는 컴포넌트는 반드시 변수를 할당하여 사용합니다.
    [SerializeField]
    private Transform playerTr;
    private float moveSpeed = 10.0f;
    //회전속도 변수
    public float rotSpeed = 80.0f;
    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;
    //Animation 컴포넌트를 저장하기 위한 변수
    public Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        playerTr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행합니다.
        anim.clip = playerAnim.idle;
        anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        r = Input.GetAxis("Mouse X");

        //이동방향 벡터 계산입니다.
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //이동계산 = (이동방향 * 이동속도 * Time.dletaTime * 이동기준 좌표)
        playerTr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        //Vector3.up 을 기준으로 rotSpeed만큼의 속도로 회전합니다.
        playerTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);

        //키보드 입력값을 기준으로 동작할 애니메이션을 수행합니다.
        if(v >= 0.1f)
        {
            anim.CrossFade(playerAnim.runF.name, 0.3f);
        }
        else if(v <= -0.1f)
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if(h <= -0.1f)
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        else
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
    }
}
