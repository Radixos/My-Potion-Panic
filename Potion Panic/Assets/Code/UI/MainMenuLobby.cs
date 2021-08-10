using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLobby : MonoBehaviour
{
    private MainMenu mainMenu;

    [Range(1, 4)]
    public int playerID;

    // Icon
    public Sprite PSIcon;
    public Sprite XboxIcon;
    public Image inputIcon;

    private string controllerType;
    private bool isReady;

    // Text
    public Text Press;
    public Text toPlay;
    public Color readyColour;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();

        controllerType = "";
        ControllerCheck();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
        {
            if (ControllerCheck())
                Press.gameObject.transform.parent.gameObject.SetActive(true);
            else
                Press.gameObject.transform.parent.gameObject.SetActive(false);

            if (mainMenu.inMenu && !(controllerType == ""))
            {
                if (Input.GetButtonDown("OK " + controllerType + " " + playerID.ToString()))
                {
                    mainMenu.playersReady++;

                    toPlay.gameObject.SetActive(false);
                    inputIcon.gameObject.SetActive(false);
                    Press.text = "Ready!";
                    Press.color = readyColour;

                    isReady = true;
                }

            }
        }
    }

    bool ControllerCheck()
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
                    {
                        controllerType = "Xbox";
                        inputIcon.sprite = XboxIcon;
                    }
                    else
                    {
                        controllerType = "PS";
                        inputIcon.sprite = PSIcon;
                    }

                    break;
                }

            }
            else
                controllerType = ""; // If it gets disconnected in game

        }

        if (controllerType == "")
            return false;
        else
        {
            return true;
        }

    }
}
