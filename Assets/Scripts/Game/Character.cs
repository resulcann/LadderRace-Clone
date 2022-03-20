using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public abstract class Character : MonoBehaviour
{
    protected Rigidbody rigidBody;
    protected Animator animator;
    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isGrounded = false;
    protected float distToGround = 0.4f;
    public float playerSpeed;
    protected float playerSpeedHolder;
    public List<GameObject> Ladders;
    [SerializeField] protected GameObject ladderPrefab;
    protected float spawnTime;
    [SerializeField] protected float spawnDelay;
    protected Vector3 startPos;
    protected Collector collector;

    protected void Move()
    {
        transform.Translate(new Vector3(0f, 0f, playerSpeed * Time.deltaTime));
    }

    protected void StopMoving()
    {
        playerSpeed = 0f;
    }

    public virtual void SpawnLadder()
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

    protected void StartSpawningLadder()
    {   
        rigidBody.useGravity = false;
        startPos = transform.position;
        playerSpeed = 0f;

        DOTween.KillAll();
        InvokeRepeating("SpawnLadder", spawnTime, spawnDelay);
    }
    
    protected void StopSpawningLadder()
    {
        CancelInvoke("SpawnLadder");
        rigidBody.useGravity = true;
        playerSpeed = playerSpeedHolder;
        
        if(Ladders.Count > 0)
        {
            foreach (var item in Ladders)
            {
                item.GetComponent<Rigidbody>().useGravity = true;
                item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
        Ladders.Clear();
        
    }

    protected void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, distToGround, groundMask); // Is character touching ground?
        if(isGrounded)
        {
            animator.SetBool("Run", true);
            animator.SetBool("HighPoint", false);
            animator.SetBool("Landing", false);
        }
        
        if(rigidBody.velocity.y < 0 && !isGrounded)
        {
            animator.SetBool("Landing", true);
            animator.SetBool("HighPoint", false);
            animator.SetBool("Run", false);
        }
        
    }

    

}
