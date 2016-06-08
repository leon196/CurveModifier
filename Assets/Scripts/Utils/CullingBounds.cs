using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CullingBounds : MonoBehaviour
{
	public float distance = 100f;

	void OnEnable ()
	{
		Renderer[] rendererArray = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in rendererArray)
		{
			MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
			if (meshFilter != null) {
				meshFilter.sharedMesh.bounds = new Bounds (transform.position, Vector3.one * distance);
			}
		}
	}
}