using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardModel model;
    public void Init(int cardId)
    {
        model = new CardModel(cardId);
    }


}
