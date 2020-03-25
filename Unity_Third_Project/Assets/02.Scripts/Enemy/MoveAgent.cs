using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//네이게이션 기능을 사용하기 위해 추가해야하는 nameSpace
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class MoveAgent : MonoBehaviour
{
    //순찰지점들을 저장하기 위한 List 타입의 변수
    public List<Transform> wayPoints;

    //다음 순찰 지점의 배열의 Index
    public int nextIdx;

    //네비게이션 컴포넌트를 저장할 변수.
    private NavMeshAgent agent;
    //적 캐릭터(본인)의 Transform 컴포넌트를 저장할변수
    private Transform enemyTr;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;

    // 회전할 때의 속도를 조절하는 게수
    private float damping = 1.0f;

    //순찰여부를 판단하는 변수
    private bool _isPatrol;
    //Patrol 프로퍼티를 정의합니다 (getter, setter)

    public bool isPatrol
    {
        get { return _isPatrol; }
        set
        {
            _isPatrol = value;
            if(_isPatrol)
            {
                agent.speed = patrolSpeed;
                //정찰 상태의 회전계수
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    //추적대상의 위치를 저장하는 변수
    private Vector3 _traceTarget;
    //_traveTarget 프로퍼티를 정의합니다.(getter, setter)
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            //추적 상태의 회전계수.
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }

    //NavMeshAgent의 이동속도에 대한 프로퍼티 정의(getter)
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //적 캐릭터(본인) 의 Transform 컴포넌트를 추출.
        enemyTr = GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();
        //목적지에 가까워질수록 속도를 줄이는 옵션을 비활성화.
        agent.autoBraking = false;
        //자도응로 회전하는 기능을   비활성화.
        agent.updateRotation = false;

        agent.speed = patrolSpeed;

        //하이어라키뷰의 WatPoints Group 게임오브젝트를 추출합니다.
        var group = GameObject.Find("WayPointGroup");

        if (group != null)
        {
            //WayPointGroup 하위에 있는 모든 Transform 컴포넌트를 추출한 후 
            //List 타입의 WayPoints 배열에 추가합니다.
            group.GetComponentsInChildren<Transform>(wayPoints);
            //배열의 첫번째 항목을 삭제합니다.
            wayPoints.RemoveAt(0);

            //첫번째로 이동할 위치를 불규칙하게 추출합니다.
            nextIdx = Random.Range(0, wayPoints.Count);
        }

        MoveWayPoint();
    }

    void MoveWayPoint()
    {
        //최단거리 경로 계산이 끝나지않았다면 다음을 수행하지않습니다.
        if (agent.isPathStale) return;

        //다음목적지를 waypoints 배열에서 추출한 위치로 다음 목적지를 지정합니다.
        agent.destination = wayPoints[nextIdx].position;
        //네비게이션 기능을 활성화 해서 이동을 시작합니다.
        agent.isStopped = false;
    }

    //주인공을 추적할때 이동시키는 함수.
    void TraceTarget(Vector3 pos)
    {
        //경로가 유효하지않으면 다음을 수행하지않음
        if (agent.isPathStale) return;

        agent.destination = pos;
        //네이게이션 기능을 활성화 하고 이동을 시작함
        agent.isStopped = false;
    }

    //순찰 및 추적을 정지시키는 함수.
    public void Stop()
    {
        agent.isStopped = true;
        //바로정지하기 위해 속도를 0으로 설정
        agent.velocity = Vector3.zero;
        _isPatrol = false;
    }

    // Update is called once per frame
    void Update()
    {
        //적캐릭터가이동중일때만회전합니다.
        if(!agent.isStopped)
        {
            //NavMeshAgent가 가야할 방향 벡터를 쿼터니언 타입의 각도로 변환합니다.
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            //보간 함수를 사용해 점진적으로 회전시킵니다.
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        //순찰모드가 아닐경우 이 로직을 수행하지않습니다.
        if (!_isPatrol) return;

        //NavMeshAgent가 이동하고 있고 목적지에 도착했는지 여부를계산합니다.
        if(agent.velocity.sqrMagnitude >= 0.2f * 0.2f &&
            agent.remainingDistance <= 0.5f)
        {
            //다음 목적지의 배열첨자를 계산합니다.
            //nextIdx = ++nextIdx % wayPoints.Count;
            //랜덤한 목적지를 산출합니다.
            nextIdx = Random.Range(0, wayPoints.Count);
            //다음목적지로의 이동을 시작합니다.
            MoveWayPoint();
        }
    }
}
