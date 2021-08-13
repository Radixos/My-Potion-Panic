using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathEffect : MonoBehaviour
{
    ParticleSystem ps;
    public float buffer = 0;
    public Vector3 lookUp;
    public List<GameObject> differentColors = new List<GameObject>();
    public int playerCount;
    public int player;
    void Start()
    {
        lookUp = new Vector3(this.transform.position.x, this.transform.position.y + 50, this.transform.position.z);
        transform.rotation = Quaternion.LookRotation(lookUp);
        //get colors
        foreach (Transform child in transform)
        {
            differentColors.Add(child.gameObject);
        }
        //get correct color
        for (int i = 0; i < playerCount; i++)
        {
            if (i == player)
            {
                differentColors[i].SetActive(true);
                ps = differentColors[i].GetComponent<ParticleSystem>();
            }
        }
    }
    void Update()
    {
        if (!ps.isEmitting)
        {
            buffer += Time.deltaTime;
        }
        if (buffer > 2.5)
        {
            Destroy(this.gameObject);
        }
    }
}
