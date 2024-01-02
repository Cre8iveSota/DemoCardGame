using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardModel model;
    CardView cardView;
    private void Awake()
    {
        cardView = GetComponent<CardView>();
    }
    public void Init(int cardId)
    {
        model = new CardModel(cardId);
        cardView.Show(model);
    }
}
