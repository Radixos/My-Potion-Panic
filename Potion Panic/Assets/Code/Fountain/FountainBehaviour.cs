using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainBehaviour : MonoBehaviour
{
    [SerializeField] private int delay = 2;
    [SerializeField] private float minRandomSpawnDelay = 1.5f, maxRandomSpawnDelay = 2f;
    [SerializeField] private float minSpawnOff = 1f, maxSpawnOff = 2f;
    [SerializeField] private List<GameObject> ingredients;

    private Vector3 center;

    private void Start()
    {
        StartCoroutine(nameof(Fountain));
    }

    private void Update()
    {
        
    }

    private IEnumerator Fountain()
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            Vector3 center = transform.position;
            Vector3 pos = RandomCircle(center, Random.Range(minSpawnOff, maxSpawnOff));
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            Instantiate(ingredients[Random.Range(0, ingredients.Count)], pos, rot);

            //float waitTime = Random.Range(minRandomSpawnDelay, maxRandomSpawnDelay);
            //Debug.Log(waitTime);
            //yield return new WaitForSeconds(0);

            StartCoroutine(DelayAction(minRandomSpawnDelay, maxRandomSpawnDelay));
        }
    }

    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float angle = Random.value * 360;
        Vector3 pos;
        pos.x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }

    private IEnumerator DelayAction(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    private IEnumerator DelayAction(float minWaitTime, float maxWaitTime)
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
    }
}
