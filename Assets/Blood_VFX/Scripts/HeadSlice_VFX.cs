using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSlice_VFX : MonoBehaviour
{

    public Animator anim;
    public Rigidbody playerRb;
    public CapsuleCollider capsuleCollider;
    public GameObject wallBloodSpatterVFX;
    public GameObject headSliceVFX;
    public GameObject headSlicePiece;


    private Rigidbody[] rigidbodies; private Collider[] colliders;


    void Start()
    {

        wallBloodSpatterVFX.SetActive(false);
        headSliceVFX.SetActive(false);

        // headPiece.GetComponent<Rigidbody>().Sleep();
        //  headPiece.GetComponent<Rigidbody>().useGravity = false;

        SetRigidbodiesKinematic(true);
        

    }


    private void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        // constForce = GetComponentsInChildren<ConstantForce>();

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

        wallBloodSpatterVFX.SetActive(true);
        headSliceVFX.SetActive(true);

        headSlicePiece.transform.parent = null;
        headSlicePiece.GetComponent<Rigidbody>().useGravity = true;

        capsuleCollider.enabled = false;
        anim.enabled = false;

        SetRigidbodiesKinematic(false);
        SetCollidersEnabled(true);

        yield return new WaitForSeconds(0.035f);

        SetRigidbodiesRemoveVelocity();

        // Transorm and rotate the cut off piece of the head
        headSlicePiece.GetComponent<Rigidbody>().AddForce(Random.Range(-1f, 2f), Random.Range(1.0f, 2.0f), Random.Range(-2f, -3f), ForceMode.Impulse);
        headSlicePiece.GetComponent<Rigidbody>().AddTorque(transform.up * (Random.Range(-100f, 100f)));

    }

}