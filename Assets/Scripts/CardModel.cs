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
    public CardModel(int cardId, bool isPlayer)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardDataList/Card" + cardId);
        name = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        isAlive = true;
        isPlayerCard = isPlayer;
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

    public void Attack(CardController cardController)
    {
        cardController.model.Damage(at);
    }
}
