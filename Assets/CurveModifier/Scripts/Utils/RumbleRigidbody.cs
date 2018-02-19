using UnityEngine;
using System.Collections;

public class RumbleRigidbody : MonoBehaviour 
{
	public float strength = 2f;
	private Rigidbody[] rigidbodyArray;
	private float lastTime = 0f;
	private float delay = 1f;

	void Start () 
	{
		rigidbodyArray = GameObject.FindObjectsOfType<Rigidbody>();
	}

	Vector3 RandomDirection (float min, float max)
	{
		return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
	}
	
	void Update () 
	{
		if (lastTime + delay < Time.time) {
			lastTime = Time.time;
			foreach (Rigidbody rigidbody in rigidbodyArray) {
				rigidbody.AddForce(RandomDirection(-strength, strength), ForceMode.Impulse);
				rigidbody.AddTorque(RandomDirection(-strength, strength), ForceMode.Impulse);
			}
		}
	}
}
