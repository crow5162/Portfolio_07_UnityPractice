using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Transform itemTr;
    private Transform inventoryTransform;
    private Transform itemListTr;
    private CanvasGroup canvasGroup;

    public static GameObject draggingItem = null;

    // Start is called before the first frame update
    void Start()
    {
        itemTr = GetComponent<Transform>();
        inventoryTransform = GameObject.Find("Inventory").GetComponent<Transform>();
        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();

        //Canvas Group 컴포넌트 추출
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //드래그 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        //드래그 이벤트가 발생하면 아이템의 위치를 마우스의 커서의 위치로 변경.
        itemTr.position = Input.mousePosition;
    }

    //드래그를 시작할 때 한번만 호출되는 함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        //부모를 Inventory로 변경
        this.transform.SetParent(inventoryTransform);
        //드래그가 시작되면 드래그 되는 아이템의 정보를 저장합니다.
        draggingItem = this.gameObject;
        //드래그가 시작되면 다른 UI이벤트를 받지 않도록 설정.
        canvasGroup.blocksRaycasts = false;
    }

    //드래그가 종료 되었을때 한 번 호출되는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그가 종료되면 드래그 아이템을 null 로 변환
        draggingItem = null;
        //드래그가 종료되면 다른 UI이벤트를 받도록 설정합니다.
        canvasGroup.blocksRaycasts = true;

        //슬롯에 드래그 하지않았을때 원래대로 ItemList로 되돌린다.
        if(itemTr.parent == inventoryTransform)
        {
            itemTr.SetParent(itemListTr.transform);
        }
    }

}
