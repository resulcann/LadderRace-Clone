using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class CharacterMove : MonoBehaviour
{
    Rigidbody rigidBody;
    Animator animator;
    public bool isGrounded = false;
    float distToGround = 0.5f;
    public float playerSpeed;
    float playerSpeedHolder;
    public List<GameObject> Ladders;
    [SerializeField] GameObject ladderPrefab;
    bool stopSpawning = false;
    public float spawnTime, spawnDelay;
    Vector3 startPos;
    Collector collector;
    bool stopHolding = false;

    void Start()
    {
        playerSpeedHolder = playerSpeed;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collector = GetComponent<Collector>();
    }

    void Update()
    {
        CharacterMovement();
        GroundCheck();
        
    }

    void CharacterMovement()
    {
        if(GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            // Character moving forward continously
            transform.Translate(new Vector3(0f,0f,playerSpeed * Time.deltaTime));

            /* If you keep touching to screen or holding left mouse button and if you collected ladder pieces then player speed is will be zero and
            character goes to jumping animation and 1 ladder spawns at the time.*/
            
            if(Input.GetMouseButtonDown(0) && collector.collectedLadderParts.Count > 0)
            {
                rigidBody.useGravity = false;
                animator.SetBool("Running", false);
                animator.SetBool("JumpStart", true);
                startPos = transform.position;
                playerSpeed = 0f;
                InvokeRepeating("SpawnLadder", spawnTime, spawnDelay);
                
            }
            /* If you release your hands from screen or stop holding left mouse button character goes to landing animation and you stopped ladders spawning.
            */
            if(Input.GetMouseButtonUp(0))
            {
                playerSpeed = playerSpeedHolder;
                rigidBody.useGravity = true;

                animator.SetBool("JumpLanding", true);
                animator.SetBool("JumpStart", false);

                CancelInvoke("SpawnLadder");
                foreach (var item in Ladders)
                {
                    item.GetComponent<Rigidbody>().useGravity = true;
                    item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
                Ladders.Clear();
            }

        }
        


    }

    /* Ladder spawner. Every time i call this function its spawns one ladder and adds it to Ladders list.
        Every ladder's position will be in front of previous ladder in list. 
        And Every time i call this function Player's position will be last spawned ladder's position.
    */

    public void SpawnLadder()
    {
        if(collector.collectedLadderParts.Count > 0)
        {
            float posY = 2.65f, posZ = 0.2f;
            GameObject ladder = Instantiate(ladderPrefab) as GameObject;
            Ladders.Add(ladder);
            ladder.GetComponent<Rigidbody>().useGravity = false;
            ladder.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            GameObject tempGo = collector.collectedLadderParts.Last();
            collector.collectedLadderParts.Remove(collector.collectedLadderParts.Last());
            Destroy(tempGo);

            for (int i = 0; i < Ladders.Count; i++)
            {
                Ladders[i].transform.localPosition = new Vector3(1f, posY, startPos.z + posZ);
                Ladders[i].name = "Ladder Part" + " " + i;
                posY += 0.295442f;
                posZ += 0.0520945f;
                transform.position = Ladders[i].transform.position;
            }
        }else{
            CancelInvoke("SpawnLadder");
        }
        

    }
    
    // Am i touching ground or not?
    void GroundCheck()
    {
        if(Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f))
        {
            isGrounded = true;
            animator.Play("Running");
            animator.SetBool("Running", true);
            animator.SetBool("JumpStart", false);
            animator.SetBool("JumpHighPoint", false);
            animator.SetBool("JumpLanding", false);

        }else{
            isGrounded = false;
        }
    }
    
}
