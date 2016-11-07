using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;

public class Networking : MonoBehaviour {
  enum COMMAND:byte {CREATE_MODEL=1};
	//public string m_IPAdress = "128.105.19.20";//"cantor.cs.wisc.edu";
  //public string m_IPAdress = "54.174.141.212";//"amazon ec2"
  public string m_IPAdress = "128.105.19.12";//"dollitle.cs.wisc.edu"
  //public string m_IPAdress = "127.0.0.1";
  public int kPort = 12345;
  public GameObject deformable_object;
  private MeshFilter deformable_mesh_filter;
	// Use this for initialization
  private TcpClient client;
	void Start () {
    client = new TcpClient(m_IPAdress, kPort);
    NetworkStream stream = client.GetStream();
    // CREATE_MODEL
    byte[] data = new byte[1];
    data[0] = (byte)COMMAND.CREATE_MODEL;
    stream.Write(data, 0, 1);
    // SEND VERTICES
    deformable_mesh_filter = deformable_object.GetComponent<MeshFilter> ();
    Vector3[] deformable_vertices=deformable_mesh_filter.mesh.vertices;
    int number_of_vertices = deformable_vertices.Length;
    stream.Write(BitConverter.GetBytes(number_of_vertices), 0, 4);
    Debug.Log("number_of_vertices: "+number_of_vertices);
    byte[] vertex_buffer = new byte[number_of_vertices * 3 * 4];
    for (int i = 0; i < number_of_vertices; i++) {
      BitConverter.GetBytes (deformable_vertices [i].x).CopyTo (vertex_buffer,i * 3 * 4);
      BitConverter.GetBytes (deformable_vertices [i].y).CopyTo (vertex_buffer,(i * 3 + 1) * 4);
      BitConverter.GetBytes (deformable_vertices [i].z).CopyTo (vertex_buffer,(i * 3 + 2) * 4);
      //Debug.Log (i + deformable_vertices [i].ToString("F6"));
    }
    stream.Write(vertex_buffer, 0, number_of_vertices * 3 * 4);
    // SEND TRIANGLES
    int[] deformable_triangles=deformable_mesh_filter.mesh.triangles;
    int number_of_triangles_times_three = deformable_triangles.Length;
    stream.Write(BitConverter.GetBytes(number_of_triangles_times_three), 0, 4);
    byte[] triangle_buffer = new byte[number_of_triangles_times_three * 4];
    for (int i = 0; i < number_of_triangles_times_three; i++) {
      BitConverter.GetBytes (deformable_triangles [i]).CopyTo (triangle_buffer,i * 4);
    }
    //for (int i = 0; i < number_of_triangles_times_three / 3; i++) {
      //Debug.Log (i + ": " +deformable_triangles [i*3] + ", " + deformable_triangles [i*3+1] + ", " + deformable_triangles [i*3+2]);
    //}
    stream.Write(triangle_buffer, 0, number_of_triangles_times_three * 4);
  }
	// Update is called once per frame
	void Update () {
	
	}
}
