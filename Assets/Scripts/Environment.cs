using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Transform obstacleSpawnPoint;
    public Obstacle obsactlePrefab;

    private GameObject spawnedObject;
    private Player player;
    private TextMeshPro scoreboard;

    public void OnEnable()
    {
        this.player = this.transform.GetComponentInChildren<Player>();
        this.scoreboard = this.transform.GetComponentInChildren<TextMeshPro>();
    }

    private void FixedUpdate()
    {
        this.scoreboard.text = player.GetCumulativeReward().ToString("f2");
    }

    public void SpawnObstacle()
    {
        Debug.Log("Spawned obstacle");
        spawnedObject = Instantiate(obsactlePrefab.gameObject);
        spawnedObject.transform.position = new Vector3(obstacleSpawnPoint.position.x, obstacleSpawnPoint.position.y, obstacleSpawnPoint.position.z);
    }

    public void DestroySpawnedObstacle()
    {
        Destroy(spawnedObject);
    }
}
