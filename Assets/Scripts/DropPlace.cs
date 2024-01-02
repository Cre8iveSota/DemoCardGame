using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        CardMovement cardMovement = eventData.pointerDrag.GetComponent<CardMovement>();
        if (cardMovement != null)
        {
            // this.transformでこのコンポーネントを持つ場所に設定してあげることで、ドラッグ中のカードの親を指定する
            cardMovement.defaultParent = this.transform;
        }
    }
}
