using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainBehaviour : MonoBehaviour
{
    [SerializeField] private int delay = 2;
    //[SerializeField] private float minRandomSpawnDelay = 1.5f, maxRandomSpawnDelay = 2f;
    [SerializeField] private float minSpawnOff = 1.75f, maxSpawnOff = 3f;
    [SerializeField] private int minIngredientOfType = 1, maxIngredientOfType = 4; //max is exclusive
    [SerializeField] private int timeBetweenWaves = 10;
    [SerializeField] public int destroyDelay = 8;
    [SerializeField] private AnimationCurve ac;
    [SerializeField] private List<GameObject> ingredientsList;

    private List<GameObject> ingredientsToSpawn;

    private void Start()
    {
        ingredientsToSpawn = new List<GameObject>(ingredientsList);
        StartCoroutine(nameof(Fountain));
    }

    private IEnumerator Fountain()
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            int noOfIngredientsOfType = Random.Range(minIngredientOfType, maxIngredientOfType + 1);

            if (noOfIngredientsOfType > 0)
            {
                Vector3 center = gameObject.GetComponent<Renderer>().bounds.center;
                center.y -= 1f;

                Vector3 pos = RandomCircle(center, Random.Range(minSpawnOff, maxSpawnOff));
                Debug.Log(pos);

                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
                GameObject clone = Instantiate(ingredientsToSpawn[0], center, rot);

                Vector3 cloneCenter = clone.transform.localPosition;

                float privateRotation = 70f;    //between 1f and 89f inclusive
                float dragDown = 20f;           //"speed"

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
                clone.transform.localRotation = Quaternion.LookRotation(pos - cloneCenter);

                float elapse_time = 0;

                while (elapse_time < flightDuration)
                {
                    clone.transform.Translate(0, (Vy - (dragDown * elapse_time)) * Time.deltaTime * ac.Evaluate(elapse_time), Vx * Time.deltaTime * ac.Evaluate(elapse_time));

                    elapse_time += Time.deltaTime;

                    yield return null;
                }

                clone.GetComponent<Rigidbody>().useGravity = true;

                clone.transform.rotation = Quaternion.Euler(clone.transform.rotation.x, 0, clone.transform.rotation.z);

                noOfIngredientsOfType--;

                if (noOfIngredientsOfType == 0)
                    ingredientsToSpawn.RemoveAt(0);
            }

            if (ingredientsToSpawn.Count == 0)
            {
                ingredientsToSpawn = new List<GameObject>(ingredientsList);
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }
    }

    //private IEnumerator SpawnIngredients()
    //{
    //    DetermineIngredientsToSpawn();

    //    while (ingredientsToSpawn.Count > 0)
    //    {
    //        //ingredientsToSpawn[1].SetActive(true);

    //        float force = 5f;
    //        float angle = 45f;

    //        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180) * force;
    //        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180) * force;

    //        ingredientsToSpawn[1].GetComponent<Rigidbody>().AddForce(ycomponent, 0, xcomponent);

    //        ingredientsToSpawn.RemoveAt(1);  //check logic - .Count()
    //    }

    //    yield return new WaitForSeconds(timeBetweenWaves);
    //}

    //void DetermineIngredientsToSpawn()
    //{
    //    foreach (var ingredient in ingredientsList)
    //    {
    //        int noOfIngredientsOfType = Random.Range(minIngredientOfType, maxIngredientOfType);

    //        for (int i = 0; i < noOfIngredientsOfType; i++)
    //        {
    //            GameObject clone = Instantiate(ingredient);
    //            //clone.SetActive(false);

    //            ingredientsToSpawn.Add(clone);
    //        }
    //    }
    //}

    //------------------------------------------------------------------------------------------------

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
        pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
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


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FountainBehaviour : MonoBehaviour
//{
//    [SerializeField] private int delay = 2;
//    [SerializeField] private float minRandomSpawnDelay = 1.5f, maxRandomSpawnDelay = 2f;
//    [SerializeField] private float minSpawnOff = 1.75f, maxSpawnOff = 3f;
//    [SerializeField] private AnimationCurve ac;
//    [SerializeField] private List<GameObject> ingredients;

//    private void Start()
//    {
//        StartCoroutine(nameof(Fountain));
//    }

//    private IEnumerator Fountain()
//    {
//        yield return new WaitForSeconds(delay);

//        while (true)
//        {
//            Vector3 center = gameObject.GetComponent<Renderer>().bounds.center;
//            center.y += 1.1f;
//            Vector3 pos = RandomCircle(center, Random.Range(minSpawnOff, maxSpawnOff));

//            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
//            GameObject clone = Instantiate(ingredients[(Random.Range(0, ingredients.Count))], center, rot);

//            Vector3 cloneCenter = clone.transform.position;

//            float privateRotation = 70f;    //between 1f and 89f inclusive
//            float dragDown = 10f;           //"speed"

//            // Calculate distance to target
//            float target_Distance = Vector3.Distance(cloneCenter, pos);

//            // Calculate the velocity needed to throw the object to the target at specified angle.
//            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * privateRotation * Mathf.Deg2Rad) / dragDown);

//            // Extract the X  Y componenent of the velocity
//            float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(privateRotation * Mathf.Deg2Rad);
//            float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(privateRotation * Mathf.Deg2Rad);

//            // Calculate flight time.
//            float flightDuration = target_Distance / Vx;

//            // Rotate projectile to face the target.
//            clone.transform.rotation = Quaternion.LookRotation(pos - clone.transform.position);

//            float elapse_time = 0;

//            while (elapse_time < flightDuration)
//            {
//                clone.transform.Translate(0, (Vy - (dragDown * elapse_time)) * Time.deltaTime * ac.Evaluate(elapse_time), Vx * Time.deltaTime * ac.Evaluate(elapse_time));

//                elapse_time += Time.deltaTime;

//                yield return null;
//            }

//            float waitTime = Random.Range(minRandomSpawnDelay, maxRandomSpawnDelay);
//            //Debug.Log(waitTime);
//            yield return new WaitForSeconds(waitTime);

//            //StartCoroutine(DelayAction(minRandomSpawnDelay, maxRandomSpawnDelay));
//        }
//    }

//    /// <summary>
//    /// Generate a random circle with a center and radius
//    /// </summary>
//    /// <param name="center">The center point of the circle</param>
//    /// <param name="radius">The radius of the circle (use Random.Range to create a ring)</param>
//    /// <returns>A circle</returns>
//    Vector3 RandomCircle(Vector3 center, float radius)
//    {
//        float angle = Random.value * 360;
//        Vector3 pos;
//        pos.x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
//        pos.y = center.y;
//        pos.z = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
//        return pos;
//    }

//    private IEnumerator DelayAction(int waitTime)
//    {
//        yield return new WaitForSeconds(waitTime);
//    }

//    private IEnumerator DelayAction(float minWaitTime, float maxWaitTime)
//    {
//        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
//    }
//}
