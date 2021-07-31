using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerController> players;

    public Transform[] spawnPoints;

    private List<float> spawnTimers = new List<float>(); // Countdown to respawn
    private float spawnDelay; // Time between respawns

    // EVENTS
    public delegate void OnMatchCompleted();
    public event OnMatchCompleted OnMatchCompletedEvent;
    private bool matchCompleted;

    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = 4.0f;

        for(int i = 0; i < players.Count; i++)
            spawnTimers.Add(0.0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!matchCompleted)
        {
            // Check who has won
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].kills >= 10)
                {
                    OnMatchCompletedEvent?.Invoke();
                    matchCompleted = true;
                    break;
                }

            }

            // Remove player from play area
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].health <= 0 && !players[i].isDead)
                {
                    players[i].gameObject.SetActive(false);
                    players[i].isDead = true;
                }
            }

            // Spawn player at spawn point after few seconds
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].isDead)
                {
                    spawnTimers[i] += Time.deltaTime;

                    if (spawnTimers[i] >= spawnDelay)
                    {
                        spawnTimers[i] = 0.0f;

                        players[i].Respawn();
                        players[i].transform.position = spawnPoints[i].position;
                        players[i].gameObject.SetActive(true);
                    }
                }
            }
        }
        
    }
}
