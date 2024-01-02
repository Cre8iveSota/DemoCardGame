using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform, enemyHandTransform, playerFieldTransform, enemyFieldTransform;
    [SerializeField] CardController CardPrefab;

    bool isPlayerTurn;

    // シングルトン化
    public static GameManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        isPlayerTurn = true;
        // Distribute 3 cards for player hand
        SettingInitHand();
        TurnCulc();
    }

    private void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateCard(playerHandTransform);
            CreateCard(enemyHandTransform);
        }
    }

    private void CreateCard(Transform hand)
    {
        CardController card = Instantiate(CardPrefab, hand, false);
        card.Init(1);
        // card.Init(2);
    }
    void TurnCulc()
    {
        if (isPlayerTurn)
        {
            PlayerTurn();
            CardController[] cardList = playerFieldTransform.GetComponentsInChildren<CardController>();
            foreach (CardController item in cardList)
            {
                // change the state to enable attack
                item.SetAttackEnable(true);
            }
        }
        else
        {
            EnemyTurn();
            ChangeTurn();
        }
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn) CreateCard(playerHandTransform);
        else
        {
            CreateCard(enemyHandTransform);
        }
        TurnCulc();
    }

    private void PlayerTurn()
    {
        Debug.Log("Start Player");
    }
    private void EnemyTurn()
    {
        // To get the hand list
        CardController[] cardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        // choose the card to play
        CardController enemyCard = cardList[0];
        // To move card
        enemyCard.cardMovement.SetCardTransform(enemyFieldTransform);
        // To get field card list
        CardController[] enemyFeildCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // Get can attack card
        CardController[] enemyCanAttackCardList = Array.FindAll(enemyFeildCardList, item => item.model.canAttack);
        // Get defender card
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        if (enemyCanAttackCardList.Length > 0 && playerFieldCardList.Length > 0)
        {
            // To choose attacker
            CardController attacker = enemyCanAttackCardList[0];
            // To choose Defender
            CardController defender = playerFieldCardList[0];
            // Make attacker and defender battle
            CardsButtle(attacker, defender);
        }
        Debug.Log("Start Enemy");
    }

    public void CardsButtle(CardController attacker, CardController defender)
    {
        Debug.Log("attacker hp" + attacker.model.hp);
        Debug.Log("defender hp" + defender.model.hp);

        attacker.Attack(defender);
        defender.Attack(attacker);
        Debug.Log("attacker hp" + attacker.model.hp);
        Debug.Log("defender hp" + defender.model.hp);
        attacker.CheckAlive();
        defender.CheckAlive();
    }

}
