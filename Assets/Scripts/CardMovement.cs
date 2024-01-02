using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaultParent;
    public void OnBeginDrag(PointerEventData eventData)
    {
        defaultParent = transform.parent; // 元のカードのポジションをデフォルト位置(親)にする
        transform.SetParent(defaultParent.parent, false); // カードを移動するために一段階上の親を親に変更する
        GetComponent<CanvasGroup>().blocksRaycasts = false; //ドラッグ中にドラッグしているカードに重なっているしたのの座標を手に入れるため、BlockRaycastをfalseにする。
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(defaultParent, false); // Drop時に元の親に変更する
        GetComponent<CanvasGroup>().blocksRaycasts = true; //移動後にBlockRaycastをtrueにすることで、再度カードをマウスで指定できるようにする。これがないと、二度とカードを指定できなくなる。
    }
    public void SetCardTransform(Transform parentTransform)
    {
        transform.SetParent(parentTransform);
    }
}
