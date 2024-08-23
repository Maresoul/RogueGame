using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood_Pool : MonoBehaviour
{

    public GameObject bloodPool;
    private bool poolCheck = false;

    void Start()
    {

        bloodPool.SetActive(false);

    }
        

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            print("pool");
            if (poolCheck == false)

            {
                StartCoroutine("StartBloodPool");
                poolCheck = true;
            }

        }

    }


    IEnumerator StartBloodPool()
    {

        // Set blood pool at random orientation
        bloodPool.transform.Rotate(0, (Random.Range(1, 360)), 0);

        bloodPool.SetActive(true);

        yield return new WaitForSeconds(20.0f);

        bloodPool.SetActive(false);


        poolCheck = false;

    }



}

