using UnityEngine;
using System.Collections;

public class RayCaster : MonoBehaviour {
    private Camera camera;
    public GameObject collision_indicator;
    private CollisionIndicator indicator;
    private MeshRenderer indicator_render;
    private float hook_offset; 
	// Use this for initialization
	void Start () {
      camera = this.GetComponent<Camera>();
        indicator = collision_indicator.GetComponent<CollisionIndicator> ();
        indicator_render = collision_indicator.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
  void FixedUpdate () {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!indicator.hook_mode) {
            if (Physics.Raycast (ray, out hit)) {
                collision_indicator.transform.position = hit.point;
                indicator.indicator_valid = true;
                indicator_render.enabled = true;
                indicator.triangleID = hit.triangleIndex;
                indicator.barycentric_coordinate = hit.barycentricCoordinate;
                hook_offset = hit.distance;
            } else {
                indicator.indicator_valid = false;
                indicator_render.enabled = false;
            }
        } else {
            Debug.Log ("in hook_mode!");
            collision_indicator.transform.position = ray.origin + ray.direction.normalized * hook_offset;
        }
	}
}
