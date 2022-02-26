using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collector : MonoBehaviour
{
    float posY = -0.16f;
    float posZ = -0.1f;
    public GameObject backpack;
    public List<GameObject> collectedLadders;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Collectible")
        {
            other.gameObject.transform.parent = backpack.transform;
            other.gameObject.transform.localRotation = new Quaternion(0f,0f,0f,0f);
            other.gameObject.GetComponent<CollectibleLadder>().SetCollected();
            SetPositions();
            collectedLadders.Add(other.gameObject);
            
        }
    }

    public void SetPositions()
    {

        for (int i = 0; i < collectedLadders.Count; i++)
        {
            collectedLadders[i].transform.DOLocalMove(new Vector3(0f, (i / 4) * 0.1f - 0.1f , -(i % 4) * 0.1f - 0.1f), 0.1f);
        }

    }
}
