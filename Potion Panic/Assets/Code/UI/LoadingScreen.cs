using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Image BG;

    public List<GameObject> UIObjects;
    private int currentObj;
    private bool goingDark;

    private List<float> displayTimers = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        displayTimers.Add(5.0f);
        displayTimers.Add(15.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (goingDark)
        {
            BG.color += new Color(0, 0, 0, Time.deltaTime);

            if (currentObj + 1 >= UIObjects.Count)
                GetComponent<AudioSource>().volume -= Time.deltaTime;

            if (BG.color.a >= 1)
            {
                if (currentObj + 1 < UIObjects.Count)
                {
                    UIObjects[currentObj].SetActive(false);
                    ++currentObj;
                    UIObjects[currentObj].SetActive(true);
                    goingDark = false;
                }
                else
                {
                    GetComponent<AudioSource>().volume = 0;
                    SceneManager.LoadScene("Scene_Mockup_Mani");
                }
            }
        }
        else if (BG.color.a > 0)
            BG.color -= new Color(0, 0, 0, Time.deltaTime);


        if (BG.color.a <= 0)
        {
            displayTimers[currentObj] -= Time.deltaTime;

            if (displayTimers[currentObj] <= 0)
                goingDark = true;
        }



    }
}
