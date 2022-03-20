using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class AI : Character
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
            base.CheckIsGrounded();

            if(collector.collectedLadderParts.Count == 0 && !isGrounded)
            {
                base.StopSpawningLadder();
            }

            if(GameManager.Instance.aiSuccessPercentage > 80)
            {
                GameManager.Instance.aiSuccessPercentage = 80;
            }
            if(GameManager.Instance.aiSuccessPercentage < 30)
            {
                GameManager.Instance.aiSuccessPercentage = 30;
            }
        }

    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "StartSpawning" && isGrounded && collector.collectedLadderParts.Count > 0)
        {
            int randomValue = (int)Random.Range(0,100);
            if( GameManager.Instance.aiSuccessPercentage > randomValue)
            {
                base.StartSpawningLadder();
            }
            
        }
        if(other.gameObject.tag == "StopSpawning")
        {
            base.StopSpawningLadder();
        }


        if(other.gameObject.tag == "FinishLine")
        {
            base.StopMoving();
            animator.SetBool("Victory", true);
            FindObjectOfType<Player>().LoseGame();
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
            Ladders[i].name = "AI's Ladder Part" + "[" + i +"]";
            LevelManager.Instance.allSpawnedLadders.Add(Ladders[i]);
            posY += 0.259808f;
            posZ += 0.15f;
            transform.position = Ladders[i].transform.position;
        }
    }

}