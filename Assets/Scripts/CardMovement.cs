using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaultParent;
    public bool isDragable;
    public void OnBeginDrag(PointerEventData eventData)
    {
        CardController cardController = GetComponent<CardController>();
        if (cardController.model.isPlayerCard && GameManager.instance.isPlayerTurn && !cardController.model.isFieldCard && cardController.model.cost <= GameManager.instance.playerManaCost)
        {
            isDragable = true;
        }
        else if (cardController.model.isPlayerCard && GameManager.instance.isPlayerTurn && cardController.model.isFieldCard && cardController.model.canAttack)
        {
            isDragable = true;
        }
        else
        {
            isDragable = false;
        }
        if (!isDragable) return;
        defaultParent = transform.parent; // 元のカードのポジションをデフォルト位置(親)にする
        transform.SetParent(defaultParent.parent, false); // カードを移動するために一段階上の親を親に変更する
        GetComponent<CanvasGroup>().blocksRaycasts = false; //ドラッグ中にドラッグしているカードに重なっているしたのの座標を手に入れるため、BlockRaycastをfalseにする。
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragable) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragable) return;
        transform.SetParent(defaultParent, false); // Drop時に元の親に変更する
        GetComponent<CanvasGroup>().blocksRaycasts = true; //移動後にBlockRaycastをtrueにすることで、再度カードをマウスで指定できるようにする。これがないと、二度とカードを指定できなくなる。
    }

    public IEnumerator MoveToFeild(Transform field)
    {
        // Once the Canvas is set as a parent
        transform.SetParent(defaultParent.parent);

        // Dotween moves the card to field 
        transform.DOMove(field.position, 0.25f);

        yield return new WaitForSeconds(0.25f);
        defaultParent = field;
        transform.SetParent(defaultParent);
    }
    public IEnumerator MoveToTarget(Transform target)
    {
        // Get the current coordinates and the order in the current parent element.
        Vector3 currentPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        // Once the Canvas is set as a parent
        transform.SetParent(defaultParent.parent);

        // Dotween moves the card to target 
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);
        transform.SetParent(defaultParent);
        transform.SetSiblingIndex(siblingIndex);
    }

    void Start()
    {
        defaultParent = transform.parent;
    }
}
