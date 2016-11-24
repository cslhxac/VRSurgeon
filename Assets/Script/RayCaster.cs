using UnityEngine;
using System.Collections;

public class RayCaster : MonoBehaviour {
  private Camera camera;
  public GameObject collision_indicator;
	// Use this for initialization
	void Start () {
    camera = this.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
    RaycastHit hit;
    Ray ray = camera.ScreenPointToRay(Input.mousePosition);

    if (Physics.Raycast(ray, out hit)) {
      collision_indicator.transform.position = hit.point;

      // Do something with the object that was hit by the raycast.
    }
	}
}
