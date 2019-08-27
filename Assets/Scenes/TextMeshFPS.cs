using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshFPS : MonoBehaviour {

    float deltaTime;
	public int targetFPS = 60;
	public TextMesh text_mesh;
	public Color textColor = Color.white;
	public Color yellowColor = Color.yellow;
	public Color redColor = Color.red;
    public bool hideIfNotEditor = true;

    private void Awake()
    {
        if (hideIfNotEditor && Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.WindowsEditor) gameObject.SetActive(false);
    }

    void Update () {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			string text = string.Format("FPS {1:0.}", msec, fps);

			Color finalCol = textColor;
			if (fps < 40) finalCol = yellowColor;
			if (fps < 30) finalCol = redColor;

		text_mesh.text = text;
		text_mesh.color = finalCol;
	}
}
