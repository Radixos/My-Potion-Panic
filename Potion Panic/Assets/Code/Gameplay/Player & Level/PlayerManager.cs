using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();
    public bool levelEstablished;

    public Transform[] spawnPoints;

    private List<float> spawnTimers = new List<float>(); // Countdown to respawn
    private float spawnDelay; // Time between respawns

    // EVENTS
    //public delegate void OnMatchCompleted();
    //public event OnMatchCompleted OnMatchCompletedEvent;
    public bool matchCompleted;

    // UI
    private string victoryText;
    private Color victoryTextColor;
    public Text victoryBanner;

    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = 4.0f;

        EstablishLevel();

        StartCoroutine(ManagePlayers());
    }

    // Update is called once per frame
    void Update()
    {
        if (!matchCompleted)
        {
            // Spawn player at spawn point after a few seconds
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
                        players[i].transform.rotation = spawnPoints[i].rotation;
                        players[i].gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            //victoryBanner.transform.parent.gameObject.SetActive(true);

            victoryBanner.text = victoryText;
            victoryBanner.color = victoryTextColor;
        }

        // DEBUG CODE
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if(Input.GetKeyDown(KeyCode.Tab))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    private void EstablishLevel()
    {
        int activePlayers = 0;

        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (!(Input.GetJoystickNames()[i] == ""))
            {
                activePlayers++;
            }
        }

        if (activePlayers < 4) // Removing non-active players from game
        {
            for (int i = activePlayers + 1; i <= 4; i++)
            {
                Destroy(GameObject.Find("Player " + i.ToString() + " Info"));
                Destroy(GameObject.Find("Player " + i.ToString() + " Cauldron"));
                Destroy(GameObject.Find("Player " + i.ToString() + " Arrow Pool"));
                Destroy(GameObject.Find("Player " + i.ToString() + " Volcanic Blast Pool"));
                Destroy(GameObject.Find("Player " + i.ToString()));
            }
        }

        for(int i = 1; i <= activePlayers; i++)
        {
            players.Add(GameObject.Find("Player " + i).GetComponent<PlayerController>());
        }

        for (int i = 0; i < players.Count; i++)
            spawnTimers.Add(0.0f);

        levelEstablished = true;

    }

    private IEnumerator ManagePlayers()
    {
        while (true)
        {
            if (!matchCompleted)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].kills >= 1)
                    {
                        //OnMatchCompletedEvent?.Invoke();
                        victoryText = "Player " + (i + 1) + " Wins!";

                        switch(i + 1)
                        {
                            case 1: victoryTextColor = new Color(0.0f, 0.5f, 1.0f);
                                break;
                            case 2: victoryTextColor = Color.red;
                                break;
                            case 3: victoryTextColor = Color.green;
                                break;
                            case 4: victoryTextColor = Color.yellow;
                                break;
                        }

                        matchCompleted = true;
                    }

                    if (players[i].health <= 0 && !players[i].isDead)
                    {
                        players[i].isDead = true;
                        players[i].PlayerReset();
                        players[i].gameObject.SetActive(false);
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
