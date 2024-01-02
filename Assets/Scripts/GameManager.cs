using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform;
    [SerializeField] CardController CardPrefab;
    private void Start()
    {
        CreateCard(playerHandTransform);
    }

    private void CreateCard(Transform hand)
    {
        CardController card = Instantiate(CardPrefab, hand, false);
        card.Init(1);
        // card.Init(2);
    }
}
