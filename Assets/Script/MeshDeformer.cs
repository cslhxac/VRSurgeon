using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {
	Mesh deformingMesh;
	Vector3[] originalVertices;
	Vector3[] displacedVertices;
	// Use this for initialization
	void Start () {
		deformingMesh = GetComponent<MeshFilter> ().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices [i] = originalVertices [i];
		}
    this.transform.GetComponent<MeshCollider>().sharedMesh = deformingMesh;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		for (int i = 0; i < displacedVertices.Length; i++) {
			//displacedVertices[i] += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));;
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
    //this.transform.GetComponent<MeshCollider>().sharedMesh = deformingMesh;
    this.transform.Translate (Vector3.forward*(0.1f));
	}
}
