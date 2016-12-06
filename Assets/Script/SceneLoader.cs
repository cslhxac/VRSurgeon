/* This version of ObjImporter first reads through the entire file, getting a count of how large
 * the final arrays will be, and then uses standard arrays for everything (as opposed to ArrayLists
 * or any other fancy things). 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ObjImporter
{
    public struct meshStruct
    {
        public Vector3[] vertices;
        public Vector3[] uvw;
        public int[] triangles;
        public string fileName;
    }
    public meshStruct newMesh;
    // Use this for initialization
    public Mesh ImportFile(string filePath)
    {
        newMesh = createMeshStruct(filePath);
        populateMeshStruct(ref newMesh);
        Mesh mesh = new Mesh();

        //mesh.vertices = newVerts;   
        mesh.vertices = newMesh.vertices;
        //mesh.uv = newUVs;        
        //mesh.normals = newNormals;
        mesh.triangles = newMesh.triangles;
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    private static meshStruct createMeshStruct(string filename)
    {
        int triangles = 0;
        int vertices = 0;
        int vt = 0;
        meshStruct mesh = new meshStruct();
        mesh.fileName = filename;
        StreamReader stream = File.OpenText(filename);
        string entireText = stream.ReadToEnd();
        stream.Close();
        using (StringReader reader = new StringReader(entireText))
        {
            string currentText = reader.ReadLine();
            char[] splitIdentifier = { ' ' };
            string[] brokenString;
            while (currentText != null)
            {
                if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt "))
                {
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");
                    }
                }
                else
                {
                    currentText = currentText.Trim();                           //Trim the current line
                    brokenString = currentText.Split(splitIdentifier, 50);      //Split the line into an array, separating the original line by blank spaces
                    switch (brokenString[0])
                    {
                        case "v":
                            vertices++;
                            break;
                        case "vt":
                            vt++;
                            break;
                        case "f":
                            triangles = triangles + 3;
                            break;
                    }
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");
                    }
                }
            }
        }
        mesh.triangles = new int[triangles];
        mesh.vertices = new Vector3[vertices];
        mesh.uvw = new Vector3[vt];
        return mesh;
    }

    private static void populateMeshStruct(ref meshStruct mesh)
    {
        StreamReader stream = File.OpenText(mesh.fileName);
        string entireText = stream.ReadToEnd();
        stream.Close();
        using (StringReader reader = new StringReader(entireText))
        {
            string currentText = reader.ReadLine();

            char[] splitIdentifier = { ' ' };
            char[] splitIdentifier2 = { '/' };
            string[] brokenString;
            string[] brokenBrokenString;
            int f = 0;
            int v = 0;
            int vt = 0;
            while (currentText != null)
            {
                if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt "))
                {
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");
                    }
                }
                else
                {
                    currentText = currentText.Trim();
                    brokenString = currentText.Split(splitIdentifier, 50);
                    switch (brokenString[0])
                    {
                        case "v":
                            mesh.vertices[v] = new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
                                System.Convert.ToSingle(brokenString[3]));
                            v++;
                            break;
                    case "vt":
                        if (brokenString.Length == 4) {
                            mesh.uvw [vt] = new Vector3 (System.Convert.ToSingle (brokenString [1]),
                                System.Convert.ToSingle (brokenString [2]),
                                System.Convert.ToSingle (brokenString [3]));
                        } else {
                            mesh.uvw [vt] = new Vector3 (System.Convert.ToSingle (brokenString [1]),
                                System.Convert.ToSingle (brokenString [2]),0);
                        }
                            vt++;
                            break;
                        case "f":
                            int j = 1;
                            List<int> intArray = new List<int>();
                            while (j < brokenString.Length && ("" + brokenString[j]).Length > 0)
                            {
                                Vector3 temp = new Vector3();
                                brokenBrokenString = brokenString[j].Split(splitIdentifier2, 3);    //Separate the face into individual components (vert, uv, normal)
                                temp.x = System.Convert.ToInt32(brokenBrokenString[0]);
                                intArray.Add(System.Convert.ToInt32(brokenBrokenString[0]));
                                if (brokenBrokenString.Length > 1)                                  //Some .obj files skip UV and normal
                                {
                                    if (brokenBrokenString[1] != "")                                    //Some .obj files skip the uv and not the normal
                                    {
                                        temp.y = System.Convert.ToInt32(brokenBrokenString[1]);
                                    }
                                    if (brokenBrokenString.Length > 2)
                                        temp.z = System.Convert.ToInt32(brokenBrokenString[2]);
                                }
                                j++;
                            }
                            j = 1;
                            while (j + 2 < brokenString.Length)     //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
                            {
                                mesh.triangles[f] = intArray[0] - 1;
                                f++;
                                mesh.triangles[f] = intArray[j] - 1;
                                f++;
                                mesh.triangles[f] = intArray[j + 1] - 1;
                                f++;
                                j++;
                            }
                            break;
                    }
                    currentText = reader.ReadLine();
                    if (currentText != null)
                    {
                        currentText = currentText.Replace("  ", " ");       //Some .obj files insert double spaces, this removes them.
                    }
                }
            }
        }
    }
}


public class SceneLoader : MonoBehaviour
{
    // Use this for initialization

    public string dynamic_model_file;
    public Material material;
    Mesh mesh;
    Mesh holderMesh;
    MeshFilter filter;
    ObjImporter importer;
    void Start()
    {
        Debug.Log (gameObject.name);
        importer = new ObjImporter();
        holderMesh = new Mesh();
        holderMesh = importer.ImportFile(dynamic_model_file);

        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        renderer.material = material;
        filter.mesh = holderMesh;
        if (GetComponent<MeshCollider> () != null)
            GetComponent<MeshCollider> ().sharedMesh = GetComponent<MeshFilter> ().mesh;
        //Debug.Log("filter.mesh.vertices :" + filter.mesh.vertices.Length);
        mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
