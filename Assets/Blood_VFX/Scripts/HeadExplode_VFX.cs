using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadExplode_VFX : MonoBehaviour
{

    public Animator anim;
    public Rigidbody playerRb;
    public CapsuleCollider capsuleCollider;
    public GameObject head;
    public GameObject neck;

    private Rigidbody[] rigidbodies; 
    private Collider[] colliders;


    void Start()
    {

        neck.SetActive(false);
        SetRigidbodiesKinematic(true);
        
    }


    private void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        SetCollidersEnabled(false);
        SetRigidbodiesKinematic(true);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            StartCoroutine("ActivateRagdoll");

        }
    }

    private void SetCollidersEnabled(bool enabled)
    {

        foreach (Collider col in colliders)
        {

            col.enabled = enabled;
 

        }
    }



    private void SetRigidbodiesKinematic(bool kinematic)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = kinematic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void SetRigidbodiesRemoveVelocity()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    IEnumerator ActivateRagdoll()
    {

        neck.SetActive(true);
        head.SetActive(false);

        capsuleCollider.enabled = false;
        anim.enabled = false;

        SetRigidbodiesKinematic(false);
        SetCollidersEnabled(true);

        yield return new WaitForSeconds(0.035f);

        SetRigidbodiesRemoveVelocity();


    }

}