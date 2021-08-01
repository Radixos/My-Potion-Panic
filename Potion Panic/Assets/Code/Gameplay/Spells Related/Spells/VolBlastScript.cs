using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolBlastScript : MonoBehaviour
{
    [SerializeField] private Spell_SO spellInfo;
    //user player. needed for checking their inventory? and number of uses
    //targeted player. needed for accessing their health, etc
    [SerializeField] private PlayerController userPlayer, targetPlayer;
    private ParticleSystem volcanoFX;

    void Start()
    {
        volcanoFX = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (volcanoFX.isPlaying == false)
        {
            Destroy(gameObject);
        }
        //Debug.Log(Vector3.Distance(transform.position, targetPlayer.transform.position));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            targetPlayer.transform.position = targetPlayer.transform.position;
        }
    }
}

//<=2 (closest) --> <=5.5 (furthest)
//INSTAKILL -------> =-