using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    Rigidbody rigidBody;
    Animator animator;
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float thrustForce = 10;
    public Vector3 axis;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        MoveForward();
    }

    void MoveForward()
    {
        if(GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            rigidBody.velocity = Vector3.forward * playerSpeed * 100 * Time.deltaTime;
            if(Input.GetMouseButton(0))
            {
                rigidBody.AddForce(Vector3.up * thrustForce);
            }
            
        }

        
    }
}
