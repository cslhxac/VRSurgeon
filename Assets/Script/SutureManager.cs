using UnityEngine;
using System.Collections;

public class SutureManager : MonoBehaviour {
    // We fixed that maximumly 20 sutures.
    public static int max_suture = 20;
    // This marks if we actually stores a hook here
    private bool [] suture_valid = new bool[max_suture];
    // This stores the hook index in the simulator.
    private int [] suture_index = new int[max_suture];
    // This marks if the hook position is actually synced in the simulator.
    private bool [] suture_synced = new bool[max_suture];
    private Vector3 [] barycentric_coordinate1 = new Vector3[max_suture];
    private Vector3 [] barycentric_coordinate2 = new Vector3[max_suture];
    private int [] triangleID1 = new int[max_suture];
    private int [] triangleID2 = new int[max_suture];
    private GameObject[] suture = new GameObject[max_suture];
    private LineRenderer[] suture_renderer = new LineRenderer[max_suture];
    // This is used to compute the suture position
    public GameObject deformable_object;
	// Use this for initialization
    public GameObject suture_prefab;
	void Start () {
        for (int i = 0; i < max_suture; ++i) {
            suture_valid [i] = false;
            suture_synced [i] = true;
            suture [i] = Instantiate (suture_prefab);
            suture_renderer [i] = suture [i].GetComponent<LineRenderer> ();
            suture_renderer [i].enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Create_Suture(int triangleID1_in, Vector3 uv1_in,
        int triangleID2_in, Vector3 uv2_in){
        for (int i = 0; i < max_suture; ++i) {
            if (!suture_valid [i]) {
                suture_valid [i] = true;
                suture_synced [i] = false;
                suture_index [i] = -1;
                suture [i] = Instantiate (suture_prefab);
                suture_renderer [i].enabled = true;
                triangleID1 [i] = triangleID1_in;
                triangleID2 [i] = triangleID2_in;
                barycentric_coordinate1 [i] = uv1_in;
                barycentric_coordinate2 [i] = uv2_in;
                break;
            }
        }
    }
    public int Get_Unsynced_Suture(out int triangleID1_out, out Vector3 uv1_out,
        out int triangleID2_out, out Vector3 uv2_out){
        uv1_out = new Vector3();
        uv2_out = new Vector3();
        triangleID1_out = -1;
        triangleID2_out = -1;
        for (int i = 0; i < max_suture; ++i) {
            if (suture_valid [i] && (!suture_synced [i]) && suture_index [i] == -1) {
                uv1_out = barycentric_coordinate1[i];
                uv2_out = barycentric_coordinate2[i];
                triangleID1_out = triangleID1 [i];
                triangleID2_out = triangleID2 [i];
                suture_synced [i] = true;
                return i;         
            }
        }
        return -1;
    }
    public void Delete_Suture(int index){
        if (suture_valid [index]) {
            suture_valid [index] = false;
            suture_synced [index] = true;
            suture_renderer [index].enabled = true;
        }
    }
    public void Delete_Suture(GameObject suture_in){
        for (int i = 0; i < max_suture; ++i) {
            if (suture [i] == suture_in) {
                if (suture_valid [i]) {
                    suture_valid [i] = false;
                    suture_synced [i] = true;
                    suture_renderer [i].enabled = true;
                }
                break;           
            }
        }
    }
    public int Get_Deleted_Suture(){
        for (int i = 0; i < max_suture; ++i) {
            if (!suture_valid [i] && !suture_synced [i]) {
                suture_synced[i] = true;
                return suture_index [i];
            }
        }
        return -1;
    }
    public void Update_Suture_Rendering_Position(int index, Vector3 p1, Vector3 p2){
        Vector3[] tmp = new Vector3[2];
        tmp [0] = p1;
        tmp [1] = p1;
        suture_renderer[index].SetPositions (tmp);
    }
    public void Set_Suture_Index(int index,int simulation_index){
        suture_index [index] = simulation_index;
    }
}
