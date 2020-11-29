using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Transform obstacleSpawnPoint;
    public Transform coinSpawnPoint;
    public Coin coinPrefab;
    public Obstacle obsactlePrefab;
    public GameObject obstacles;
    public GameObject coins;
    public float minSpawnTimeObstacle = 3f;
    public float maxSpawnTimeObstacle = 6f;
    public float minSpawnTimeCoin = 3f;
    public float maxSpawnTimeCoin = 20f;

    private GameObject spawnedObstacle;
    private GameObject spawnedCoin;
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
        spawnedObstacle = Instantiate(obsactlePrefab.gameObject);
        spawnedObstacle.transform.SetParent(obstacles.transform);
        spawnedObstacle.transform.position = new Vector3(obstacleSpawnPoint.position.x, obstacleSpawnPoint.position.y, obstacleSpawnPoint.position.z);
        Invoke("SpawnObstacle", Random.Range(minSpawnTimeObstacle, maxSpawnTimeObstacle));
    }

    public void SpawnCoin()
    {
        spawnedCoin = Instantiate(coinPrefab.gameObject);
        spawnedCoin.transform.position = new Vector3(coinSpawnPoint.position.x, coinSpawnPoint.position.y, coinSpawnPoint.position.z);
        spawnedCoin.transform.SetParent(coins.transform);
        Invoke("SpawnCoin", Random.Range(minSpawnTimeCoin, maxSpawnTimeCoin));
    }

    public void DestroyAllSpawnedObjects()
    {
        CancelInvoke();
        foreach(Transform obstacle in obstacles.transform)
        {
            Destroy(obstacle.gameObject);
        }
        foreach(Transform coin in coins.transform)
        {
            Destroy(coin.gameObject);
        }
    }
}
