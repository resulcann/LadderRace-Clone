using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class AI : Character
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
    float spawnTime;
    [SerializeField] float spawnDelay;
    Vector3 startPos;
    Collector collector;

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
            Move();
            CheckIsGrounded();
        }

    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "StartSpawning" && collector.collectedLadderParts.Count > 0 && isGrounded)
        {
            StartSpawningLadder();
        }
        if(other.gameObject.tag == "StopSpawning" || collector.collectedLadderParts.Count == 0)
        {
            StopSpawningLadder();
        }


        if(other.gameObject.tag == "FinishLine")
        {
            FindObjectOfType<GameplayController>().IsActive = false;
            GameManager.Instance.ChangeCurrentGameState(GameState.FinishSuccess);
            StopMoving();
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
            StopSpawningLadder();
        }
    }

    public override void Move()
    {
        transform.Translate(new Vector3(0f, 0f, playerSpeed * Time.deltaTime));
    }

    public override void StopMoving()
    {
        playerSpeed = 0f;
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
            Ladders[i].name = "Ladder Part" + " " + i;
            posY += 0.259808f;
            posZ += 0.15f;
            transform.position = Ladders[i].transform.position;
        }
    }
    public void StartSpawningLadder()
    {
        rigidBody.useGravity = false;
        startPos = transform.position;
        playerSpeed = 0f;
        animator.SetBool("HighPoint",true);
        DOTween.KillAll();
        InvokeRepeating("SpawnLadder", spawnTime, spawnDelay);
    }

    public override void StopSpawningLadder()
    {
        rigidBody.useGravity = true;
        animator.SetBool("HighPoint", true);
        playerSpeed = playerSpeedHolder;
        CancelInvoke("SpawnLadder");
        foreach (var item in Ladders)
        {
            item.GetComponent<Rigidbody>().useGravity = true;
            item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        Ladders.Clear();
    }

    public override void CheckIsGrounded()
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
        
        if(rigidBody.velocity.y < 0 && !isGrounded)
        {
            animator.SetBool("HighPoint", true);
            animator.SetBool("Landing", true);
            animator.SetBool("Run", false);
        }
    }

}