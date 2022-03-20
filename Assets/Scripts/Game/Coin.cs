using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public AudioClip coinSound;
    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position , GameManager.Instance.IsMusicEnabled ? 0.5f : 0f);

            GameManager.Instance.numberOfCoins++;
            PlayerPrefs.SetInt("NumberOfCoins", GameManager.Instance.numberOfCoins);
            Destroy(gameObject);
        }
    }
}
