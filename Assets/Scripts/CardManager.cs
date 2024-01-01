using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform endPoint;
    void Start()
    {
        transform.DOMove(endPoint.position, 2f);
    }
}
