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
    // Start is called before the first frame update
    void Start()
    {
        iceMineCollision = GetComponent<SphereCollider>();
        iceMineDetonatedPS.Stop();
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
        gameObject.SetActive(false);
    }
}
