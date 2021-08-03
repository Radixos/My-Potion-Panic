using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoHUD : MonoBehaviour
{
    private PlayerController player;
    [Range(1, 4)]
    public int playerID;

    // KIllS
    public Text killCounter;

    // HEALTH
    public Image healthBar;

    // SPELL
    public GameObject spellInfo;
    public Image spellIcon;
    public Text numOfUses;

    public bool leftAligned;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player " + playerID.ToString()).GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (leftAligned)
            killCounter.text = "x " + player.kills.ToString();
        else
            killCounter.text = player.kills.ToString() + " x";

        if (player.hasSpell)
        {
            spellInfo.SetActive(true);

            spellIcon.sprite = player.spellInfo.spellPreview;

            if (leftAligned)
                numOfUses.text = "x " + player.spellUses;
            else
                numOfUses.text = player.spellUses + " x";
        }
        else
            spellInfo.SetActive(false);

        healthBar.fillAmount = player.health / 100;
    }

}
