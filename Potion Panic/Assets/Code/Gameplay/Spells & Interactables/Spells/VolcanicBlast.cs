using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicBlast : SpellBehaviour
{
    private const float maxRadius = 5;
    private float radius = 1f;

    public GameObject blastEffect;

    private LayerMask playerLayer;

    private const float baseDamage = 100.0f;

    private List<GameObject> damagedEnemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        playerLayer = 1 << 6;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius, playerLayer);

        if (enemies.Length > 0)
        {
            float damage = 0;

            // Dealing max damage if enemies are within half the max blast radius
            if (radius <= 0.5 * maxRadius)
                damage = baseDamage;
            else // Dealing damage based on distance from the cast position
                damage = baseDamage - (baseDamage * radius / maxRadius);

            for (int i = 0; i < enemies.Length; i++)
            {
                float distBetween = Vector3.Distance(transform.position, enemies[i].transform.position);

                // Ignoring caster collider
                if (enemies[i].gameObject.GetInstanceID() != caster.gameObject.GetInstanceID() && // Spell shouldn't attack caster
                    !damagedEnemies.Contains(enemies[i].gameObject) && // One time hit
                    !enemies[i].gameObject.GetComponent<PlayerController>().isBlinking) // Can't damage while invulnerable
                {
                    enemies[i].gameObject.GetComponent<PlayerController>().health -= damage;

                    if (enemies[i].gameObject.GetComponent<PlayerController>().health <= 0)
                        caster.kills += 1;
                    else // Only add them if the spell hasn't killed them
                        damagedEnemies.Add(enemies[i].gameObject);
                }
            }
        }

        blastEffect.transform.localScale = new Vector3(radius * 2.15f, 0.1f, radius * 2.15f);

        if (radius < maxRadius) radius += 5 * Time.deltaTime;

        if (radius >= maxRadius)
            SpellReset();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    protected override void SpellReset()
    {
        radius = 1f;
        blastEffect.transform.localScale = new Vector3(radius, 0.1f, radius);
        damagedEnemies.Clear();
        gameObject.SetActive(false);
    }
}
