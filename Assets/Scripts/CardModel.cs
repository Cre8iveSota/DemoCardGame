using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel
{
    public string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;
    public bool isPlayerCard;
    public ABILITY ability;
    public SPELL spell;
    public CardModel(int cardId, bool isPlayer)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardDataList/Card" + cardId);
        name = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        ability = cardEntity.ability;
        isAlive = true;
        isPlayerCard = isPlayer;
        spell = cardEntity.spell;
    }
    void Damage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    // Self heal
    void RecoveryHp(int amount)
    {
        hp += amount;
    }

    // Card heal
    public void Heal(CardController card)
    {
        card.model.RecoveryHp(at);
    }

    public void Attack(CardController cardController)
    {
        cardController.model.Damage(at);
    }
}
