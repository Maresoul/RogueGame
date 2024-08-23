using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadShot_VFX : MonoBehaviour {

    public Animator anim;
    public Rigidbody playerRb;
    public CapsuleCollider capsuleCollider;
    public GameObject bloodSpatterVFX;
    public GameObject bulletWound;

    private Rigidbody[] rigidbodies; 
    private Collider[] colliders;

    void Start()
    {
        bloodSpatterVFX.SetActive(false);
        bulletWound.SetActive(false);
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

        bulletWound.SetActive(true);
        bloodSpatterVFX.SetActive(true);
        
        capsuleCollider.enabled = false;
        anim.enabled = false;

        SetRigidbodiesKinematic(false);
        SetCollidersEnabled(true);

        yield return new WaitForSeconds(0.03f);

        SetRigidbodiesRemoveVelocity();
        SetCollidersEnabled(true);
        SetRigidbodiesKinematic(false);

    }

}