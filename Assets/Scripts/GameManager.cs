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
    [SerializeField] TMP_Text playerManaCostText, enemyManaCostText;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text timeCountText;
    [SerializeField] Transform playerHero;

    List<int> playerDeck = new List<int>() { 1, 1, 2, 2, 3 },
              enemyDeck = new List<int>() { 1, 2, 2, 3, 1 };
    int playerHeroHp, enemyHeroHp;
    public int playerManaCost, enemyManaCost;


    bool isPlayerTurn;
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
        if (hand.name == "PlayerHand")
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
            StartCoroutine(EnemyTurn());
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
    private IEnumerator EnemyTurn()
    {
        CardController[] cardEnemyFieldList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        EnableAtackFramColor(cardEnemyFieldList, true);
        yield return new WaitForSeconds(1);

        // To get the hand list
        CardController[] cardList = enemyHandTransform.GetComponentsInChildren<CardController>();

        // if enemy has under cost card, they keep to play card until it goes to be nothing hand
        while (Array.Exists(cardList, card => card.model.cost < enemyManaCost))
        {
            // Get under cost card
            CardController[] selectableHandCardList = Array.FindAll(cardList, card => card.model.cost < enemyManaCost);

            // choose the card to play
            CardController enemyCard = selectableHandCardList[0];
            // To move card
            enemyCard.OnField(false);
            StartCoroutine(enemyCard.cardMovement.MoveToFeild(enemyFieldTransform));
            cardList = enemyHandTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }


        yield return new WaitForSeconds(1);

        // To get field card list
        CardController[] enemyFeildCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // They attack successively until there are no more cards available for attack.
        while (Array.Exists(enemyFeildCardList, card => card.model.canAttack))
        {
            // Get can attack card
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyFeildCardList, item => item.model.canAttack);
            // Get defender card
            CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();

            // To choose attacker
            CardController attacker = enemyCanAttackCardList[0];
            if (playerFieldCardList.Length > 0)
            {
                // To choose Defender
                CardController defender = playerFieldCardList[0];

                StartCoroutine(attacker.cardMovement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.51f);
                // Make attacker and defender battle
                CardsButtle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.cardMovement.MoveToTarget(playerHero));
                yield return new WaitForSeconds(0.25f);
                AttackToHero(attacker, false);
                yield return new WaitForSeconds(0.25f);
                CheckHeroHP();
            }
            yield return new WaitForSeconds(1);
            enemyFeildCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        yield return new WaitForSeconds(1);
        ChangeTurn();
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

    void EnableAtackFramColor(CardController[] fieldCardList, bool enable)
    {
        foreach (CardController card in fieldCardList)
        {
            card.SetAttackEnable(enable);
        }
    }
}
