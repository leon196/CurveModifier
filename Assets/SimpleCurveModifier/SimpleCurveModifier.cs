using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCurveModifier : MonoBehaviour {

	public BezierCurve bezierCurve;
	public Material curveMaterial;
	private Texture2D texture;

	void Start () {
		GeneratePositionTexture();

		// distribute allover the curve
		// transform.localScale = Vector3.one / GetComponent<Renderer>().bounds.size.y;
	}
	
	void Update () {
		curveMaterial.SetTexture("_CurveTexture", texture);
	}

	void GeneratePositionTexture () {
		texture = new Texture2D(32, 1, TextureFormat.RGBAFloat, false);
		Color[] pixels = new Color[texture.width];
		for (int i = 0; i < texture.width; ++i) {
			Vector3 position = bezierCurve.GetPointAt((float)i/(float)texture.width);
			pixels[i].r = position.x;
			pixels[i].g = position.y;
			pixels[i].b = position.z;
		}
		texture.SetPixels(pixels);
		texture.Apply();
	}
}
