using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerater : MonoBehaviour
{
    public GameObject cardImage;
    [SerializeField] private Transform cardParent;
    // Start is called before the first frame update
    void Start()
    {
        // カードの生成方法1
        // GameObject cardObj = Instantiate(cardImage);
        // cardObj.transform.SetParent(cardParent, false);

        // カードの生成方法2
        GameObject cardObj = Instantiate(cardImage, cardParent);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
