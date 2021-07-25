using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainBehaviour : MonoBehaviour
{
    [SerializeField] private int delay = 2;
    [SerializeField] private float minRandomSpawnDelay = 1.5f, maxRandomSpawnDelay = 2f;
    [SerializeField] private float minSpawnOff = 1.75f, maxSpawnOff = 3f;
    [SerializeField] private List<GameObject> ingredients;

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
            Vector3 center = gameObject.GetComponent<Renderer>().bounds.center;
            Vector3 pos = RandomCircle(center, Random.Range(minSpawnOff, maxSpawnOff));

            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            GameObject clone = Instantiate(ingredients[(Random.Range(0, ingredients.Count))], center, rot);

            Vector3 cloneCenter = clone.transform.position;

            float privateRotation = 70f;    //between 1f and 89f
            float dragDown = 10f;           //"speed"

            // Calculate distance to target
            float target_Distance = Vector3.Distance(cloneCenter, pos);

            // Calculate the velocity needed to throw the object to the target at specified angle.
            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * privateRotation * Mathf.Deg2Rad) / dragDown);

            // Extract the X  Y componenent of the velocity
            float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(privateRotation * Mathf.Deg2Rad);
            float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(privateRotation * Mathf.Deg2Rad);

            // Calculate flight time.
            float flightDuration = target_Distance / Vx;

            // Rotate projectile to face the target.
            clone.transform.rotation = Quaternion.LookRotation(pos - clone.transform.position);

            float elapse_time = 0;

            while (elapse_time < flightDuration)
            {
                clone.transform.Translate(0, (Vy - (dragDown * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

                elapse_time += Time.deltaTime;

                yield return null;
            }

            float waitTime = Random.Range(minRandomSpawnDelay, maxRandomSpawnDelay);
            //Debug.Log(waitTime);
            yield return new WaitForSeconds(waitTime);
            
            //StartCoroutine(DelayAction(minRandomSpawnDelay, maxRandomSpawnDelay));
        }
    }

    //private IEnumerator Fountain()
    //{
    //    yield return new WaitForSeconds(delay);

    //    while (true)
    //    {
    //        Vector3 center = transform.position;
    //        Vector3 pos = RandomCircle(center, Random.Range(minSpawnOff, maxSpawnOff));
    //        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
    //        Instantiate(ingredients[(Random.Range(0, ingredients.Count))], pos, rot);

    //        float waitTime = Random.Range(minRandomSpawnDelay, maxRandomSpawnDelay);
    //        Debug.Log(waitTime);
    //        yield return new WaitForSeconds(waitTime);

    //        //StartCoroutine(DelayAction(minRandomSpawnDelay, maxRandomSpawnDelay));
    //    }
    //}

    /// <summary>
    /// Generate a random circle with a center and radius
    /// </summary>
    /// <param name="center">The center point of the circle</param>
    /// <param name="radius">The radius of the circle (use Random.Range to create a ring)</param>
    /// <returns>A circle</returns>
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
