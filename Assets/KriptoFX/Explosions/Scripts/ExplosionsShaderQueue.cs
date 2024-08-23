using UnityEngine;
using System.Collections;

public class ExplosionsShaderQueue : MonoBehaviour
{
    public int AddQueue = 1;
    private Renderer rend;
	// Use this for initialization
    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend!=null)
            rend.sharedMaterial.renderQueue += AddQueue;
        else
            Invoke("SetProjectorQueue", 0.1f);
    }

    void SetProjectorQueue()
  {
    GetComponent<Projector>().material.renderQueue += AddQueue;
  }
	
	// Update is called once per frame
    void OnDisable()
    {
        if (rend != null)
            rend.sharedMaterial.renderQueue = -1;
	}
}
