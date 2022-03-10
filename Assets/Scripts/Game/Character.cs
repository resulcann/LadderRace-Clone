using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract void Move();
    public abstract void StopMoving();
    public abstract void SpawnLadder();
    public abstract void StopSpawningLadder();
    public abstract void CheckIsGrounded();
    

}
