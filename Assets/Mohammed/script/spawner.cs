using System.Collections;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject tup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(timer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Instantiate(tup, transform.position, transform.rotation);
        }
    }
}
