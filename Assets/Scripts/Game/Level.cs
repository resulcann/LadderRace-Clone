using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Level", menuName ="Levels", order = 1)]
public class Level : ScriptableObject
{
    public GameObject levelPrefab;
    public int levelIndex;
    GameObject spawnedLevelPrefab;

    public void CreateLevel()
    {
        spawnedLevelPrefab = Instantiate(levelPrefab) as GameObject;
        Camera.main.GetComponent<CamFollow>().target = FindObjectOfType<Player>().transform;
    }

    public void DestroyLevel()
    {
        DestroyImmediate(spawnedLevelPrefab);
    }
}
