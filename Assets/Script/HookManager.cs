using UnityEngine;
using System.Collections;

public class HookManager : MonoBehaviour {
    // We fixed that maximumly 10 hooks.
    public static int max_hook = 10;
    // This marks if we actually stores a hook here
    private bool [] hook_valid = new bool[max_hook];
    // This stores the hook index in the simulator.
    private int [] hook_index = new int[max_hook];
    // This marks if the hook position is actually synced in the simulator.
    private bool [] hook_synced = new bool[max_hook];
    private Vector3 [] hook_position = new Vector3[max_hook];
    private Vector3 [] barycentric_coordinate = new Vector3[max_hook];
    private int [] triangleID = new int[max_hook];
    private GameObject[] hooks = new GameObject[max_hook];
    private int active_hook;
    public GameObject collision_indicator_object;
    public GameObject deformable_object;
    private CollisionIndicator collision_indicator;
	// Use this for initialization
	void Start () {
        collision_indicator = collision_indicator_object.GetComponent<CollisionIndicator> ();
	}
	
	// Update is called once per frame
	void Update () {
        if (collision_indicator.indicator_valid && (!collision_indicator.hook_mode)) {
            if (Input.GetMouseButtonDown (0)) {
                for (int i = 0; i < max_hook; ++i) {
                    if (!hook_valid [i]) {
                        hook_valid [i] = true;
                        hook_synced [i] = false;
                        hook_index [i] = -1;
                        hooks [i] = Instantiate (collision_indicator_object);
                        hook_position [i] = deformable_object.transform.InverseTransformPoint (hooks [i].transform.position);
                        triangleID [i] = collision_indicator.triangleID;
                        barycentric_coordinate [i] = collision_indicator.barycentric_coordinate;
                        active_hook = i;
                        collision_indicator.hook_mode = true;
                        break;
                    }
                }
            }
        } else if (collision_indicator.indicator_valid && collision_indicator.hook_mode) {
            hook_synced [active_hook] = false;
            hook_position [active_hook] = deformable_object.transform.InverseTransformPoint (collision_indicator_object.transform.position);   
            hooks [active_hook].transform.position = collision_indicator_object.transform.position;
        }else {
            active_hook = -1;
        }
        if (Input.GetMouseButtonUp (0)) {
            collision_indicator.hook_mode = false;
        }
	}
    public int Create_Hook(out Vector3 uv, out int triangle_index){
        uv = new Vector3();
        triangle_index = -1;
        for (int i = 0; i < max_hook; ++i) {
            if (hook_valid [i] && (!hook_synced [i]) && hook_index [i] == -1) {
                uv = barycentric_coordinate[i];
                triangle_index = triangleID [i];
                hook_synced [i] = true;
                return i;         
            }
        }
        return -1;
    }
    public void Set_Hook_Index(int id, int index){
        hook_index [id] = index;
    }
    public int Get_Unsynced_Hook(out Vector3 position){
        position = new Vector3();
        for (int i = 0; i < max_hook; ++i) {
            if (hook_valid [i] && (!hook_synced [i]) && hook_index [i] != -1) {
                position = hook_position[i];
                hook_synced [i] = true;
                return hook_index [i];         
            }
        }
        return -1;
    }
}
