using UnityEngine;

public class UCameraController : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 16.7f, -25.5f);
    public Transform target;
    public float speed=30;
    bool yet = false;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target!=null)
        {
            yet = true;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Follow();
    }

    public void Follow() {
        if (yet==true)
        {
            var targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }
}
