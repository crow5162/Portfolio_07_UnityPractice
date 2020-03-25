using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    //셰이크 효과를 줄 카메라의 Transform 을 저장할 변수
    public Transform shakeCamera;
    //회전시킬것인지를 판단할 변수
    public bool shakeRotate = false;


    //초기 좌푯값과 회전값을 저장할 변수
    private Vector3 originPos;
    private Quaternion originRot;

    // Start is called before the first frame update
    void Start()
    {
        //카메라의 초깃값을 저장 합니다.
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    public IEnumerator ShakeCameraCo(float duration = 0.05f,
                                    float magnitudePos = 0.03f,
                                    float magnitudeRot = 0.1f)
    {
        //지나간 시간을 누적할 변수
        float passTime = 0.0f;
    
        //진동시간동안 루프를 순환시킵니다.
        while(passTime < duration)
        {
            //불규칙한 위치를 산출합니다
            Vector3 shakePos = Random.insideUnitSphere;

            //카메라의 위치를 변경합니다.
            shakeCamera.localPosition = shakePos * magnitudePos;

            //불규칙한 회전을 사용할경우
            if(shakeRotate)
            {
                //불규칙한 회전값을 펄린 노이즈 함수를 이용해 추출합니다.
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));

                //카메라의 회전값을 변경합니다.
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }

            //진동 시간을 누적합니다.
            passTime += Time.deltaTime;


            yield return null;
        }

        //진동이 끝난후 카메라의 초기값으로 설정합니다.
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
