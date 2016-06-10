using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScriptCurveModifier : CurveModifier 
{
	public float radius = 10f;
	public float height = 1f;

	void OnEnable ()
	{
		Init();
		CurveToTexture();
	}

	public override Vector3 GetCurvePoint (float ratio)
	{
		float angle = ratio * Mathf.PI * 2f;
		return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle * 5) * height, Mathf.Sin(angle) * radius);
	}
	
	void OnRenderObject () 
	{
		if (shouldUpdate) 
		{
			if (vectorArray.Length != resolution) 
			{
				Init();
			}

			CurveToTexture();
		}
	}
}
