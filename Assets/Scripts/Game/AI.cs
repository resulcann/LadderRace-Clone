using UnityEngine;
using DG.Tweening;

public class AI : Character
{
    int successPercentage = 50;

    void Start()
    {
        playerSpeedHolder = playerSpeed;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collector = GetComponent<Collector>();
        Mathf.Clamp(successPercentage, 30, 80);
    }

    void Update()
    {
        if(GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            base.Move();
            base.CheckIsGrounded();
        }

    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "StartSpawning" && collector.collectedLadderParts.Count > 0 && isGrounded)
        {
            int randomValue = (int)Random.Range(0,100);
            Debug.Log(randomValue);
            if( successPercentage > randomValue)
            {
                base.StartSpawningLadder();
            }
            
        }
        if(other.gameObject.tag == "StopSpawning" || collector.collectedLadderParts.Count == 0)
        {
            base.StopSpawningLadder();
        }


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

        if(other.gameObject.tag == "Wall" || other.gameObject.tag == "Ground")
        {
            base.StopSpawningLadder();
        }
    }

}