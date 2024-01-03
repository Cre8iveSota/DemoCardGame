using System;
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

        // If there is a shield card on the opponent's field, it cannot attack the opponent's hero directly.
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards();
        if (Array.Exists(enemyFieldCards, card => card.model.ability == ABILITY.SHEILD))
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
