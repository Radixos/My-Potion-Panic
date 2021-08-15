using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureArrow : SpellBehaviour
{
    private float lifeTime = 4.0f;

    // Start is called before the first frame update
    //void Start()
    //{        
    //}

    // Update is called once per frame
    void Update()
    {
        if (lifeTime <= 0.0f)
            SpellReset();
        else
            lifeTime -= Time.deltaTime;
    }

    protected override void SpellReset()
    {
        caster = null;
        lifeTime = 4.0f;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetInstanceID() != caster.gameObject.GetInstanceID())
            {
                if (!other.gameObject.GetComponent<PlayerController>().isBlinking)
                {
                    
                    // MATT - Audio Call for impact of arrow

                    other.gameObject.GetComponent<PlayerController>().health = 0;
                    caster.kills += 1;
                    //SpellReset();
                }
            }
        }
    }
}
