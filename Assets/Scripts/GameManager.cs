using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    void Start()
    {
        panel.SetActive(true);
    }

}
