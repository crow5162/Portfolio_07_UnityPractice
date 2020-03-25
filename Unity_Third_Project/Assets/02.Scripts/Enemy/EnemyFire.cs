using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class President
{
    public string name;

    President America;
    President SouthKorea;
    President China;
    President Unity;
    President WorldBestPresident;

    President()
    {
        America.name = "Trump";
        SouthKorea.name = "Moon";
        China.name = "SijinPingPinge";
        Unity.name = "MinWook Kim";
        WorldBestPresident = Unity;
    }
}
public class EnemyFire : MonoBehaviour
{
    //Audio Source 컴포넌트를 저장할 변수
    private AudioSource audio;
    //Animator 컴포넌트를 저장할 변수
    private Animator animator;
    //주인공 캐릭터의 Transform 컴포넌트 입니다
    private Transform playerTr;
    //적 캐릭터 (본인)의 Transform 컴포넌트를 저장할 변수
    private Transform enemyTr;
    //애니메이터 컨트롤러에 정의한 파라메터의 해시값을 미리 추출합니다
    private readonly int hashFire = Animator.StringToHash("isFire");
    private readonly int hashReload = Animator.StringToHash("Reload");

    //다음 발사할 시간계산용 변수
    private float nextFire = 0.0f;
    //총알 발사 간격 
    private readonly float fireRate = 0.1f;
    //주인공을 향해 회전할 속도 계수
    private readonly float damping = 10.0f;

    //============================================= Reload 관련 변수
    //재장전 시간
    private readonly float reloadTime = 2.0f;
    //탄창의 최대 총알수
    private readonly int maxBullet = 10;
    //초기 총알 여부
    private int currentBullet = 10;
    //재장전 여부
    private bool isReload = false;
    //재장전시간동안 기다릴 변수
    private WaitForSeconds wsReload;
    //재장전 사운드클립을 저장할 변수
    public AudioClip reloadSfx;
    //==============================================

    //총알 발사여부를 판단할 변수
    public bool isFire = false;
    //총알 발사 사운드를 저장할 변수
    public AudioClip fireSfx;

    //총알의 프리팹 정보 저장 변수
    public GameObject Bullet;
    //총알의 발사위치 정보
    public Transform firePos;

    //MuzzleFlash의 MeshRenderer 컴포넌트를 저장할 변수
    public MeshRenderer muzzleFlash;


    // Start is called before the first frame update
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        wsReload = new WaitForSeconds(reloadTime);
        //MuzzleFlash를 비활성화.
        muzzleFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFire && !isReload)
        {
            if (Time.time >= nextFire)
            {
                Fire();
                //다음 발사시간 계산
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f);
            }

            //주인공이 있는 위치까지의 회전각도 계산.
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            //보간함수를 통해 점진적으로 회전합니다.
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        animator.SetTrigger(hashFire);
        audio.PlayOneShot(fireSfx, 1.0f);

        StartCoroutine(this.ShowMuzzleFlash());

        //총알을 생성합니다.
        GameObject _bullet = Instantiate(Bullet, firePos.position, firePos.rotation);
        //일정시간이 지난뒤 삭제합니다.
        Destroy(_bullet, 3.0f);


        //남은 총알로 재장전여부를 계산합니다.
        isReload = (--currentBullet % maxBullet == 0);
        if(isReload)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        //MuzzleFlash를 비활성화
        muzzleFlash.enabled = false;
        //재장전애니메이션 실행.
        animator.SetTrigger(hashReload);
        //재장전 사운드 발생
        audio.PlayOneShot(reloadSfx, 1.0f);
        //재장전 시간만큼 대기하는동안 제어권 양보
        yield return wsReload;

        //총알의 갯수를 초기화.
        currentBullet = maxBullet;
        isReload = false;
    }

    IEnumerator ShowMuzzleFlash()
    {
        //MeshRenderer 활성화.
        muzzleFlash.enabled = true;

        //불규칙한 회전각도를 계산합니다.
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        //MuzzleFlash를 Z축 방향으로 회전합니다.
        muzzleFlash.transform.localRotation = rot;
        //MuzzleFlash의 스케일을 불규칙하게 변경.
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1.0f, 2.0f);

        //Texture의 offset 속성에 적용할 불규칙한 값을 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        //MuzzleFlash의 머티리얼의 Offset 값을 적용합니다
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        //MuzzleFlash가 잠시 보일동안 대기.
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        //MuzzleFlash를 비활성화
        muzzleFlash.enabled = false;
    }
}
