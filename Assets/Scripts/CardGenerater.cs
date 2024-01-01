using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerater : MonoBehaviour
{
    public GameObject cardImage;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(cardImage);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
