using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;

public class Networking : MonoBehaviour {
  enum COMMAND:byte {CREATE_MODEL=1,SET_FIXED_GEOMETRY=2,FIXED_POINTS=3};
	//public string m_IPAdress = "128.105.19.20";//"cantor.cs.wisc.edu";
  //public string m_IPAdress = "54.174.141.212";//"amazon ec2"
  public string m_IPAdress = "128.105.19.12";//"dollitle.cs.wisc.edu"
  //public string m_IPAdress = "127.0.0.1";
  public int kPort = 12345;
  public GameObject deformable_object;
  public GameObject fixed_geometry;
  public GameObject scene_descriptor;
  private MeshFilter deformable_mesh_filter;
  private MeshFilter fixed_geometry_mesh_filter;

	// Use this for initialization
  private TcpClient client;
  void Create_Model(){
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
      BitConverter.GetBytes (deformable_vertices [i].z).CopyTo (vertex_buffer,(i * 3 + 2) * 4);}
    stream.Write(vertex_buffer, 0, number_of_vertices * 3 * 4);
    // SEND TRIANGLES
    int[] deformable_triangles=deformable_mesh_filter.mesh.triangles;
    int number_of_triangles_times_three = deformable_triangles.Length;
    stream.Write(BitConverter.GetBytes(number_of_triangles_times_three), 0, 4);
    byte[] triangle_buffer = new byte[number_of_triangles_times_three * 4];
    for (int i = 0; i < number_of_triangles_times_three; i++) {
      BitConverter.GetBytes (deformable_triangles [i]).CopyTo (triangle_buffer,i * 4);}
    stream.Write(triangle_buffer, 0, number_of_triangles_times_three * 4);
    Scene_Descriptor descriptor=scene_descriptor.GetComponent<Scene_Descriptor> ();
    stream.Write(BitConverter.GetBytes(descriptor.dx), 0, 4);
    stream.Write(BitConverter.GetBytes(descriptor.youngs_modulus), 0, 4);
    stream.Write(BitConverter.GetBytes(descriptor.poissons_ratio), 0, 4);
    stream.Write(BitConverter.GetBytes(descriptor.refinement), 0, 4);
  }
  void Set_Fixed_Points(){
    NetworkStream stream = client.GetStream();
    byte[] data = new byte[1];
    data[0] = (byte)COMMAND.SET_FIXED_GEOMETRY;
    stream.Write(data, 0, 1);
    data[0] = (byte)COMMAND.FIXED_POINTS;
    stream.Write(data, 0, 1);
    //Setting Fixed Grometry
    data[0] = (byte)COMMAND.SET_FIXED_GEOMETRY;
    stream.Write(data, 0, 1);
    fixed_geometry_mesh_filter = fixed_geometry.GetComponent<MeshFilter> ();
    //SEND VERTICES
    Vector3[] fixed_geometry_mesh_filter_vertices=deformable_mesh_filter.mesh.vertices;
    int number_of_vertices = fixed_geometry_mesh_filter_vertices.Length;
    stream.Write(BitConverter.GetBytes(number_of_vertices), 0, 4);
    byte[] vertex_buffer = new byte[number_of_vertices * 3 * 4];
    for (int i = 0; i < number_of_vertices; i++) {
      BitConverter.GetBytes (fixed_geometry_mesh_filter_vertices [i].x).CopyTo (vertex_buffer,i * 3 * 4);
      BitConverter.GetBytes (fixed_geometry_mesh_filter_vertices [i].y).CopyTo (vertex_buffer,(i * 3 + 1) * 4);
      BitConverter.GetBytes (fixed_geometry_mesh_filter_vertices [i].z).CopyTo (vertex_buffer,(i * 3 + 2) * 4);}
    stream.Write(vertex_buffer, 0, number_of_vertices * 3 * 4);
  }
	void Start () {
    client = new TcpClient(m_IPAdress, kPort);
    Create_Model ();
    Set_Fixed_Points ();
  }
	// Update is called once per frame
	void Update () {
	
	}
}
