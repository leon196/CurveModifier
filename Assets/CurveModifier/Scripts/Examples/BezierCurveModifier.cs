using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BezierCurveModifier : CurveModifier 
{
	public BezierCurve bezierCurve;

	void OnEnable ()
	{
		if (bezierCurve != null) {
			resolution = bezierCurve.resolution;
			Init();
			CurveToTexture();
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

	public override float GetCurveLength ()
	{
		if (bezierCurve != null) {
			return bezierCurve.length;
		} else {
			return base.GetCurveLength();
		}
	}

	void OnRenderObject () 
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
