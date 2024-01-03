using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public IEnumerator EnemyTurn()
    {
        CardController[] cardEnemyFieldList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.EnableAtackFramColor(cardEnemyFieldList, true);
        yield return new WaitForSeconds(1);

        // To get the hand list
        CardController[] cardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        // if enemy has under cost card, they keep to play card until it goes to be nothing hand
        while (Array.Exists(cardList, card =>
                (card.model.cost < gameManager.enemy.manaCost)
            && (!card.IsSpell
            || (card.IsSpell && card.CanUseSpell()))
        ))
        {
            // Get under cost card
            CardController[] selectableHandCardList = Array.FindAll(cardList, card =>
            (card.model.cost < gameManager.enemy.manaCost)
            && (!card.IsSpell
            || (card.IsSpell && card.CanUseSpell())
            ));

            // choose the card to play
            CardController enemyCard = selectableHandCardList[0];

            // Turn the card over to the surface
            enemyCard.Show();

            // If the card type is a spell card, use it
            if (enemyCard.IsSpell)
            {
                StartCoroutine(CastSpellOf(enemyCard));
            }
            else
            {
                // To move card
                enemyCard.OnField();
                StartCoroutine(enemyCard.cardMovement.MoveToFeild(gameManager.enemyFieldTransform));
            }
            yield return new WaitForSeconds(1);
            cardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }


        yield return new WaitForSeconds(1);

        // To get field card list
        CardController[] enemyFeildCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        // They attack successively until there are no more cards available for attack.
        while (Array.Exists(enemyFeildCardList, card => card.model.canAttack))
        {
            // Get can attack card
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyFeildCardList, item => item.model.canAttack);
            // Get defender card
            CardController[] playerFieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();

            // To choose attacker
            CardController attacker = enemyCanAttackCardList[0];
            if (playerFieldCardList.Length > 0)
            {
                // To choose Defender
                // Only Shild cards are targeted for attack.
                if (Array.Exists(playerFieldCardList, card => card.model.ability == ABILITY.SHEILD))
                {
                    playerFieldCardList = Array.FindAll(playerFieldCardList, card => card.model.ability == ABILITY.SHEILD);
                }
                CardController defender = playerFieldCardList[0];

                StartCoroutine(attacker.cardMovement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.51f);
                // Make attacker and defender battle
                gameManager.CardsButtle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.cardMovement.MoveToTarget(gameManager.playerHero));
                yield return new WaitForSeconds(0.25f);
                gameManager.AttackToHero(attacker);
                yield return new WaitForSeconds(0.25f);
                gameManager.CheckHeroHP();
            }
            yield return new WaitForSeconds(1);
            enemyFeildCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        yield return new WaitForSeconds(1);
        gameManager.ChangeTurn();
        Debug.Log("Start Enemy");
    }

    IEnumerator CastSpellOf(CardController card)
    {
        CardController target = null;
        Transform movePosition = null;
        switch (card.model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                target = gameManager.GetEnemyFieldCards(card.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.HEAL_FRIEND_CARD:
                target = gameManager.GetFriendFieldCards(card.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                movePosition = gameManager.playerFieldTransform;
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                movePosition = gameManager.enemyFieldTransform;
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                movePosition = gameManager.playerHero;
                break;
            case SPELL.HEAL_FRIEND_HERO:
                movePosition = gameManager.enemyHero;
                break;
        }
        StartCoroutine(card.cardMovement.MoveToFeild(movePosition));
        yield return new WaitForSeconds(0.5f);
        card.UseSpellTo(target);
    }

}
