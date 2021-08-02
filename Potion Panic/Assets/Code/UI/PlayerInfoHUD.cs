using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoHUD : MonoBehaviour
{
    private PlayerController player;

    // UI
    public Text killCounter;
    public Text spellInfo;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find(gameObject.name).GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        killCounter.text = player.kills.ToString();

        if (player.hasSpell)
            spellInfo.text = "Spell: " + player.spellInfo.Name + " (" + player.spellUses + ")";
        else if (player.myCauldron.consumedIngredients.Count < 3)
            spellInfo.text = "Ingredients Used: " + player.myCauldron.consumedIngredients.Count;
    }

}
