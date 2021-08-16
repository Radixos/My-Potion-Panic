using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Image BG;

    public bool inMenu;
    private bool gameStarted;

    private int numOfPlayers;
    public int playersReady;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (!(Input.GetJoystickNames()[i] == ""))
                ++numOfPlayers;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted && inMenu) // While in Menu
        {
            ControllerCheck();

            if (numOfPlayers == playersReady)
                gameStarted = true;
        }
        else if (gameStarted) // When the player presses OK button
        {
            BG.color += new Color(0, 0, 0, Time.deltaTime);

            GetComponent<AudioSource>().volume -= Time.deltaTime;

            if (BG.color.a >= 1)
            {
                GetComponent<AudioSource>().volume = 0;
                SceneManager.LoadScene("Loading Scene");
            }
        }
        else // Before entering Menu
        {
            BG.color -= new Color(0, 0, 0, Time.deltaTime);

            if (BG.color.a <= 0)
                inMenu = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
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
