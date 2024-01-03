using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellDropManager : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // To choose attacker
        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        // To choose Defender
        CardController target = GetComponent<CardController>();
        if (spellCard == null)
        {
            return;
        }
        if (spellCard.CanUseSpell())
        {
            spellCard.UseSpellTo(target);
        }
    }
}
