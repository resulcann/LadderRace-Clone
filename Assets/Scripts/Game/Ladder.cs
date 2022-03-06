using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Ground" && other.gameObject.tag == "Enemy")
        {
            FindObjectOfType<CharacterMove>().canPressButton = false;
        }else{
            FindObjectOfType<CharacterMove>().canPressButton = true;
        }
    }
}
