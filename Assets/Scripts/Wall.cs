using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        player.EndEpisode();
    }
}
