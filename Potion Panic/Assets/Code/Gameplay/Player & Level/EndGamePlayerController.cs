using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGamePlayerController : MonoBehaviour
{
    [Range(1,4)]
    public int playerID;

    private bool selected;

    private string controllerType;

    private EndGameManager endGameManager;

    private int selectedOption = 2;

    // Start is called before the first frame update
    void Start()
    {
        endGameManager = FindObjectOfType<EndGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ControllerCheck();

        if (Input.GetButtonDown("OK " + controllerType + " " + playerID.ToString()) &&
            selectedOption != 0)
        {
            if(!selected)
            {
                endGameManager.totalVotes++;
                selected = true;
                endGameManager.IncreaseRetryVote(false);
            }
            else
                endGameManager.IncreaseRetryVote(true);

            selectedOption = 0;
        }
        else if (Input.GetButtonDown("Interact " + controllerType + " " + playerID.ToString()) &&
            selectedOption != 1)
        {
            if (!selected)
            {
                endGameManager.totalVotes++;
                selected = true;
                endGameManager.IncreaseExitVote(false);
            }
            else
                endGameManager.IncreaseExitVote(true);

            selectedOption = 1;
        }
    }

    void ControllerCheck()
    {
        int controllerNum = 0;

        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (!(Input.GetJoystickNames()[i] == ""))
            {
                ++controllerNum;

                // Cofirming that the controller in order matches with the player number
                if (controllerNum == playerID)
                {
                    if (Input.GetJoystickNames()[i].ToLower().Contains("xbox"))
                        controllerType = "Xbox";
                    else
                        controllerType = "PS";

                    break;
                }
            }
            else
                controllerType = ""; // If it gets disconnected in game

        }
    }
}
