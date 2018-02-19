using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FollowCurveModifier : CurveModifier 
{
	public Transform target;
	public float deltaScale = 10f;
	public float minimumDistance = 0.1f;
	public float maximumDistance = 1f;

	void OnEnable ()
	{
		Init();
	}
	
	void OnRenderObject () 
	{
		if (shouldUpdate && target != null) 
		{
			if (vectorArray.Length == resolution) 
			{
				vectorArray[resolution - 1] = target.position;
				for (int i = 0; i < resolution - 1; ++i) 
				{
					float ratio = Mathf.Clamp(Time.deltaTime * deltaScale, 0f, 1f);
					if (Vector3.Distance(vectorArray[i], vectorArray[i + 1]) > maximumDistance) 
					{
						vectorArray[i] = vectorArray[i + 1] + maximumDistance * Vector3.Normalize(vectorArray[i] - vectorArray[i + 1]);
					}
					else if (Vector3.Distance(vectorArray[i], vectorArray[i + 1]) < minimumDistance) 
					{
						vectorArray[i] = vectorArray[i + 1] + minimumDistance * Vector3.Normalize(vectorArray[i] - vectorArray[i + 1]);
					}
					else
					{
						vectorArray[i] = Vector3.Lerp(vectorArray[i], vectorArray[i + 1], ratio);
					}
				}
			}
			else
			{
				Init();
			}

			CurveToTexture();
		}
	}
}
