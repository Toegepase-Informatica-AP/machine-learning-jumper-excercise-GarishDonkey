﻿using Unity.MLAgents;
using UnityEngine;

public class Player : Agent
{
    private Environment environment;
    private Rigidbody body;
    public Transform spawnPoint = null;
    public float Force = 15f;
    private bool canJump = false;
    public override void Initialize()
    {
        this.body = this.GetComponent<Rigidbody>();
        this.environment = this.GetComponentInParent<Environment>();
    }

    public override void OnEpisodeBegin()
    {
        ResetPlayer();
        environment.SpawnObstacle();
    }

    private void OnCollisionEnter(Collision collision)

    {
        if (collision.gameObject.CompareTag("Obstacle") == true)
        {
            AddReward(-1.0f);
            Destroy(collision.gameObject);
            EndEpisode();
        } 
        if(collision.gameObject.CompareTag("Road") == true)
        {
            this.canJump = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reward"))
        {
            AddReward(1f);
            //Destroy(other.gameObject);
            //EndEpisode();
        }
    }

    void MoveUpwards()
    {
        canJump = false;
        body.AddForce(Vector3.up * Force, ForceMode.Impulse);
    }



    public override void OnActionReceived(float[] vectorAction)
    {
        if(vectorAction[0] == 1 && canJump)
        {
            AddReward(-0.2f);
            MoveUpwards();
        }
    }


    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        if(Input.GetKey(KeyCode.UpArrow) == true && canJump)
        {
            
            actionsOut[0] = 1;
        }
    }

    private void ResetPlayer()
    {
        this.body.angularVelocity = Vector3.zero;
        this.body.velocity = Vector3.zero;
        this.transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
    }
}
