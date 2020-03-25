using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelControll : MonoBehaviour
{
    //폭발효과 프리팹을 저장할 변수
    public GameObject expEffect;
    //총알이 맞은 횟수
    private int hitCount = 0;
    //RigidBody를 저장할 변수
    private Rigidbody rbody;
    //Mesh 저장할 배열
    public Mesh[] meshes;

    //Texture를 저장할 배열
    public Texture[] textures;

    //Mesh Filter 컴포넌트를 적용할 변수
    private MeshFilter meshFilter;

    //MeshRenderer 에 접근할 변수
    private MeshRenderer _renderer;

    //폭발 반경
    private float expRadius = 10.0f;

    //AudioSource 컴포넌트를 저장할 변수
    private AudioSource _audio;

    //폭발 효과음 오디오 클립 연결
    public AudioClip expClip;

    private ShakeCamera shake;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        //MeshRenderer 컴포넌트를 추출해 저장합니다.
        _renderer = GetComponent<MeshRenderer>();

        //난수를 발생싴켜 불규칙적인 텍스쳐를 적용합니다.
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];

        _audio = GetComponent<AudioSource>();

        //Shake스크립트를 추출합니다.
        shake = GameObject.Find("CameraRig").GetComponent<ShakeCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "BULLET")
        {
            if(++hitCount == 3)
            {
                //Distroy(this.gameObject);
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        //폭발효과 프리팹을 동적으로 생성합니다.
        GameObject effect = (GameObject)Instantiate(expEffect, transform.position, Quaternion.identity);
        //RigidBody의 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 합니다.
        //rbody.mass = 1.0f;
        //위로 솟구치는 힘을 가합니다.
        //rbody.AddForce(Vector3.up * 1000.0f);
        //동적생성 이펙트 삭제
        Destroy(effect, 2.0f);

        IndirectDamage(transform.position);

        //난수를 발생
        int idx = Random.Range(0, meshes.Length);

        meshFilter.sharedMesh = meshes[idx];

        //오디오 소스 컴포넌트에서 클립에 연결시켜준 효과음을 재생시킵니다.
        _audio.PlayOneShot(expClip, 1.0f);
        _audio.volume = 5.0f;

        //셰이크 효과 연출
        StartCoroutine(shake.ShakeCameraCo(0.1f, 0.2f, 0.5f));
    }

    //폭발력을 주변에 전달하는 함수.
    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 11);

        foreach(var coll in colls)
        {
            //폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트를 검출합니다.
            var _rb = coll.GetComponent<Rigidbody>();

            //검출된 드럼통의 질량을 가볍게 합니다.
            _rb.mass = 1.0f;
            //폭발력을 전달합니다
            _rb.AddExplosionForce(1200.0f, pos, expRadius, 1000.0f);
        }
    }
}
