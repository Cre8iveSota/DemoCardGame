using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GamePlayerManager player, enemy;
    [SerializeField] AI enemyAI;
    [SerializeField] public Transform playerHandTransform, enemyHandTransform, playerFieldTransform, enemyFieldTransform;
    [SerializeField] CardController CardPrefab;
    [SerializeField] public Transform playerHero, enemyHero;
    [SerializeField] UIManager uiManager;

    public bool isPlayerTurn;
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
        player.Init(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        enemy.Init(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        uiManager.HideResultPanel();
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        isPlayerTurn = true;
        // Distribute 3 cards for player hand
        SettingInitHand();
        TurnCulc();
    }

    private void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            GiveCardtoHand(player.deck, playerHandTransform);
            GiveCardtoHand(enemy.deck, enemyHandTransform);
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
        timeCount = 15;
        uiManager.UpdateTime(timeCount);
        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1f);
            timeCount--;
            uiManager.UpdateTime(timeCount);
        }
        ChangeTurn();
    }

    public CardController[] GetEnemyFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
    }
    public CardController[] GetFriendFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
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
            player.IncreaseManaCost();
            GiveCardtoHand(player.deck, playerHandTransform);
        }
        else
        {
            enemy.IncreaseManaCost();
            GiveCardtoHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
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
    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
    }
    public void AttackToHero(CardController attackerCard)
    {
        if (attackerCard.model.isPlayerCard) { enemy.heroHp -= attackerCard.model.at; }
        else
        {
            player.heroHp -= attackerCard.model.at;
        }
        attackerCard.SetAttackEnable(false);
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    }
    public void HealToHero(CardController healer)
    {
        if (healer.model.isPlayerCard) { player.heroHp += healer.model.at; }
        else
        {
            enemy.heroHp += healer.model.at;
        }
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    }
    public void CheckHeroHP()
    {
        if (player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            ShowResultPanel(player.heroHp);
        }
    }

    void ShowResultPanel(int playerHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(playerHp);
    }

    public void Restart()
    {
        // Delete hand  and field
        ClearFieldandHand();

        // deck generate
        player.deck = new List<int>() { 1, 1, 2, 2, 3 };
        enemy.deck = new List<int>() { 1, 2, 2, 3, 1 };
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
