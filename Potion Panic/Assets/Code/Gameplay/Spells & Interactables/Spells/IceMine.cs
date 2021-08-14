using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class IceMine : SpellBehaviour
{
    public ParticleSystem iceMinePlantedPS;
    public ParticleSystem iceMineDetonatedPS;
    public ParticleSystem iceMineDetonatedBuildupPS;
    private SphereCollider iceMineCollision;
    public int iceMineState = 0;
    private float timerForHurtbox = 0f;
    private bool foundCaster = false;
    private GameObject[] storedPlayers;
    private PlayerController[] storedControllers;
    public ParticleSystem[] storedOrigins;
    //in order, blue, red, green, yellow
    private byte[] storedR = {25, 207, 13, 236};
    private byte[] storedG = {53, 13, 207, 218};
    private byte[] storedB = {214, 27, 13, 23};
    public ParticleSystem.EmissionModule emissionInterface;
    public ParticleSystem.MinMaxGradient colorInterface;

    // Start is called before the first frame update
    void Start()
    {
        iceMineCollision = GetComponent<SphereCollider>();
        iceMineDetonatedPS.Stop();
        storedPlayers = GameObject.FindGameObjectsWithTag("Player");
        storedControllers = new PlayerController[4];
        for (int i = 0; i < storedPlayers.Length; i++)
        {
            storedControllers[i] = storedPlayers[i].GetComponent<PlayerController>();
        }
        //findCaster();
    }

    // Update is called once per frame
    void Update()
    {
        if (iceMineState > 0)
        {
            timerForHurtbox += Time.deltaTime;
            if (timerForHurtbox > 3f && iceMineState == 1)
            {
                // MATT - This is where Ice Mine gets detonated i.e. another player steps into the mine

                iceMineState++;
            }
            if (iceMineState == 2 && iceMineCollision.radius < 4.2)
            {
                iceMineCollision.radius = iceMineCollision.radius + 0.1f;
            }
            if (iceMineState == 2 && iceMineDetonatedBuildupPS.emission.rateOverTimeMultiplier < 200)
            {
                emissionInterface = iceMineDetonatedBuildupPS.emission;
                emissionInterface.rateOverTimeMultiplier = iceMineDetonatedBuildupPS.emission.rateOverTimeMultiplier + 3f;
            }
            if (timerForHurtbox > 10f && !iceMineDetonatedPS.isEmitting)
            {
                SpellReset();
            }
        }
        else
        {
            iceMineDetonatedPS.Stop();
        }
    }
    //private void findCaster()
    //{
    //    for (int i = 0; i < storedPlayers.Length; i++)
    //    {
    //        if (storedControllers[i] == caster)
    //        {
    //            for (int j = 0; j < storedOrigins.Length; j++)
    //            {
    //                //this doesn't work and i don't know why lol
    //                colorInterface = storedOrigins[j].main.startColor;
    //                colorInterface.color = new Color32(storedR[i], storedG[i], storedB[i], 255);
    //            }
    //        }
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.CompareTag("Player")) && (other.gameObject.GetInstanceID() != caster.gameObject.GetInstanceID())
            && (!other.gameObject.GetComponent<PlayerController>().isBlinking) && !iceMinePlantedPS.isPlaying)
        {
            switch (iceMineState)
            {
                case 0:
                    iceMineState++;
                    iceMineDetonatedPS.Play();
                    RuntimeManager.PlayOneShot("event:/Potions/Ice Spell Activate", transform.position);
                    break;
                case 2:
                    other.gameObject.GetComponent<PlayerController>().health--;
                    if (other.gameObject.GetComponent<PlayerController>().health <= 0)
                    {
                        caster.kills += 1;
                    }
                    break;
            }
        }
    }

    protected override void SpellReset()
    {
        caster = null;
        timerForHurtbox = 0f;
        iceMineState = 0;
        iceMineCollision.radius = 2;
        iceMinePlantedPS.Stop();
        iceMineDetonatedPS.Stop();
        emissionInterface.rateOverTimeMultiplier = 20;
        foundCaster = false;
        gameObject.SetActive(false);
    }
}
