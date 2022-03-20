using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public List<Level> Levels;
    public int currentLevelIndex = 1;
    [HideInInspector] public List<GameObject> allSpawnedLadders;

    void Awake() 
    {
        Instance = this;
    }
    void Start() 
    {
        SpawnCurrentLevel();
    }
    
    public void SpawnCurrentLevel()
    {
        Levels[currentLevelIndex-1].CreateLevel();
    }

    public void NextLevel()
    {
        
        Levels[currentLevelIndex-1].DestroyLevel();
        currentLevelIndex += 1;
        Levels[currentLevelIndex-1].CreateLevel();
        DestroyAllLadder();
    }
    public void RetryLevel()
    {
        Levels[currentLevelIndex-1].DestroyLevel();
        Levels[currentLevelIndex-1].CreateLevel();
        DestroyAllLadder();
    }

    public void DestroyAllLadder()
    {
        for (int i = 0; i < allSpawnedLadders.Count; i++)
        {
            Destroy(allSpawnedLadders[i]);
        }
        allSpawnedLadders.Clear();
    }
}