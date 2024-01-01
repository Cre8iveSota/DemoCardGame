using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform;
    [SerializeField] GameObject CardPrefab;
    private void Start()
    {
        CreateCard(playerHandTransform);
    }

    private void CreateCard(Transform hand)
    {
        Instantiate(CardPrefab, hand, false);
    }
}
