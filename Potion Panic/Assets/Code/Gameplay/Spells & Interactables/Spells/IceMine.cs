using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        findCaster();
    }

    // Update is called once per frame
    void Update()
    {
        if (iceMineState > 0)
        {
            timerForHurtbox += Time.deltaTime;
            if (timerForHurtbox > 3f && iceMineState == 1)
            {
                iceMineState++;
            }
            if (iceMineState == 2 && iceMineCollision.radius < 4.2)
            {
                iceMineCollision.radius = iceMineCollision.radius + 0.1f;
            }
            if (iceMineState == 2 && iceMineDetonatedBuildupPS.emissionRate < 200)
            {
                iceMineDetonatedBuildupPS.emissionRate = iceMineDetonatedBuildupPS.emissionRate + 3;
            }
            if (timerForHurtbox > 10.5f)
            {
                SpellReset();
            }
        }
    }
    private void findCaster()
    {
        for (int i = 0; i < storedPlayers.Length; i++)
        {
            if (storedControllers[i] == caster)
            {
                for (int j = 0; j < storedOrigins.Length; j++)
                {
                    ParticleSystem.MainModule main = storedOrigins[j].main;
                    //storedOrigins[j].startColor = new Color32(storedR[i], storedG[i], storedB[i], 255);
                    main.startColor = new Color(storedR[i], storedG[i], storedB[i], 255f);
                }
            }
        }
    }

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
                    break;
                case 2:
                    other.gameObject.GetComponent<PlayerController>().health--;
                    if (other.gameObject.GetComponent<PlayerController>().health == 0)
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
        iceMineDetonatedBuildupPS.emissionRate = 20;
        foundCaster = false;
        gameObject.SetActive(false);
    }
}
