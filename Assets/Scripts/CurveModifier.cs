using UnityEngine;
using System.Collections;

public class CurveModifier : MonoBehaviour 
{
	public enum CurveAxis { XYZ, XZY, ZYX };

	public int resolution = 32;
	public bool shouldUpdate = true;
	public bool showCurve = false;
	public bool curveLoop = false;
	public bool inverseCurve = false;
	public CurveAxis curveAxis = CurveAxis.XYZ;
	public float cycleOffset = 0f;
	public float cycleTimeScale = 0f;
	public float curveScale = 1f;
	public float planarScale = 1f;

	protected Texture2D texture;
	protected Color[] colorArray;
	protected Vector3[] vectorArray;
	protected Renderer[] rendererArray;
	protected MeshFilter[] meshFilterArray;

	private Vector3 forward = Vector3.forward;
	private Vector3 up = Vector3.up;
	private Vector3 right = Vector3.right;

	public void Init ()
	{
		texture = new Texture2D(resolution, 1, TextureFormat.RGBAFloat, false);
		colorArray = new Color[resolution];
		vectorArray = new Vector3[resolution];
		rendererArray = GetComponentsInChildren<Renderer>();
		meshFilterArray = GetComponentsInChildren<MeshFilter>();
		CurveToTexture();
	}

	public virtual Vector3 GetCurvePoint (float ratio)
	{
		if (vectorArray != null) {
			return vectorArray[(int)Mathf.Clamp(Mathf.Floor(vectorArray.Length * ratio), 0f, vectorArray.Length - 1)];
		} else {
			return Vector3.zero;
		}
	}

	public Vector3 GetCurveCenter ()
	{
		Vector3 center = Vector3.zero;
		for (int i = 0; i < resolution; ++i) {
			center += GetCurvePoint(i / (float)resolution);
		}
		return center / (float)resolution;
	}

	public Vector3 GetCurveSize ()
	{
		Vector3 center = GetCurveCenter();
		Vector3 size = Vector3.one * -Mathf.Infinity;
		for (int i = 0; i < resolution; ++i) {
			Vector3 point = GetCurvePoint(i / (float)resolution) - center;
			size.x = Mathf.Max(size.x, Mathf.Abs(point.x));
			size.y = Mathf.Max(size.y, Mathf.Abs(point.y));
			size.z = Mathf.Max(size.z, Mathf.Abs(point.z));
		}
		return size * 2f;
	}

	public void CurveToTexture ()
	{
		if (texture != null) 
		{
			int i = 0;
			Vector3 p;
			for (i = 0; i < resolution; ++i) {
				p = GetCurvePoint(i / (float)resolution);
				colorArray[i].r = p.x;
				colorArray[i].g = p.y;
				colorArray[i].b = p.z;
			}

			texture.SetPixels(colorArray);
			texture.Apply();

			SetupAxis();

			if (meshFilterArray != null)
			{
				foreach (MeshFilter meshFilter in meshFilterArray)
				{
					Bounds bounds = meshFilter.sharedMesh.bounds;
					bounds.center = Vector3.zero;
					bounds.size = GetCurveSize();
					meshFilter.sharedMesh.bounds = bounds;
				}
			}
		}

		if (rendererArray != null) 
		{
			foreach (Renderer renderer in rendererArray)  
			{
				foreach (Material material in renderer.sharedMaterials)  
				{
					material.SetTexture("_CurveTexture", texture);
					material.SetFloat("_CurveResolution", resolution);
					material.SetFloat("_ShouldLoop", curveLoop ? 1f : 0f);
					material.SetFloat("_ShouldInverse", inverseCurve ? 1f : 0f);
					material.SetFloat("_CycleOffset", cycleOffset);
					material.SetFloat("_CycleTime", 100f + Time.time * cycleTimeScale / 10f);
					material.SetFloat("_CurveScale", curveScale);
					material.SetFloat("_PlanarScale", planarScale);
					material.SetVector("_Forward", forward);
					material.SetVector("_Up", up);
					material.SetVector("_Right", right);
				}
			}
		}
	}

	void SetupAxis ()
	{
		switch (curveAxis)
		{
			case CurveAxis.XYZ :
			{
				forward = Vector3.forward;
				up = Vector3.up;
				right = Vector3.right;
				break;
			}
			case CurveAxis.XZY :
			{
				forward = Vector3.up;
				up = Vector3.forward;
				right = Vector3.right;
				break;
			}
			case CurveAxis.ZYX :
			{
				forward = Vector3.right;
				up = Vector3.up;
				right = Vector3.forward;
				break;
			}
		}
	}

	public virtual float GetCurveLength ()
	{
		float dist = 0f;
		Vector3 from, to;
		for(int i = 0; i < resolution - 1; i++)
		{
			from = GetCurvePoint(i / (float)resolution);
			to = GetCurvePoint((i + 1) / (float)resolution);
			dist += Vector3.Distance(from, to);
		}
		return dist;
	}

	void OnDrawGizmos () 
	{
		if (showCurve)
		{
			float res = resolution - 1;
			Vector3 from = Vector3.zero;
			Vector3 to = Vector3.zero;
			Gizmos.color = Color.red;
			for (int i = 0; i < resolution - 1; ++i) {
				from = GetCurvePoint(i / (float)res);
				to = GetCurvePoint(((i+1) / (float)res));

				Gizmos.DrawLine(from, to);
				Gizmos.DrawLine(from, from + Vector3.Cross(from.normalized, to.normalized).normalized);
			}

			Gizmos.color = Color.yellow;
			if (meshFilterArray != null)
			{
				foreach (MeshFilter meshFilter in meshFilterArray)
				{
					Bounds bounds = meshFilter.sharedMesh.bounds;
					Gizmos.DrawWireCube(GetCurveCenter() + bounds.center, bounds.size);
				}
			}
		}
	}
}
