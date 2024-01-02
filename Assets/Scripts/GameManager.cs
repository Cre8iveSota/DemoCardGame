using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform, enemyHandTransform;
    [SerializeField] CardController CardPrefab;
    bool isPlayerTurn;
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
        TurnCulc();
    }

    private void PlayerTurn()
    {
        Debug.Log("Start Player");
    }
    private void EnemyTurn()
    {
        Debug.Log("Start Enemy");
    }

}
