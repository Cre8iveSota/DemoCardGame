using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] AI enemyAI;
    [SerializeField] public Transform playerHandTransform, enemyHandTransform, playerFieldTransform, enemyFieldTransform;
    [SerializeField] CardController CardPrefab;
    [SerializeField] TMP_Text playerHeroHpText, enemyHeroHpText;
    [SerializeField] TMP_Text playerManaCostText, enemyManaCostText;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text timeCountText;
    [SerializeField] public Transform playerHero;

    List<int> playerDeck = new List<int>() { 1, 1, 2, 2, 3 },
              enemyDeck = new List<int>() { 1, 2, 2, 3, 1 };
    int playerHeroHp, enemyHeroHp;
    public int playerManaCost, enemyManaCost;


    public bool isPlayerTurn;
    int playerDefaultManaCost, enemyDefaultManaCost;
    int timeCount;

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
        playerDefaultManaCost = enemyDefaultManaCost = 5;
        playerManaCost = enemyManaCost = 5;
        resultPanel.SetActive(false);
        playerHeroHp = 30;
        enemyHeroHp = 30;
        ShowHeroHp();
        ShowManaCost();
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
        if (hand.name == "Player Hand")
        {
            card.Init(cardId, true);
        }
        else
        {
            card.Init(cardId, false);
        }
        // card.Init(2);
    }
    void TurnCulc()
    {
        StopAllCoroutines(); // 安全のため、現状のコルーチンを止めておく
        StartCoroutine(CountDown());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(enemyAI.EnemyTurn());
        }
    }

    IEnumerator CountDown()
    {
        timeCount = 5;
        timeCountText.text = timeCount.ToString();
        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1f);
            timeCount--;
            timeCountText.text = timeCount.ToString();
        }
        ChangeTurn();
    }

    public CardController[] GetEnemyFieldCards()
    {
        return enemyFieldTransform.GetComponentsInChildren<CardController>();
    }

    public void OnClickTurnEndButton()
    {
        if (isPlayerTurn)
        {
            ChangeTurn();
        }
    }
    public void ChangeTurn()
    {
        CardController[] cardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        EnableAtackFramColor(cardList, false);

        CardController[] cardEnemyFieldList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        EnableAtackFramColor(cardEnemyFieldList, false);



        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn)
        {
            playerDefaultManaCost++;
            playerManaCost = playerDefaultManaCost;
            GiveCardtoHand(playerDeck, playerHandTransform);
        }
        else
        {
            enemyDefaultManaCost++;
            enemyManaCost = enemyDefaultManaCost;
            GiveCardtoHand(enemyDeck, enemyHandTransform);
        }
        ShowManaCost();
        TurnCulc();
    }

    private void PlayerTurn()
    {
        CardController[] cardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        EnableAtackFramColor(cardList, true);
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

    void ShowManaCost()
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }
    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            playerManaCost -= cost;
        }
        else
        {
            enemyManaCost -= cost;
        }
        ShowManaCost();
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
    }
    public void CheckHeroHP()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            ShowResultPanel(playerHeroHp);
        }
    }

    void ShowResultPanel(int playerHp)
    {
        StopAllCoroutines();
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

    public void EnableAtackFramColor(CardController[] fieldCardList, bool enable)
    {
        foreach (CardController card in fieldCardList)
        {
            card.SetAttackEnable(enable);
        }
    }
}
