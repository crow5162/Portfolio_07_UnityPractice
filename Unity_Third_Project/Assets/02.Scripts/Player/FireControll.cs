using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
//총알 발사와 재장전 오디오 클립을 저장할 구조체.
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireControll : MonoBehaviour
{
    //무기타입 
    public enum WeaponType
    {
        RIFLE=0,
        SHITGUN
    }
    //주인공이 현재 들고있는 무기를 저장할 변수
    public WeaponType currentWeapon = WeaponType.RIFLE;

    //총알 프리팹 불러오기
    public GameObject _bullet;
    //총알 발사지점 세팅.
    public Transform _firePos;

    public ParticleSystem cartridge;
    private ParticleSystem _muzzleFlash;

    //AudioSorce를 저장할 변수
    private AudioSource _audio;
    //Audio Clip을 저장할 변수
    public PlayerSfx playerSfx;

    //Shake 클래스를 저장할 변수
    private ShakeCamera shake;

    //탄창이미지 UI
    public Image magazineImg;
    //남은 총알 수 Text Ui
    public Text magazineText;

    //최대 총알 수
    public int maxBullet = 10;
    //남은 총알 수 
    public int remainingBullet = 10;

    //재장전 시간
    private float reloadBullet = 2.0f;
    //재장전 여부를 판단할 변수
    private bool isReloading = false;

    //변경할 무기 이미지
    public Sprite[] weaponIcons;
    //교체할 무기 이미지 UI
    public Image weaponImage;

    // Start is called before the first frame update
    void Start()
    {
        _muzzleFlash = _firePos.GetComponentInChildren<ParticleSystem>();
        //AudioSource 컴포넌트를 저장할 변수
        _audio = GetComponent<AudioSource>();
        //Shake 스크립트를 추출.
        shake = GameObject.Find("CameraRig").GetComponent<ShakeCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(_firePos.position, _firePos.forward * 20.0f, Color.green);

        if (EventSystem.current.IsPointerOverGameObject()) return;

        if(Input.GetMouseButtonDown(0) && !isReloading)
        {

            //총알 수 를 하나감소
            --remainingBullet;
            Fire();

            if (remainingBullet == 0)
            {
                StartCoroutine(this.Reloading());
            }
        }
    }

    void Fire()
    {
        //카메라 쉐이크 코루틴 호출.
        StartCoroutine(shake.ShakeCameraCo());
        //bullet 프리팹을 동적으로 생성
        //Instantiate(_bullet, _firePos.position, _firePos.rotation);

        //Instantiate로 생성하는 방식이 아닌 ObjectPool 에서 가져오는 방식으로 전환.
        var _bullet = GameManager.instance.GetBullet();
        if(_bullet != null)
        {
            _bullet.transform.position = _firePos.position;
            _bullet.transform.rotation = _firePos.rotation;
            _bullet.SetActive(true);
        }

        cartridge.Play();
        _muzzleFlash.Play();
        FireSfx();

        //재장전 이미지의 fillAmount 속성값 지정
        magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
        //남은 총알 수 계산
        UpdateBulletText();
    }

    void FireSfx()
    {
        //현재 장착중인 무기의 오디오 클립을 가져옵니다.
        var _sfx = playerSfx.fire[(int)currentWeapon];
        _audio.PlayOneShot(_sfx, 1.0f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currentWeapon], 0.3f);

        //재장전 오디오의 길이 +0.3초 동안 대기합니다.
        yield return new WaitForSeconds(playerSfx.reload[(int)currentWeapon].length + 0.3f);

        //각종 변수값의 초기화
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;
        //남은 총알 수 갱신
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        //(남은 총알 수 / 최대 총알 수 )표시
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet,
            maxBullet);
    }

    public void OnChangeWeapon()
    {
        currentWeapon = (WeaponType)((int)++currentWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currentWeapon];
    }
}
