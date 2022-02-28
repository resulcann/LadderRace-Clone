using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterMove : MonoBehaviour
{
    Rigidbody rigidBody;
    Animator animator;
    public bool isGrounded = false;
    float distToGround = 0.5f;
    public float playerSpeed, zPos;
    float playerSpeedHolder;
    public float thrustForce;
    public List<GameObject> Ladders;
    [SerializeField] GameObject ladderPrefab;
    bool stopSpawning = false;
    public float spawnTime, spawnDelay;
    Vector3 startPos;

    void Start()
    {
        playerSpeedHolder = playerSpeed;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
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
            transform.Translate(new Vector3(0f,0f,playerSpeed * Time.deltaTime));
            if(Input.GetMouseButton(0))
            {
                
                // float tempY = 2.65f, tempZ = 0.2f;

                // transform.Translate(0f, thrustForce/100, zPos);
                rigidBody.useGravity = false;
                animator.SetBool("Running", false);
                animator.SetBool("JumpStart", true);

                // GameObject ladder = Instantiate(ladderPrefab) as GameObject;
                // Ladders.Add(ladder);

                // for (int i = 0; i < Ladders.Count; i++)
                // {
                //     Ladders[i].transform.localPosition = new Vector3(1f, tempY, transform.position.z + tempZ);
                //     tempY += 0.295442f;
                //     tempZ += 0.0520945f;
                // }

                InvokeRepeating("SpawnObject", spawnTime, spawnDelay);


            }

            if(Input.GetMouseButtonDown(0))
            {
                startPos = transform.position;
                playerSpeed = 0f;
                
            }
            if(Input.GetMouseButtonUp(0))
            {
                playerSpeed = playerSpeedHolder;
                rigidBody.useGravity = true;
                animator.SetBool("JumpLanding", true);
                animator.SetBool("JumpStart", false);
                CancelInvoke("SpawnObject");
                Ladders.Clear();
            }

        }
        


    }

    public void SpawnObject()
    {
        float tempY = 2.65f, tempZ = 0.2f;

        GameObject ladder = Instantiate(ladderPrefab) as GameObject;
        Ladders.Add(ladder);
        for (int i = 0; i < Ladders.Count; i++)
        {
            Ladders[i].transform.localPosition = new Vector3(1f, tempY, startPos.z + tempZ);
            Ladders[i].name = "Ladder Part" + i;
            tempY += 0.295442f;
            tempZ += 0.0520945f;
            transform.position = Ladders[i].transform.position;
        }
        if(stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }
    

    void GroundCheck()
    {
        if(Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f))
        {
            isGrounded = true;
            animator.SetBool("Running", true);
            animator.SetBool("JumpStart", false);
            animator.SetBool("JumpHighPoint", false);
            animator.SetBool("JumpLanding", false);

        }else{
            isGrounded = false;
        }
    }
    
}
