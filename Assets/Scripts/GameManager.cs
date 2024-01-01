using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Entry entry;
    void Start()
    {
        panel.SetActive(true);
        Debug.Log("EntryAt " + entry.at);
        Debug.Log("EntryHP  " + entry.hp);
    }

}
