using UnityEngine;

public class CollectibleLadder : MonoBehaviour
{
    bool isCollected;
    void Start()
    {
        isCollected = false;
    }

    // Is the object collected or not?
    public bool GetIsCollected()
    {
        return isCollected;
    }

    // The object is set to have been collected before.
    public void SetCollected()
    {
        isCollected = true;
        transform.tag = "Collected";
    }
    

}
