using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Player : Character
{
    GameplayController gameplayController;
    void Awake()
    {
        playerSpeedHolder = playerSpeed;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collector = GetComponent<Collector>();
        gameplayController = FindObjectOfType<GameplayController>();
        
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
            if((Input.GetMouseButtonUp(0) && !isGrounded) || (Input.GetMouseButton(0) && collector.collectedLadderParts.Count == 0 && !isGrounded))
            {
                base.StopSpawningLadder();
            }
        }

    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "FinishLine")
        {
            gameplayController.FinishGameplay(true);
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
            LoseGame();
        }

        if(other.gameObject.tag == "Wall" || other.gameObject.tag == "Ground")
        {
            
            base.StopSpawningLadder();
        }
    }
    public void LoseGame()
    {
        base.StopMoving();
        animator.Play("Lose");
        gameplayController.FinishGameplay(false);
    }

    public override void SpawnLadder()
    {
        animator.SetBool("HighPoint",true);
        float posY = 0.15f, posZ = 0.2f;
        GameObject ladder = Instantiate(ladderPrefab) as GameObject;
        Ladders.Add(ladder);
        ladder.GetComponent<Rigidbody>().useGravity = false;
        ladder.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        GameObject tempGo = collector.collectedLadderParts.Last();
        collector.collectedLadderParts.Remove(collector.collectedLadderParts.Last());
        Destroy(tempGo);

        for (int i = 0; i < Ladders.Count; i++)
        {
            Ladders[i].transform.localPosition = new Vector3(transform.localPosition.x, startPos.y + posY, startPos.z + posZ);
            Ladders[i].name = "Player's Ladder Part" + "[" + i +"]";
            LevelManager.Instance.allSpawnedLadders.Add(Ladders[i]);
            posY += 0.259808f;
            posZ += 0.15f;
            transform.position = Ladders[i].transform.position;
        }
    }

    

}