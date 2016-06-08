using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CurveModifier : MonoBehaviour 
{
	public enum CurveAxis { XYZ, XZY, ZYX };

	public int resolution = 32;
	public bool shouldUpdate = true;
	public bool showCurve = false;
	public bool curveLoop = false;
	public bool inverseCurve = false;
	public CurveAxis curveAxis = CurveAxis.XYZ;
	public float cycleTimeScale = 0f;

	protected Color[] colorArray;
	protected Texture2D texture;
	protected Vector3[] vectorArray;
	protected Renderer[] rendererArray;

	private Vector3 forward = Vector3.forward;
	private Vector3 up = Vector3.up;
	private Vector3 right = Vector3.right;

	public void Init ()
	{
		if (resolution > 0)
		{
			texture = new Texture2D(resolution, 1, TextureFormat.RGBAFloat, false);
			texture.wrapMode = TextureWrapMode.Clamp;
			colorArray = new Color[resolution];
			vectorArray = new Vector3[resolution];
			rendererArray = GetComponentsInChildren<Renderer>();
			CurveToTexture();
		}
	}

	public virtual Vector3 GetCurvePoint (float ratio)
	{
		if (vectorArray != null) {
			return vectorArray[(int)Mathf.Floor(vectorArray.Length * ratio)];
		} else {
			return Vector3.zero;
		}
	}

	public void CurveToTexture ()
	{
		if (texture != null) 
		{
			for (int i = 0; i < resolution; ++i) 
			{
				float ratio;
				if (inverseCurve)
				{
					ratio = (resolution - i - 1) / (float)resolution;
				}
				else
				{
					ratio = i / (float)resolution;
				}
				Vector3 p = GetCurvePoint(ratio);
				colorArray[i] = new Color(p.x, p.y, p.z, 0f);
			}

			texture.SetPixels(colorArray);
			texture.Apply();

			SetupAxis();

			if (rendererArray != null) 
			{
				foreach (Renderer renderer in rendererArray)  
				{
					foreach (Material material in renderer.sharedMaterials)  
					{
						material.SetTexture("_CurveTexture", texture);
						material.SetFloat("_CurveResolution", resolution);
						material.SetFloat("_ShouldLoop", curveLoop ? 1f : 0f);
						material.SetFloat("_TimeSpeed", cycleTimeScale);
						material.SetVector("_Forward", forward);
						material.SetVector("_Up", up);
						material.SetVector("_Right", right);
					}
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

	void OnDrawGizmos () 
	{
		if (showCurve)
		{
			Gizmos.color = Color.red;
			for(int i = 0; i < resolution - 1; i++)
			{
				Gizmos.DrawLine(GetCurvePoint(i / (float)resolution), GetCurvePoint((i + 1) / (float)resolution));
			}
		}
	}
}
