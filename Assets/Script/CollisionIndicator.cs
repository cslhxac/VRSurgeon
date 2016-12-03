using UnityEngine;
using System.Collections;

public class CollisionIndicator : MonoBehaviour {
    public bool indicator_valid;
    public bool hook_mode;
    public int triangleID;
    public Vector3 barycentric_coordinate;
	// Use this for initialization
	void Start () {
        indicator_valid = false;
        hook_mode = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
