using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    public int totalVotes;

    private int retryVotes;
    private int exitVotes;

    private int numOfPlayers;

    public Text retryVotesDisplay;
    public Text exitVotesDisplay;
    public List<GameObject> endGameInputObjs; // Option groups

    private PlayerManager playerManager;

    public List<GameObject> endGameVoters;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (!(Input.GetJoystickNames()[i] == ""))
                ++numOfPlayers;
        }

        playerManager = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.matchCompleted)
        {
            for (int i = 0; i < numOfPlayers; i++)
                endGameVoters[i].SetActive(true);

            for (int i = 0; i < endGameInputObjs.Count; i++)
                endGameInputObjs[i].SetActive(true);

            ControllerCheck();

            if (totalVotes >= numOfPlayers)
            {
                if (retryVotes > exitVotes)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                else if (exitVotes > retryVotes)
                    SceneManager.LoadScene("Main Menu");
            }

            retryVotesDisplay.text = "(Votes: " + retryVotes.ToString() + ")";
            exitVotesDisplay.text = "(Votes: " + exitVotes.ToString() + ")";
        }

    }

    public void IncreaseRetryVote(bool isSwitching)
    {
        retryVotes++;

        if (isSwitching)
            exitVotes--;
    }

    public void IncreaseExitVote(bool isSwitching)
    {
        exitVotes++;

        if (isSwitching)
            retryVotes--;
    }

    void ControllerCheck()
    {
        int updatedPlayers = 0;

        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (!(Input.GetJoystickNames()[i] == ""))
            {
                ++updatedPlayers;

                numOfPlayers = updatedPlayers;
            }

        }
    }
}
