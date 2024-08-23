using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDecapitate_VFX : MonoBehaviour
{

    public Animator anim;
    public Rigidbody playerRb;
    public CapsuleCollider capsuleCollider;
    public GameObject decapitationStumpVFX;
    public GameObject decapitationHeadVFX;
    public GameObject head;
    public GameObject neck;

    private Rigidbody[] rigidbodies; private Collider[] colliders;



    void Start()
    {
        decapitationStumpVFX.SetActive(false);
        decapitationHeadVFX.SetActive(false);
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

        decapitationStumpVFX.SetActive(true);
        decapitationHeadVFX.SetActive(true);
        neck.SetActive(true);

        head.transform.parent = null;
        head.GetComponent<Rigidbody>().useGravity = true;

        capsuleCollider.enabled = false;
        anim.enabled = false;

        SetRigidbodiesKinematic(false);
        SetCollidersEnabled(true);

        yield return new WaitForSeconds(0.035f);

        SetRigidbodiesRemoveVelocity();

        head.GetComponent<Rigidbody>().AddForce(Random.Range(-0.5f, 0.5f), Random.Range(8.0f, 10.0f), Random.Range(-1f, 1f), ForceMode.Impulse);
        head.GetComponent<Rigidbody>().AddTorque(transform.up * (Random.Range(-100f, 100f)));

    }

}