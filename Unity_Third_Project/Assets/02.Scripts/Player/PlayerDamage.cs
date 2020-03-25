using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    private float initHp = 100.0f;
    public float currentHp;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    //Bloody Screen 을 저장하기 위한 변수
    public Image bloodScreen;

    //Hp Bar Image를 저장하기 위한 변수
    public Image hpBar;

    //생명 게이지의 처음 상태(녹색)
    //Vector4 (R, G, B, Alpha);
    private readonly Color initColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    private Color currentColor;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = initHp;

        //생명게이지의 초기 색상을 설정합니다.
        hpBar.color = initColor;
        currentColor = initColor;
    }

    //충돌한 Collider의 isTrigger의 옵션이 체크됐을때 발생합니다.
    private void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider의 태그가 Bullet이면 Player의 currentHp를 차감합니다.
        if(coll.tag == bulletTag)
        {
            currentHp -= 5.0f;
            Destroy(coll.gameObject);

            Debug.Log("Player HP : " + currentHp.ToString());
            StartCoroutine(this.ShowBloodScreen());

            //생명게이지 크기 및 색상변경 함수 호출합니다.
            //피격될때 마다 호출이 되게해서 현재 체력의 정보를 불러옵니다.
            DisplayHpbar();

            //플레이어이 HP가 0 이라면 사망처리

            if(currentHp <= 0)
            {
                PlayerDie();
            }
        }
    }

    //플레이어의 사망처리 루틴.
    void PlayerDie()
    {
        //Delegate 함수를 사용하지않고 직접 함수를 호출하는 방식.

        //Debug.Log("Player Die !!");

        //ENEMY Tag로 지정된 모든적 캐릭터를 추출해 배열에 저장합니다.
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        //배열의 처음부터 끝까지 순회하면서 적 캐릭터의 OnPlayerDie 함수를 호출합니다.
        //for(int i =0;i<enemies.Length;i++)
        //{
        //    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        //Delegate 함수를 이용해서 메세지를 뿌리는 방식.
        OnPlayerDie();
        GameManager.instance.isGameOver = true;
    }

    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스쳐의 알파값을 불규칙 하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);

        //BloodScreen 텍스쳐의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
    }

    void DisplayHpbar()
    {
        //생명 수치가 50% 일 때까지는 녹색에서 노란색으로 변경합니다.
        if((currentHp / initHp) > 0.5f)
        {
            currentColor.r = (1 - (currentHp / initHp)) * 2.0f;
        }
        else //생명수치가 0%일때는 노란색에서 빨간색으로 변경합니다.
        {
            currentColor.g = (currentHp - initHp) * 2.0f;
        }

        //HpBar의 색상 변경
        hpBar.color = currentColor;
        //Hpbar의 크기를 변경합ㄴ디ㅏ
        hpBar.fillAmount = (currentHp / initHp);
    }
}
