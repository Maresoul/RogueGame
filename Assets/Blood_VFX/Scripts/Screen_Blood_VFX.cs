using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Blood_VFX : MonoBehaviour
{

    public GameObject bloodSpatter;

    private bool finishCheck = false;


    void Start()
    {

        bloodSpatter.SetActive(false);

    }
        

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {


            if (finishCheck == false)

            {
                StartCoroutine("ScreenBlood");
                finishCheck = true;
            }

        }

    }


    IEnumerator ScreenBlood()
    {

        bloodSpatter.SetActive(true);

        yield return new WaitForSeconds(7.0f);

        bloodSpatter.SetActive(false);

        finishCheck = false;

    }



}

