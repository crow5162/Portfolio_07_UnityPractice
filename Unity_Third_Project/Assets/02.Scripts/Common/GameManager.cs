using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]
    //적이 출현할 위치를 담을 배열
    public Transform[] points;
    //적 캐릭터 프리팹을 저장할 변수
    public GameObject enemy;
    //적 캐릭터를 생성할 주기
    private float createTime = 2.0f;
    //적 캐릭터의 최대 생성 갯수
    public int maxEnemy = 10;
    //게임 종료 여부를 판단할 변수
    public bool isGameOver = false;

    [Header("Object Pool")]
    //생성할 총알 프리팹
    public GameObject bulletPrefab;
    //오브젝트 풀에 생성할 갯수
    public int maxPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();

    //일시정지 여부를 판단하는 변수
    private bool isPaused;
    //Inventory의 canvasGroup컴포넌트를 저장할변수
    public CanvasGroup inventoryCG;

    //싱글톤에 접근하기 위한 Static 변수 선언
    public static GameManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        //instance에 할당된 클래스의 인스턴스가 다를 경우 새로 생성된 클래스를 의미합니다.
        //두 번째 생성된 GameManager 인스턴스는 삭제합니다
        //최조에 생성된 GameManager만 남게되므로 하나의 클래스가 지속적으로 유지하는것이다.
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }

        //다른씬으로 넘어가도 삭제하지않고 유지합니다.
        DontDestroyOnLoad(this.gameObject);

        //오브젝트 풀링 함수를 호출합니다.
        CreatePooling();
    
    }

    // Start is called before the first frame update
    void Start()
    {
        //처음에 인벤토리를 비활성화
        OnInventoryOpen(false);

        //하이어라키뷰의 SpawnPoint를 찾아 하위에있는 모든 Transform 컴포넌트를 찾아옴
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        if(points.Length >0)
        {
            StartCoroutine(this.CreateEnemy());
        }
    }

    //적 캐릭터를 생성하는 코루틴 함수
    IEnumerator CreateEnemy()
    {
        //게임 종료시까지 무한루프
        while (!isGameOver)
        {
            //현재 생성된 적 캐릭터의 갯수 산출
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;

            //적 캐릭터의 최대 생성 갯수보다 작을 때만 적 캐릭터를 생성
            if(enemyCount < maxEnemy)
            {
                //적 캐릭터의 생성주기 시간만큼 대기
                yield return new WaitForSeconds(createTime);

                //불규칙적인 위치 산출
                int idx = Random.Range(1, points.Length);

                //적 캐릭터의 동적 생성
                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    //오브젝트풀에서 사용가능한 총알을 가져오는 함수
    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }

        return null;
    }

    //오브젝트 풀에 총알을 생성하는 함수
    public void CreatePooling()
    {
        //총알을 생성해 차일드화할 페어런트 게임오브젝트를 생성
        GameObject objectPools = new GameObject("ObjectPool");

        //풀링 갯수만큼 미리 총알을 생성
        for(int i =0;i<maxPool;i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            obj.name = "Bullet_" + i.ToString();
            //비활성화
            obj.SetActive(false);
            //리스트에 생성한 총알을 추가합니다.
            bulletPool.Add(obj);
        }
    }

    //일시 정지 버튼 클릭 시 호출할 함수
    public void OnPauseClick()
    {
        //일시 정지 값을 토글 시킴.
        isPaused = !isPaused;

        //TimeScale이 0이면 정지, 1이면 정상 속도.
        Time.timeScale = (isPaused) ? 0.0f : 1.0f;

        //주인공 객체를 추출합니다.
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공 캐릭터에 추가도니 모든 스크립트를 추출합니다.
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        //주인공 캐릭터의 모든 스크립트를 활성화/비활성화
        foreach(var script in scripts)
        {
            script.enabled = !isPaused;
        }

        var canvasGroup = GameObject.Find("Panel - Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    //인벤토리를 활성화/비활성화하는 함수
    public void OnInventoryOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }
}
