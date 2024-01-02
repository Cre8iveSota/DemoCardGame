using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // To choose attacker
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // To choose Defender
        CardController defender = GetComponent<CardController>();
        if (attacker == null || defender == null)
        {
            return;
        }
        if (attacker.model.canAttack)
        {
            // Make attacker and defender battle
            GameManager.instance.CardsButtle(attacker, defender);
        }
    }
}
