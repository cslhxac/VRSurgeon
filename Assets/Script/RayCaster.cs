using UnityEngine;
using System.Collections;

public class RayCaster : MonoBehaviour {
    private Camera camera;
    public GameObject collision_indicator;
    public GameObject suture_manager_object;
    private SutureManager suture_manager;
    private CollisionIndicator indicator;
    private MeshRenderer indicator_render;
    private float hook_offset; 
    private bool suture_first_point_down;
    private Vector3 suture_first_point_uv;
    private int suture_first_point_triangle_id;
	// Use this for initialization
	void Start () {
      camera = this.GetComponent<Camera>();
        indicator = collision_indicator.GetComponent<CollisionIndicator> ();
        indicator_render = collision_indicator.GetComponent<MeshRenderer> ();
        suture_manager = suture_manager_object.GetComponent<SutureManager> ();
	}
    void Update (){
        if (Input.GetKeyDown ("space")) {
            if (suture_first_point_down) {
                suture_manager.Create_Suture (suture_first_point_triangle_id,suture_first_point_uv,
                    indicator.triangleID,indicator.barycentric_coordinate);
                suture_first_point_down = false;
            } else {
                suture_first_point_uv = indicator.barycentric_coordinate;
                suture_first_point_triangle_id = indicator.triangleID;
                suture_first_point_down = true;
            }
        }
    }
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
