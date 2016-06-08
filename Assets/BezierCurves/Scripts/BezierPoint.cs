#region UsingStatements

using UnityEngine;
using System;
using System.Collections;

#endregion

/// <summary>
/// 	- Helper class for storing and manipulating Bezier Point data
/// 	- Ensures that handles are in correct relation to one another
/// 	- Handles adding/removing self from curve point lists
/// 	- Calls SetDirty() on curve when edited 
/// </summary>
[Serializable]
public class BezierPoint : MonoBehaviour{
	
	#region PublicEnumerations
	
	/// <summary>
	/// 	- Enumeration describing the relationship between a point's handles
	/// 	- Connected : The point's handles are mirrored across the point
	/// 	- Broken : Each handle moves independently of the other
	/// 	- None : This point has no handles (both handles are located ON the point)
	/// </summary>
	public enum HandleStyle
	{
		Connected,
		Broken,
		None,
	}
	
	#endregion
	
	#region PublicProperties
	
	/// <summary>
	///		- Curve this point belongs to
	/// 	- Changing this value will automatically remove this point from the current curve and add it to the new one
	/// </summary>
	[SerializeField]
	private BezierCurve _curve;
	public BezierCurve curve
	{
		get{return _curve;}
		set
		{
			if(_curve) _curve.RemovePoint(this);
			_curve = value;
			_curve.AddPoint(this);
		}
	}

	/// <summary>
	/// 	- Value describing the relationship between this point's handles
	/// </summary>
	public HandleStyle handleStyle;
	
	/// <summary>
	/// 	- Shortcut to transform.position
	/// </summary>
	/// <value>
	/// 	- The point's world position
	/// </value>
	public Vector3 position
	{
		get { return transform.position; }
		set { transform.position = value; }
	}
	
	/// <summary>
	/// 	- Shortcut to transform.localPosition
	/// </summary>
	/// <value>
	/// 	- The point's local position.
	/// </value>
	public Vector3 localPosition
	{
		get { return transform.localPosition; }
		set { transform.localPosition = value; }
	}
	
	/// <summary>
	/// 	- Local position of the first handle
	/// 	- Setting this value will cause the curve to become dirty
	/// 	- This handle effects the curve generated from this point and the point proceeding it in curve.points
	/// </summary>
	[SerializeField] 
	private Vector3 _handle1;
	public Vector3 handle1
	{
		get { return _handle1; }
		set 
		{ 
			if(_handle1 == value) return;
			_handle1 = value;
			if(handleStyle == HandleStyle.None) handleStyle = HandleStyle.Broken;
			else if(handleStyle == HandleStyle.Connected) _handle2 = -value;
			_curve.SetDirty();
		}
	}

	/// <summary>
	///		- Global position of the first handle
	///		- Ultimately stored in the 'handle1' variable
	/// 	- Setting this value will cause the curve to become dirty
	/// 	- This handle effects the curve generated from this point and the point proceeding it in curve.points
	/// </summary>
	public Vector3 globalHandle1
	{
		get{return 	transform.TransformPoint(handle1);}
		set{handle1 = transform.InverseTransformPoint(value);}
	}
	
	/// <summary>
	/// 	- Local position of the second handle
	///  	- Setting this value will cause the curve to become dirty
	///		- This handle effects the curve generated from this point and the point coming after it in curve.points
	/// </summary>
	[SerializeField] 
	private Vector3 _handle2;
	public Vector3 handle2
	{
		get { return _handle2; }
		set 
		{ 
			if(_handle2 == value) return;
			_handle2 = value;
			if(handleStyle == HandleStyle.None) handleStyle = HandleStyle.Broken;
			else if(handleStyle == HandleStyle.Connected) _handle1 = -value;
			_curve.SetDirty();
		}		
	}
	
	/// <summary>
	///		- Global position of the second handle
	///		- Ultimately stored in the 'handle2' variable
	///		- Setting this value will cause the curve to become dirty
	///		- This handle effects the curve generated from this point and the point coming after it in curve.points 
	/// </summary>
	public Vector3 globalHandle2
	{
		get{return 	transform.TransformPoint(handle2);}
		set{handle2 = transform.InverseTransformPoint(value);}
	}
	
	#endregion
	
	#region PrivateVariables
	
	/// <summary>
	/// 	- Used to determine if this point has moved since the last frame
	/// </summary>
	private Vector3 lastPosition;
	
	#endregion
	
	#region MonoBehaviourFunctions
	
	void Update()
	{
		if(!_curve.dirty && transform.position != lastPosition)
		{
			_curve.SetDirty();
			lastPosition = transform.position;
		}
	}
	
	#endregion
}
