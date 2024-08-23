using UnityEngine;
using System.Collections;

public class ExplosionPlatformActivator : MonoBehaviour
{

    public GameObject Effect;
    public float TimeDelay = 0;
    public float DefaultRepeatTime = 5;
    public float NearRepeatTime = 3;

    private float currentTime, currentRepeatTime;
    private bool canUpdate;

    void Start()
    {
        currentRepeatTime = DefaultRepeatTime;
        Invoke("Init", TimeDelay);
    }

    void Init()
    {
        canUpdate = true;
        Effect.SetActive(true);
    }

    void Update()
    {
        if (!canUpdate || Effect==null)
            return;
        currentTime += Time.deltaTime;
        if (currentTime > currentRepeatTime) {
            currentTime = 0;
            Effect.SetActive(false);
            Effect.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        currentRepeatTime = NearRepeatTime;
    }

    void OnTriggerExit(Collider other)
    {
        currentRepeatTime = DefaultRepeatTime;
    }
}
