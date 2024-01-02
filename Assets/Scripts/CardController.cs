using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardModel model;
    CardView cardView;
    public CardMovement cardMovement;
    private void Awake()
    {
        cardView = GetComponent<CardView>();
        cardMovement = GetComponent<CardMovement>();
    }
    public void Init(int cardId)
    {
        model = new CardModel(cardId);
        cardView.Show(model);
    }

    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        SetAttackEnable(false);
    }


    public void CheckAlive()
    {
        if (model.isAlive)
        {
            cardView.Refresh(model);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAttackEnable(bool enable)
    {
        cardView.SetSelectablePanel(enable);
        model.canAttack = enable;
    }
}
