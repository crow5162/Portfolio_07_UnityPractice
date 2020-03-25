using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //적캐릭터의 상태를 표현하기위한 열거형 변수의 정의
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }
    //상태를 저장할 변수
    public State _state = State.PATROL;
    //주인공의 위치를 저장할 변수
    public Transform playerTr;
    //적 캐릭(본인) 의 위치를 저장할 변수
    private Transform enemyTr;

    //공격 사정거리 
    public float attackDist = 5.0f;
    //추적 사정거리
    public float traceDist = 10.0f;

    //사망여부를 판단할 변수
    public bool isDead;
    //코루틴에서 사용할 지연시간 변수.
    private WaitForSeconds ws;
    //이동을 제어하는 MoveAgent 클래스를 저장할 변수.
    private MoveAgent moveAgent;

    //Animator 컴포넌트를 저장할 변수
    private Animator animator;
    //총알발사를 제어하는 EnemyFire 클래스를 저장할 변수
    private EnemyFire enemyFire;

    //Animator 컨트롤러에 정의한 피라메터의 해시값을 미리 추출합니다
    private readonly int hashMove = Animator.StringToHash("isMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    //랜덤한 애니메이션을 출력할 난수.
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private void Awake()
    {
        //주인공의 게임오브젝트 추출.
        var player = GameObject.FindGameObjectWithTag("PLAYER");

        //주인공의 Tranform Component 추출.
        if(player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }

        //몬스터(본인) 의 Trnasform 컴포넌트 추출.
        enemyTr = GetComponent<Transform>();

        //코루틴의 지연시간 생성합니다.
        ws = new WaitForSeconds(0.3f);
        //이동을 제어하는 MoveAgent "클래스" 추출.
        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        //총알발사를 제어하는 EnemyFire 스크립트를 저장할 변수
        enemyFire = GetComponent<EnemyFire>();

        //Cycle Offset Size의 값을 불규칙하게 변경.
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        //Speed 값을 불규칙하게 변경합니다.
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f));
    }

    private void OnEnable()
    {
        //CheckState Coroutine 함수 실행.
        StartCoroutine(this.ChackState());
        StartCoroutine(this.Action());

        PlayerDamage.OnPlayerDie += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        PlayerDamage.OnPlayerDie -= this.OnPlayerDie;
    }

    IEnumerator ChackState()
    {
        while(!isDead)
        {
            //사망한 상태라면 코루팀 함수 종료
            if (_state == State.DIE) yield break;

            //플레이어와 적 캐릭터간의 거리를 계산합니다
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            //공격 사정권이내인 경우
            if(dist <= attackDist)
            {
                _state = State.ATTACK;
            }
            //추적 사정거리 이내인 경우
            else if (dist <= traceDist)
            {
                _state = State.TRACE;
            }
            //이외엔 정찰을 계속합니다.
            else
            {
                _state = State.PATROL;
            }

            //0.3초 대기 하는동안 제어권을 양보합니다.
            yield return ws;
        }
    }

    IEnumerator Action()
    {
        //적캐릭터가 사망할때까지 무한루프
        while(!isDead)
        {

            yield return ws;

            //상태에 따라 분기 처리
            switch(_state)
            {
                case State.PATROL:
                    //순찰모드를 활성화.
                    moveAgent.isPatrol = true;
                    animator.SetBool(hashMove, true);
                    //총알발사를 정지.
                    enemyFire.isFire = false;
                    break;

                case State.ATTACK:
                    //추적 및 순찰을 정지.
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    //총알 발사 수행
                    if(!enemyFire.isFire)
                    enemyFire.isFire = true;
                    break;

                case State.TRACE:
                    //주인공의 위치를 넘겨 추적보드로 변경.
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    //총알발사를 정지.
                    enemyFire.isFire = false;
                    break;

                case State.DIE:
                    //추적 및 순찰을 정지.
                    moveAgent.Stop();
                    isDead = true;
                    enemyFire.isFire = false;
                    //사망애니메이션의 종류를 지정합니다.
                    animator.SetInteger(hashDieIdx, Random.Range(0, 3));
                    //사망애니메이션을 실행합니다.
                    animator.SetTrigger(hashDie);
                    //Capsule Collider를 비활성화 합니다.
                    GetComponent<CapsuleCollider>().enabled = false;

                    this.gameObject.tag = "Untagged";

                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Speed 피라메터에 이동속도를 전달합니다.
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        //모든 코루틴 함수를 종료합니다.
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }
}
