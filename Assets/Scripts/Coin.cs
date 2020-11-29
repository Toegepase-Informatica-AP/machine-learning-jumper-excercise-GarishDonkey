using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float minSpeed = 2f;
    public float maxSpeed = 6f;
    private float speed;
    void Start()
    {
        this.speed = Random.Range(minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        this.transform.Translate(Vector3.right * this.speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }
}
