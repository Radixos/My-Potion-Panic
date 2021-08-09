using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Image BG;

    private float displayTimer = 5.0f;

    public List<GameObject> UIObjects;
    private int currentObj;
    private bool goingDark;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        if (goingDark)
        {
            BG.color += new Color(0, 0, 0, Time.deltaTime);

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
                    SceneManager.LoadScene("Scene_Mockup_Mani");

            }
        }
        else if (BG.color.a > 0)
            BG.color -= new Color(0, 0, 0, Time.deltaTime);


        if (BG.color.a <= 0)
        {
            displayTimer -= Time.deltaTime;

            if (displayTimer <= 0)
            {
                goingDark = true;
                displayTimer = 5.0f;
            }

        }



    }
}
