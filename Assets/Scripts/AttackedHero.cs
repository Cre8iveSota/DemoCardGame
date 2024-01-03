using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // To choose attacker
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        if (attacker == null)
        {
            return;
        }
        if (attacker.model.canAttack)
        {
            // Make attacker attack to Hero
            GameManager.instance.AttackToHero(attacker, true);
            GameManager.instance.CheckHeroHP();
        }
    }
}
