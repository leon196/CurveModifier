using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[Serializable]
public class BezierCurve : MonoBehaviour 
{
	public int resolution = 32;
	public Material[] materials = new Material[0];
	public bool dirty { get; private set; }
	public Color drawColor = Color.white;
	[SerializeField] private bool _close;
	public bool close {
		get { return _close; }
		set {
			if(_close == value) return;
			_close = value;
			dirty = true;
		}
	}
	public BezierPoint this[int index] {	get { return points[index]; } }
	public int pointCount { get { return points.Length; } }
	private float _length;
	public float length {
		get { if(dirty) {
				_length = 0;
				for(int i = 0; i < points.Length - 1; i++){
					_length += ApproximateLength(points[i], points[i + 1], resolution);
				}
				if(close) _length += ApproximateLength(points[points.Length - 1], points[0], resolution);
				dirty = false;
			}
			return _length;
		}
	}
	[SerializeField] private BezierPoint[] points = new BezierPoint[0];
	
	void OnDrawGizmos () {
		Gizmos.color = drawColor;
		
		if(points.Length > 1){
			for(int i = 0; i < points.Length - 1; i++){
				DrawCurve(points[i], points[i+1], resolution);
			}
			
			if (close) DrawCurve(points[points.Length - 1], points[0], resolution);
		}
	}
	
	void Awake () {
		dirty = true;
	}
	
	public void AddPoint(BezierPoint point)
	{
		List<BezierPoint> tempArray = new List<BezierPoint>(points);
		tempArray.Add(point);
		points = tempArray.ToArray();
		dirty = true;
	}

	public BezierPoint AddPointAt(Vector3 position)
	{
		GameObject pointObject = new GameObject("Point "+pointCount);

		pointObject.transform.parent = transform;
		pointObject.transform.position = position;
		
		BezierPoint newPoint = pointObject.AddComponent<BezierPoint>();
		newPoint.curve = this;
		
		return newPoint;
	}

	public void RemovePoint(BezierPoint point)
	{
		List<BezierPoint> tempArray = new List<BezierPoint>(points);
		tempArray.Remove(point);
		points = tempArray.ToArray();
		dirty = false;
	}
	
	public BezierPoint[] GetAnchorPoints()
	{
		return (BezierPoint[])points.Clone();
	}

	public Vector3 GetPointAt(float t)
	{
		if(t <= 0) return points[0].position;
		else if (t >= 1) return points[points.Length - 1].position;
		
		float totalPercent = 0;
		float curvePercent = 0;
		
		BezierPoint p1 = null;
		BezierPoint p2 = null;
		
		for(int i = 0; i < points.Length - 1; i++)
		{
			curvePercent = ApproximateLength(points[i], points[i + 1], 10) / length;
			if(totalPercent + curvePercent > t)
			{
				p1 = points[i];
				p2 = points[i + 1];
				break;
			}
			
			else totalPercent += curvePercent;
		}
		
		if(close && p1 == null)
		{
			p1 = points[points.Length - 1];
			p2 = points[0];
		}
		
		t -= totalPercent;
		
		return GetPoint(p1, p2, t / curvePercent);
	}

	public int GetPointIndex(BezierPoint point)
	{
		int result = -1;
		for(int i = 0; i < points.Length; i++)
		{
			if(points[i] == point)
			{
				result = i;
				break;
			}
		}
		
		return result;
	}

	public void SetDirty()
	{
		dirty = true;
	}

	public static void DrawCurve(BezierPoint p1, BezierPoint p2, int resolution)
	{
		int limit = resolution+1;
		float _res = resolution;
		Vector3 lastPoint = p1.position;
		Vector3 currentPoint = Vector3.zero;
		
		for(int i = 1; i < limit; i++){
			currentPoint = GetPoint(p1, p2, i/_res);
			Gizmos.DrawLine(lastPoint, currentPoint);
			lastPoint = currentPoint;
		}		
	}	

	public static Vector3 GetPoint(BezierPoint p1, BezierPoint p2, float t)
	{
		if(p1.handle2 != Vector3.zero)
		{
			if(p2.handle1 != Vector3.zero) return GetCubicCurvePoint(p1.position, p1.globalHandle2, p2.globalHandle1, p2.position, t);
			else return GetQuadraticCurvePoint(p1.position, p1.globalHandle2, p2.position, t);
		}
		
		else
		{
			if(p2.handle1 != Vector3.zero) return GetQuadraticCurvePoint(p1.position, p2.globalHandle1, p2.position, t);
			else return GetLinearPoint(p1.position, p2.position, t);
		}	
	}

    public static Vector3 GetCubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        t = Mathf.Clamp01(t);

        Vector3 part1 = Mathf.Pow(1 - t, 3) * p1;
        Vector3 part2 = 3 * Mathf.Pow(1 - t, 2) * t * p2;
        Vector3 part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p3;
        Vector3 part4 = Mathf.Pow(t, 3) * p4;

        return part1 + part2 + part3 + part4;
    }

    public static Vector3 GetQuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);

        Vector3 part1 = Mathf.Pow(1 - t, 2) * p1;
        Vector3 part2 = 2 * (1 - t) * t * p2;
        Vector3 part3 = Mathf.Pow(t, 2) * p3;

        return part1 + part2 + part3;
    }

    public static Vector3 GetLinearPoint(Vector3 p1, Vector3 p2, float t)
    {
        return p1 + ((p2 - p1) * t);
    }

	public static Vector3 GetPoint(float t, params Vector3[] points){
		t = Mathf.Clamp01(t);
		
		int order = points.Length-1;
		Vector3 point = Vector3.zero;
		Vector3 vectorToAdd;
		
		for(int i = 0; i < points.Length; i++){
			vectorToAdd = points[points.Length-i-1] * (BinomialCoefficient(i, order) * Mathf.Pow(t, order-i) * Mathf.Pow((1-t), i));
			point += vectorToAdd;
		}
		
		return point;
	}

	public static float ApproximateLength(BezierPoint p1, BezierPoint p2, int resolution = 10)
	{
		float _res = resolution;
		float total = 0;
		Vector3 lastPosition = p1.position;
		Vector3 currentPosition;
		
		for(int i = 0; i < resolution + 1; i++)
		{
			currentPosition = GetPoint(p1, p2, i / _res);
			total += (currentPosition - lastPosition).magnitude;
			lastPosition = currentPosition;
		}
		
		return total;
	}

	private static int BinomialCoefficient(int i, int n){
		return 	Factoral(n)/(Factoral(i)*Factoral(n-i));
	}
	
	private static int Factoral(int i){
		if(i == 0) return 1;
		
		int total = 1;
		
		while(i-1 >= 0){
			total *= i;
			i--;
		}
		
		return total;
	}

}