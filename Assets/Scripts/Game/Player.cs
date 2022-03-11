using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Character
{
    void Start()
    {
        playerSpeedHolder = playerSpeed;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collector = GetComponent<Collector>();
        
    }

    void Update()
    {
        if(GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            base.Move();
            CheckIsGrounded();

            if(Input.GetMouseButtonDown(0) && collector.collectedLadderParts.Count > 0 && isGrounded)
            {
                base.StartSpawningLadder();
            }
            if(Input.GetMouseButtonUp(0) || (Input.GetMouseButton(0) && collector.collectedLadderParts.Count == 0))
            {
                base.StopSpawningLadder();
            }
        }

    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "FinishLine")
        {
            FindObjectOfType<GameplayController>().IsActive = false;
            GameManager.Instance.ChangeCurrentGameState(GameState.FinishSuccess);
            base.StopMoving();
            animator.Play("Victory");

        }
        
    }
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Wall")
        {
            transform.DOLocalMoveZ(transform.localPosition.z - 3f, 0.5f);
        } 

        if(other.gameObject.tag == "Wall" && collector.collectedLadderParts.Count == 0)
        {
            GameManager.Instance.ChangeCurrentGameState(GameState.FinishFail);
            base.StopMoving();
            animator.Play("Lose");
        }

        if(other.gameObject.tag == "Wall" || other.gameObject.tag == "Ground")
        {
            base.StopSpawningLadder();
        }
    }

}