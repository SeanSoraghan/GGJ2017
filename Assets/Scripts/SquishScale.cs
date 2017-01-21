using UnityEngine;
using System.Collections;

public class SquishScale : MonoBehaviour
{
	public float amount = .1f;
	public float speed = 10f;
	public float timeOffset;
	private Vector3 originalScale;

	void Awake()
	{
		originalScale = transform.localScale;
	}

	void Update()
	{
		transform.localScale = new Vector3(
			originalScale.x + Mathf.Sin(Time.time * speed + timeOffset) * amount,
			originalScale.y + Mathf.Sin(Time.time * speed + timeOffset + Mathf.PI) * amount,
			1f);	
	}
}