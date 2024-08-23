using UnityEngine;
using System.Collections;

public class ExplosionsBillboard : MonoBehaviour
{
  public Camera Camera;
  public bool Active = true;
  public bool AutoInitCamera = true;

  private GameObject myContainer;
  private Transform t, camT, contT;

  private void Awake()
  {
    if (AutoInitCamera) {
      Camera = Camera.main;
      Active = true;
    }

    t = transform;
    var scale = t.parent.transform.localScale;
    scale.z = scale.x;
    t.parent.transform.localScale = scale;
    camT = Camera.transform;
    var parent = t.parent;
    myContainer = new GameObject { name = "Billboard_" + t.gameObject.name };
    contT = myContainer.transform;
    contT.position = t.position;
    t.parent = myContainer.transform;
    contT.parent = parent;
  }

  private void Update()
  {
    if (Active)
      contT.LookAt(contT.position + camT.rotation * Vector3.back, camT.rotation * Vector3.up);
  }
}