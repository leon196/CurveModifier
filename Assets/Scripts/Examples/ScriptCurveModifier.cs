using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScriptCurveModifier : CurveModifier 
{
	public float radius = 10f;

	void Start ()
	{
		Init();
		CurveToTexture();
	}

	public override Vector3 GetCurvePoint (float ratio)
	{
		float angle = ratio * Mathf.PI * 2f;
		return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
	}
	
	void Update () 
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
