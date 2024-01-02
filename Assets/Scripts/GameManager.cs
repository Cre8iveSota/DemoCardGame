using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform, enemyHandTransform, playerFieldTransform, enemyFieldTransform;
    [SerializeField] CardController CardPrefab;
    [SerializeField] TMP_Text playerHeroHpText, enemyHeroHpText;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TMP_Text resultText;

    List<int> playerDeck = new List<int>() { 1, 1, 2, 2, 3 },
              enemyDeck = new List<int>() { 1, 2, 2, 3, 1 };
    int playerHeroHp, enemyHeroHp;

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
        resultPanel.SetActive(false);
        playerHeroHp = 30;
        enemyHeroHp = 30;
        ShowHeroHp();
        isPlayerTurn = true;
        // Distribute 3 cards for player hand
        SettingInitHand();
        TurnCulc();
    }

    private void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            GiveCardtoHand(playerDeck, playerHandTransform);
            GiveCardtoHand(enemyDeck, enemyHandTransform);
        }
    }
    private void GiveCardtoHand(List<int> deck, Transform hand)
    {
        if (deck.Count == 0)
        {
            return;
        }
        int cardId = deck[0];
        deck.RemoveAt(0);
        CreateCard(cardId, hand);
    }

    private void CreateCard(int cardId, Transform hand)
    {
        CardController card = Instantiate(CardPrefab, hand, false);
        card.Init(cardId);
        // card.Init(2);
    }
    void TurnCulc()
    {
        if (isPlayerTurn)
        {
            PlayerTurn();
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
        if (isPlayerTurn) GiveCardtoHand(playerDeck, playerHandTransform);
        else
        {
            GiveCardtoHand(enemyDeck, enemyHandTransform);
        }
        TurnCulc();
    }

    private void PlayerTurn()
    {
        CardController[] cardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController item in cardList)
        {
            // change the state to enable attack
            item.SetAttackEnable(true);
        }
    }
    private void EnemyTurn()
    {
        CardController[] cardEnemyFieldList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController item in cardEnemyFieldList)
        {
            // change the state to enable attack
            item.SetAttackEnable(true);
        }
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
        if (enemyCanAttackCardList.Length > 0)
        {
            // To choose attacker
            CardController attacker = enemyCanAttackCardList[0];
            if (playerFieldCardList.Length > 0)
            {
                // To choose Defender
                CardController defender = playerFieldCardList[0];
                // Make attacker and defender battle
                CardsButtle(attacker, defender);
            }
            else
            {
                AttackToHero(attacker, false);
            }
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
    void ShowHeroHp()
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }

    public void AttackToHero(CardController attackerCard, bool isPlayerCard)
    {
        if (isPlayerCard) { enemyHeroHp -= attackerCard.model.at; }
        else
        {
            playerHeroHp -= attackerCard.model.at;
        }
        attackerCard.SetAttackEnable(false);
        ShowHeroHp();
        CheckHeroHP();
    }
    void CheckHeroHP()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            resultPanel.SetActive(true);
            if (playerHeroHp <= 0)
            {
                resultText.text = "Lose";
            }
            else
            {
                resultText.text = "Win";
            }
        }
    }

    public void Restart()
    {
        // Delete hand  and field
        ClearFieldandHand();

        // deck generate
        playerDeck = new List<int>() { 1, 1, 2, 2, 3 };
        enemyDeck = new List<int>() { 1, 2, 2, 3, 1 };
        StartGame();
    }

    private void ClearFieldandHand()
    {
        foreach (Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }
    }
}
