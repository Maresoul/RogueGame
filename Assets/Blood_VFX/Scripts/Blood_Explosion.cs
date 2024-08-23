using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood_Explosion : MonoBehaviour
{

    public GameObject explodeFX;
    private bool explodeCheck = false;


    void Start()
    {

        explodeFX.SetActive(false);

    }
        

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            if (explodeCheck == false)

            {
                StartCoroutine("Explosion");
                explodeCheck = true;
            }

        }

    }


    IEnumerator Explosion()
    {

        explodeFX.SetActive(true);

        yield return new WaitForSeconds(7.0f);

        explodeFX.SetActive(false);
        explodeCheck = false;

    }


}

