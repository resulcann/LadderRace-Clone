using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    GameObject player;
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Ground" && other.gameObject.tag == "Wall")
        {
            player.GetComponent<Player>().StopSpawningLadder();
        }
    }
}
