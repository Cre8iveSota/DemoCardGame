using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardModel model;
    CardView cardView;
    public CardMovement cardMovement;
    GameManager gameManager;
    public bool IsSpell
    {
        get { return model.spell != SPELL.NONE; }
    }
    private void Awake()
    {
        cardView = GetComponent<CardView>();
        cardMovement = GetComponent<CardMovement>();
        gameManager = GameManager.instance;
    }
    public void Init(int cardId, bool isPlayer)
    {
        model = new CardModel(cardId, isPlayer);
        cardView.SetCard(model);
    }

    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        SetAttackEnable(false);
    }

    public void Heal(CardController friendCard)
    {
        model.Heal(friendCard);
        friendCard.RefreshView();
    }

    public void RefreshView()
    {
        cardView.Refresh(model);
    }
    public void CheckAlive()
    {
        if (model.isAlive)
        {
            RefreshView();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAttackEnable(bool enable)
    {
        cardView.SetSelectablePanel(enable);
        model.canAttack = enable;
    }
    public void OnField()
    {
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);
        model.isFieldCard = true;
        if (model.ability == ABILITY.INIT_ATTACKABLE)
        {
            SetAttackEnable(true);
        }
    }

    public void UseSpellTo(CardController target)
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard == model.isPlayerCard)
                {
                    return;
                }
                Attack(target);
                target.CheckAlive();
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                CardController[] opponentCards = gameManager.GetEnemyFieldCards(this.model.isPlayerCard);
                foreach (CardController opponentCard in opponentCards)
                {
                    Attack(opponentCard);
                }
                foreach (CardController opponentCard in opponentCards)
                {
                    // If this line merge to the former "Attack(target)" loop, it may happen to delete gameObject while the foreach loop
                    // To evacurate that, the line is separated.
                    opponentCard.CheckAlive();
                }
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                gameManager.AttackToHero(this);
                break;
            case SPELL.HEAL_FRIEND_CARD:
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard != model.isPlayerCard)
                {
                    return;
                }
                Heal(target);
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                CardController[] cardController = gameManager.GetFriendFieldCards(this.model.isPlayerCard);
                foreach (CardController card in cardController)
                {
                    Heal(card);
                }
                break;
            case SPELL.HEAL_FRIEND_HERO:
                gameManager.HealToHero(this);
                break;
            case SPELL.NONE:
                return;
        }
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);
        Destroy(gameObject);
    }
    public bool CanUseSpell()
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
            case SPELL.DAMAGE_ENEMY_CARDS:
                CardController[] opponentCards = gameManager.GetEnemyFieldCards(this.model.isPlayerCard);
                if (opponentCards.Length > 0)
                {
                    return true;
                }
                return false;
            case SPELL.DAMAGE_ENEMY_HERO:
            case SPELL.HEAL_FRIEND_HERO:
                return true;
            case SPELL.HEAL_FRIEND_CARD:
            case SPELL.HEAL_FRIEND_CARDS:
                CardController[] cardController = gameManager.GetFriendFieldCards(this.model.isPlayerCard);
                if (cardController.Length > 0)
                {
                    return true;
                }
                return false;
            case SPELL.NONE:
                return false;
        }
        return false;
    }

    public void Show()
    {
        cardView.Show();
    }
}
