using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public GameObject backpack;
    public List<GameObject> collectedLadderParts;
    
    void Update() 
    {
        SetPositions();
    }

    // if player touches the who has "Collectible" tag object so object is gonna added to collectedLadderParts.
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Collectible")
        {
            other.gameObject.transform.parent = backpack.transform;
            other.gameObject.transform.localRotation = new Quaternion(0f,0f,0f,0f);
            other.gameObject.GetComponent<CollectibleLadder>().SetCollected();
            collectedLadderParts.Add(other.gameObject);
            
        }
    }

    // Giving positions to collected ladderparts in my backpack. Its gonna be four horizontally and nolimits for vertical positions.
    public void SetPositions()
    {
        for (int i = 0; i < collectedLadderParts.Count; i++)
        {
            collectedLadderParts[i].transform.localPosition = new Vector3(0f, (i / 4) * 0.1f - 0.1f , -(i % 4) * 0.1f - 0.1f);
        }
    }
}
