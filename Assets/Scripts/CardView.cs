using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectablePanel;
    [SerializeField] private GameObject sheildPanel;


    public void Show(CardModel cardModel)
    {
        nameText.text = cardModel.name;
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
        if (cardModel.ability == ABILITY.SHEILD)
        {
            sheildPanel.SetActive(true);
        }
        else
        {
            sheildPanel.SetActive(false);
        }
    }

    public void Refresh(CardModel cardModel)
    {
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
    }

    public void SetSelectablePanel(bool enable)
    {
        selectablePanel.SetActive(enable);
    }
}
