using UnityEngine;
using System.Collections;

public class ExplosionsDeactivateRendererByTime : MonoBehaviour {

    public float TimeDelay = 1;
    private Renderer rend;
    
	void Awake () {
        rend = GetComponent<Renderer>();
	}
	
	void DeactivateRenderer () {
        rend.enabled = false;
	}

    void OnEnable()
    {
        rend.enabled = true;
        Invoke("DeactivateRenderer", TimeDelay);
    }
}
