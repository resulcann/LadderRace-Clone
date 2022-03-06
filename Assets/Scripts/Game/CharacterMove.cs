using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class CharacterMove : MonoBehaviour
{
    Rigidbody rigidBody;
    Animator animator;
    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isGrounded = false;
    float distToGround = 0.4f;
    public float playerSpeed;
    float playerSpeedHolder;
    public List<GameObject> Ladders;
    [SerializeField] GameObject ladderPrefab;
    private float spawnTime;
    [SerializeField] float spawnDelay;
    Vector3 startPos;
    Collector collector;
    [HideInInspector] public bool canPressButton = true;

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
        
    }

    void CharacterMovement()
    {
        if(GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            
            isGrounded = Physics.CheckSphere(groundCheck.position, distToGround, groundMask); // Is character touching ground?
            if(isGrounded)
            {
                animator.SetBool("Run", true);
                animator.SetBool("HighPoint", false);
                animator.SetBool("Landing", false);
            }else{
                animator.SetBool("Run", false);
            }
            
            transform.Translate(new Vector3(0f, 0f, playerSpeed * Time.deltaTime)); // Character moving forward continously


            if(Input.GetMouseButtonUp(0) || (Input.GetMouseButton(0) && collector.collectedLadderParts.Count == 0) || canPressButton == false)
            {
                rigidBody.useGravity = true;
                animator.SetBool("Landing", true);
                playerSpeed = playerSpeedHolder;
                CancelInvoke("SpawnLadder");
                foreach (var item in Ladders)
                {
                    item.GetComponent<Rigidbody>().useGravity = true;
                    item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
                Ladders.Clear();
            }

            /* If you keep touching to screen or holding left mouse button and if you collected ladder pieces then player speed is will be zero and
            character goes to jumping animation and 1 ladder spawns at the time.*/
            
            if(Input.GetMouseButtonDown(0) && collector.collectedLadderParts.Count > 0 && isGrounded && canPressButton == true)
            {
                rigidBody.useGravity = false;
                startPos = transform.position;
                playerSpeed = 0f;
                animator.SetBool("HighPoint",true);

                InvokeRepeating("SpawnLadder", spawnTime, spawnDelay);
                
                
            }
            /* If you release your hands from screen or stop holding left mouse button character goes to landing animation and you stopped ladders spawning.
            */
            // if(Input.GetMouseButtonUp(0))
            // {
            //     animator.SetBool("Landing", true);
            //     playerSpeed = playerSpeedHolder;
            //     rigidBody.useGravity = true;
            //     CancelInvoke("SpawnLadder");

            //     foreach (var item in Ladders)
            //     {
            //         item.GetComponent<Rigidbody>().useGravity = true;
            //         item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //     }
            //     Ladders.Clear();
            // }

            

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
                Ladders[i].transform.localPosition = new Vector3(1f, startPos.y + posY, startPos.z + posZ);
                Ladders[i].name = "Ladder Part" + " " + i;
                posY += 0.259808f;
                posZ += 0.15f;
                transform.position = Ladders[i].transform.position;
            }
        }else{
            CancelInvoke("SpawnLadder");
        }
        

    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "FinishLine")
        {
            FindObjectOfType<GameplayController>().IsActive = false;
            GameManager.Instance.ChangeCurrentGameState(GameState.FinishSuccess);
            playerSpeed = 0f;
            
            //animator.Play("Victory");

        }
        
    }
    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Enemy")
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 1f), 2f);
            if(collector.collectedLadderParts.Count == 0)
            {
                GameManager.Instance.ChangeCurrentGameState(GameState.FinishFail);
                playerSpeed = 0f;
                //animator.Play("Lose");
            }
        }
        // if(other.gameObject.tag == "Enemy" && collector.collectedLadderParts.Count == 0)
        // {
        //     GameManager.Instance.ChangeCurrentGameState(GameState.FinishFail);
        //     playerSpeed = 0f;
        //     //animator.Play("Lose");
        // }
    }
    
    
}
