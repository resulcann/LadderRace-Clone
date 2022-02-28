using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collector : MonoBehaviour
{
    public static Collector Instance { get; private set; }
    public GameObject backpack;
    public List<GameObject> collectedLadderParts;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Collectible")
        {
            other.gameObject.transform.parent = backpack.transform;
            other.gameObject.transform.localRotation = new Quaternion(0f,0f,0f,0f);
            other.gameObject.GetComponent<CollectibleLadder>().SetCollected();
            collectedLadderParts.Add(other.gameObject);
            SetPositions();
            
        }
    }

    public void SetPositions()
    {
        for (int i = 0; i < collectedLadderParts.Count; i++)
        {
            collectedLadderParts[i].transform.DOLocalMove(new Vector3(0f, (i / 4) * 0.1f - 0.1f , -(i % 4) * 0.1f - 0.1f), 0.1f);
        }
    }
}
