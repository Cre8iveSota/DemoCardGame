using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour, IDropHandler
{
    public enum TYPE
    {
        Hand,
        Field,
    }
    public TYPE type;
    public void OnDrop(PointerEventData eventData)
    {
        if (type == TYPE.Hand) return;

        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card != null)
        {
            if (!card.cardMovement.isDragable) return;
            if (card.IsSpell)
            {
                return;
            }

            // this.transformでこのコンポーネントを持つ場所に設定してあげることで、ドラッグ中のカードの親を指定する
            card.cardMovement.defaultParent = this.transform;
            if (card.model.isFieldCard)
            {
                return;
            }
            card.OnField();
        }
    }
}
