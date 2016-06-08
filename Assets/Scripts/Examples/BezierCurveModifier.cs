using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BezierCurveModifier : CurveModifier 
{
	public BezierCurve bezierCurve;

	void Start ()
	{
		if (bezierCurve != null) {
			resolution = bezierCurve.resolution;
			Init();
		}
	}

	public override Vector3 GetCurvePoint (float ratio)
	{
		if (bezierCurve != null) {
			return bezierCurve.GetPointAt(ratio);
		} else {
			return Vector3.zero;
		}
	}
	
	void Update () 
	{
		if (shouldUpdate) 
		{
			if (vectorArray != null && vectorArray.Length != resolution) 
			{
				Init();
			}

			CurveToTexture();
		}
	}
}
