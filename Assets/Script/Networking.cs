using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class Networking : MonoBehaviour
{
    enum COMMAND : byte { CREATE_MODEL = 1, SET_FIXED_GEOMETRY = 2, FIXED_POINTS = 3, ADD_HOOK = 5, MOVE_HOOK = 6, REMOVE_HOOK = 7, START_SIMULATION=8, REQUEST_UPDATE=9, ADD_SUTURE=10, REMOVE_SUTURE=11, SET_COLLISION_GEOMETRY=12 };
    //public string m_IPAdress = "128.105.19.20";//"cantor.cs.wisc.edu";
    //public string m_IPAdress = "54.174.141.212";//"amazon ec2"
    public string m_IPAdress = "128.105.19.12";//"dollitle.cs.wisc.edu"
                                               //public string m_IPAdress = "127.0.0.1";
    public int kPort = 12345;
    public GameObject deformable_object;
    public GameObject fixed_geometry;
    public GameObject collision_object;
    public GameObject scene_descriptor;
    public GameObject hook_manager_object;
    public GameObject suture_manager_object;
    private HookManager hook_manager;
    private SutureManager suture_manager;
    private MeshFilter deformable_mesh_filter;
    private MeshFilter fixed_geometry_mesh_filter;
    private MeshFilter collision_object_mesh_filter;
    private MeshCollider deformable_collider;
    // Use this for initialization
    private TcpClient client;
    void Set_Collision_Geometry(){
        NetworkStream stream = client.GetStream();
        // SET_COLLISION_GEOMETRY
        byte[] data = new byte[1];
        data[0] = (byte)COMMAND.SET_COLLISION_GEOMETRY;
        stream.Write(data, 0, 1);
        // SEND VERTICES
        collision_object_mesh_filter = collision_object.GetComponent<MeshFilter>();
        Vector3[] collision_vertices = collision_object_mesh_filter.mesh.vertices;
        int number_of_vertices = collision_vertices.Length;
        stream.Write(BitConverter.GetBytes(number_of_vertices), 0, 4);
        byte[] vertex_buffer = new byte[number_of_vertices * 3 * 4];
        for (int i = 0; i < number_of_vertices; i++)
        {
            BitConverter.GetBytes(collision_vertices[i].x).CopyTo(vertex_buffer, i * 3 * 4);
            BitConverter.GetBytes(collision_vertices[i].y).CopyTo(vertex_buffer, (i * 3 + 1) * 4);
            BitConverter.GetBytes(collision_vertices[i].z).CopyTo(vertex_buffer, (i * 3 + 2) * 4);
        }
        stream.Write(vertex_buffer, 0, number_of_vertices * 3 * 4);
        // SEND TRIANGLES
        int[] collision_triangles = collision_object_mesh_filter.mesh.triangles;
        int number_of_triangles_times_three = collision_triangles.Length;
        //Debug.Log("number_of_triangles_times_three: " + number_of_triangles_times_three);
        stream.Write(BitConverter.GetBytes(number_of_triangles_times_three), 0, 4);
        byte[] triangle_buffer = new byte[number_of_triangles_times_three * 4];
        for (int i = 0; i < number_of_triangles_times_three; i++)
        {
            BitConverter.GetBytes(collision_triangles[i]).CopyTo(triangle_buffer, i * 4);
        }
        stream.Write(triangle_buffer, 0, number_of_triangles_times_three * 4);
    }
    void Create_Model()
    {
        NetworkStream stream = client.GetStream();
        // CREATE_MODEL
        byte[] data = new byte[1];
        data[0] = (byte)COMMAND.CREATE_MODEL;
        stream.Write(data, 0, 1);
        // SEND VERTICES
        deformable_mesh_filter = deformable_object.GetComponent<MeshFilter>();
        Vector3[] deformable_vertices = deformable_mesh_filter.mesh.vertices;
        int number_of_vertices = deformable_vertices.Length;
        stream.Write(BitConverter.GetBytes(number_of_vertices), 0, 4);
        Debug.Log("number_of_vertices: "+number_of_vertices);
        byte[] vertex_buffer = new byte[number_of_vertices * 3 * 4];
        for (int i = 0; i < number_of_vertices; i++)
        {
            BitConverter.GetBytes(deformable_vertices[i].x).CopyTo(vertex_buffer, i * 3 * 4);
            BitConverter.GetBytes(deformable_vertices[i].y).CopyTo(vertex_buffer, (i * 3 + 1) * 4);
            BitConverter.GetBytes(deformable_vertices[i].z).CopyTo(vertex_buffer, (i * 3 + 2) * 4);
        }
        stream.Write(vertex_buffer, 0, number_of_vertices * 3 * 4);
        // SEND TRIANGLES
        int[] deformable_triangles = deformable_mesh_filter.mesh.triangles;
        int number_of_triangles_times_three = deformable_triangles.Length;
        //Debug.Log("number_of_triangles_times_three: " + number_of_triangles_times_three);
        stream.Write(BitConverter.GetBytes(number_of_triangles_times_three), 0, 4);
        byte[] triangle_buffer = new byte[number_of_triangles_times_three * 4];
        for (int i = 0; i < number_of_triangles_times_three; i++)
        {
            BitConverter.GetBytes(deformable_triangles[i]).CopyTo(triangle_buffer, i * 4);
        }
        stream.Write(triangle_buffer, 0, number_of_triangles_times_three * 4);
        Scene_Descriptor descriptor = scene_descriptor.GetComponent<Scene_Descriptor>();
        stream.Write(BitConverter.GetBytes(descriptor.dx), 0, 4);
        stream.Write(BitConverter.GetBytes(descriptor.youngs_modulus), 0, 4);
        stream.Write(BitConverter.GetBytes(descriptor.poissons_ratio), 0, 4);
        stream.Write(BitConverter.GetBytes(descriptor.refinement), 0, 4);
        stream.Write(BitConverter.GetBytes(descriptor.hook_stiffness), 0, 4);
        stream.Write(BitConverter.GetBytes(descriptor.suture_stiffness), 0, 4);
    }
    void Set_Fixed_Points()
    {
        NetworkStream stream = client.GetStream();
        byte[] data = new byte[1];
        //Setting Fixed Grometry
        data[0] = (byte)COMMAND.SET_FIXED_GEOMETRY;
        stream.Write(data, 0, 1);
        data[0] = (byte)COMMAND.FIXED_POINTS;
        stream.Write(data, 0, 1);
        fixed_geometry_mesh_filter = fixed_geometry.GetComponent<MeshFilter>();
        //SEND VERTICES
        Vector3[] fixed_geometry_vertices = fixed_geometry_mesh_filter.mesh.vertices;
        int number_of_vertices = fixed_geometry_vertices.Length;
        stream.Write(BitConverter.GetBytes(number_of_vertices), 0, 4);
        Debug.Log("number_of_fixed_vertices: " + number_of_vertices);
        byte[] vertex_buffer = new byte[number_of_vertices * 3 * 4];
        for (int i = 0; i < number_of_vertices; i++)
        {
            BitConverter.GetBytes(fixed_geometry_vertices[i].x).CopyTo(vertex_buffer, i * 3 * 4);
            BitConverter.GetBytes(fixed_geometry_vertices[i].y).CopyTo(vertex_buffer, (i * 3 + 1) * 4);
            BitConverter.GetBytes(fixed_geometry_vertices[i].z).CopyTo(vertex_buffer, (i * 3 + 2) * 4);
        }
        stream.Write(vertex_buffer, 0, number_of_vertices * 3 * 4);
    }
    void Start_Simulation()
    {
        NetworkStream stream = client.GetStream();
        // CREATE_MODEL
        byte[] data = new byte[1];
        data[0] = (byte)COMMAND.START_SIMULATION;
        stream.Write(data, 0, 1);
    }
    volatile bool request_in_fly = false;
    volatile bool mesh_updated = false;
    Vector3[] vertex_positions;
    byte[] vertices_buffer;
    void Request_Frame()
    {
        NetworkStream stream = client.GetStream();
        byte[] data = new byte[1];
        byte[] int_buffer = new byte[4];
        Vector3 hook_uv;
        int triangle_id;
        int hook_id;
        // See if we need to add hook
        hook_id = hook_manager.Create_Hook (out hook_uv,out triangle_id);
        if (hook_id != -1) {
            //Create Hook
            data[0] = (byte)COMMAND.ADD_HOOK;
            stream.Write(data, 0, 1);
            stream.Write(BitConverter.GetBytes(triangle_id), 0, 4);
            byte[] uv = new byte[3 * 4];
            BitConverter.GetBytes(hook_uv.x).CopyTo(uv, 0);
            BitConverter.GetBytes(hook_uv.y).CopyTo(uv, 4);
            BitConverter.GetBytes(hook_uv.z).CopyTo(uv, 8);
            stream.Write(uv, 0, 3 * 4);
            stream.Read(int_buffer, 0, 4);
            int hook_index = BitConverter.ToInt32 (int_buffer, 0);
            hook_manager.Set_Hook_Index (hook_id, hook_index);
        }
        Vector3 hook_position;
        // See if we need to update hook
        hook_id = hook_manager.Get_Unsynced_Hook (out hook_position);
        if (hook_id != -1) {
            //Move Hook
            data[0] = (byte)COMMAND.MOVE_HOOK;
            stream.Write(data, 0, 1);
            stream.Write(BitConverter.GetBytes(hook_id), 0, 4);
            byte[] position_buffer = new byte[3 * 4];
            BitConverter.GetBytes(hook_position.x).CopyTo(position_buffer, 0);
            BitConverter.GetBytes(hook_position.y).CopyTo(position_buffer, 4);
            BitConverter.GetBytes(hook_position.z).CopyTo(position_buffer, 8);
            stream.Write(position_buffer, 0, 3 * 4);
        }
        // Now Sutures
        Vector3 suture_uv2;
        int triangle_id2;
        int suture_id = suture_manager.Get_Unsynced_Suture (out triangle_id,out hook_uv,
            out triangle_id2,out suture_uv2);
        if (suture_id != -1) {
            data[0] = (byte)COMMAND.ADD_SUTURE;
            stream.Write(data, 0, 1);
            stream.Write(BitConverter.GetBytes(triangle_id), 0, 4);
            byte[] uv = new byte[3 * 4];
            BitConverter.GetBytes(hook_uv.x).CopyTo(uv, 0);
            BitConverter.GetBytes(hook_uv.y).CopyTo(uv, 4);
            BitConverter.GetBytes(hook_uv.z).CopyTo(uv, 8);
            stream.Write(uv, 0, 3 * 4);
            stream.Write(BitConverter.GetBytes(triangle_id2), 0, 4);
            BitConverter.GetBytes(suture_uv2.x).CopyTo(uv, 0);
            BitConverter.GetBytes(suture_uv2.y).CopyTo(uv, 4);
            BitConverter.GetBytes(suture_uv2.z).CopyTo(uv, 8);
            stream.Write(uv, 0, 3 * 4);
            stream.Read(int_buffer, 0, 4);
            int suture_index = BitConverter.ToInt32 (int_buffer, 0);
            suture_manager.Set_Suture_Index(suture_id, suture_index);
        }
        data[0] = (byte)COMMAND.REQUEST_UPDATE;
        stream.Write(data, 0, 1);
        stream.Read(int_buffer, 0, 4);
        int n_vertices = BitConverter.ToInt32 (int_buffer, 0);
        vertices_buffer = new byte[4 * 3 * n_vertices];
        int offset = 0;
        int total_data_size = 4 * 3 * n_vertices;
        do
        {
            offset += stream.Read(vertices_buffer, offset, total_data_size - offset);
        } while (offset < total_data_size);
        for (int i = 0; i < n_vertices; i++) {
            vertex_positions[i].x = BitConverter.ToSingle (vertices_buffer, i * 3 * 4);
            vertex_positions[i].y = BitConverter.ToSingle (vertices_buffer, (i * 3 + 1) * 4);
            vertex_positions[i].z = BitConverter.ToSingle (vertices_buffer, (i * 3 + 2) * 4);
        }
        mesh_updated = true;
        request_in_fly = false;
    }
    void Start()
    {
        client = new TcpClient(m_IPAdress, kPort);
        Create_Model();
        Set_Fixed_Points();
        Set_Collision_Geometry ();
        Start_Simulation();
        vertex_positions = deformable_mesh_filter.mesh.vertices;
        fixed_geometry.GetComponent<MeshRenderer>().enabled = false;
        hook_manager = hook_manager_object.GetComponent<HookManager> ();
        suture_manager = suture_manager_object.GetComponent<SutureManager> ();
        deformable_collider = deformable_object.GetComponent<MeshCollider> ();
    }
    // Update is called once per frame
    void Update()
    {
        if (mesh_updated) {
            deformable_mesh_filter.mesh.vertices = vertex_positions;
            deformable_mesh_filter.mesh.RecalculateNormals ();
            deformable_collider.sharedMesh = deformable_mesh_filter.mesh;
            mesh_updated = false;
        }
        if (request_in_fly) return;
        request_in_fly = true;
        Thread t = new Thread(Request_Frame);
        t.Start();
    }
}
