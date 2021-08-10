using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Image BG;
    private string controllerType;

    private bool inMenu;
    private bool gameStarted;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (!(Input.GetJoystickNames()[i] == ""))
            {
                if (Input.GetJoystickNames()[i].ToLower().Contains("xbox"))
                    controllerType = "Xbox";
                else
                    controllerType = "PS";

                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameStarted && inMenu) // While in Menu
        {
            if (Input.GetButton("OK " + controllerType + " 1"))
                gameStarted = true;
        }
        else if(gameStarted) // When the player presses OK button
        {
            BG.color += new Color(0, 0, 0, Time.deltaTime);

            GetComponent<AudioSource>().volume -= Time.deltaTime;

            if (BG.color.a >= 1)
            {
                GetComponent<AudioSource>().volume = 0;
                SceneManager.LoadScene("LoadingScene");
            }
        }
        else // Before entering Menu
        {
            BG.color -= new Color(0, 0, 0, Time.deltaTime);

            if (BG.color.a <= 0)
                inMenu = true;
        }

    }
}
