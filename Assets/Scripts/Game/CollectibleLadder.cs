using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollectibleLadder : MonoBehaviour
{
    bool isCollected;
    void Start()
    {
        isCollected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool GetIsCollected()
    {
        return isCollected;
    }

    public void SetCollected()
    {
        isCollected = true;
        transform.tag = "Collected";
    }
    
    public void SetPositions(float y, float z)
    {
        transform.DOLocalMove(new Vector3(0f, y , z), 0.1f);
    }

}
