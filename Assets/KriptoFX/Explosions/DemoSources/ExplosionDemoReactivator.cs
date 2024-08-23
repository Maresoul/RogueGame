using UnityEngine;
using System.Collections;

public class ExplosionDemoReactivator : MonoBehaviour
{

	public float TimeDelayToReactivate = 3;
	// Use this for initialization
	void Start () {
		InvokeRepeating("Reactivate", 0, TimeDelayToReactivate );
	}
	
	// Update is called once per frame
	void Reactivate ()
	{
	  var childs = GetComponentsInChildren<Transform>();
	  foreach (var child in childs) {
	    child.gameObject.SetActive(false);
      child.gameObject.SetActive(true);
	  }
	}
}
