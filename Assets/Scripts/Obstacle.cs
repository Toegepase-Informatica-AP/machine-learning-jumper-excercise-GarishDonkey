using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 0.6f;
    private float speed;
    private void Start()
    {
        this.speed = Random.Range(minSpeed, maxSpeed);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Translate(Vector3.right * this.speed * Time.deltaTime);
    }
}
