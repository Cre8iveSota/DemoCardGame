using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text playerHeroHpText, enemyHeroHpText;
    [SerializeField] TMP_Text playerManaCostText, enemyManaCostText;
    [SerializeField] TMP_Text timeCountText;

    public void HideResultPanel()
    {
        resultPanel.SetActive(false);
    }
    public void ShowHeroHp(int playerHeroHp, int enemyHeroHp)
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }

    public void ShowManaCost(int playerManaCost, int enemyManaCost)
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }

    public void UpdateTime(int timeCount)
    {
        timeCountText.text = timeCount.ToString();
    }

    public void ShowResultPanel(int playerHeroHp)
    {
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
}
