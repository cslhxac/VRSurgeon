using UnityEngine;
using System.Collections;

[System.Serializable]
public class Scene {
	public string[] collisionObjects;
	public class Scene_Object{
		public string file_name;
		public int textureMap;
		public int normalMap;
	};
	public Scene_Object[] dynamicObjects;
}
public class SceneObject : MonoBehaviour {
	Scene scene = new Scene();
	// Use this for initialization
	void Start () {
		TextAsset targetFile = Resources.Load<TextAsset>("scenes/simple");

		scene = JsonUtility.FromJson<Scene>(targetFile.text);
		Debug.Log(scene.collisionObjects[0]);
		Debug.Log(scene.dynamicObjects);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
