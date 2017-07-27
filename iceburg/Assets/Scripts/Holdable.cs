using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : MonoBehaviour
{
	public List<Transform> holders = new List<Transform>();

	public float smoothTime;

	private Vector3 currentVelocity;

	public void AddHolder(Transform holder)
	{
		if (!holders.Contains(holder))
		{
			holders.Add(holder);
		}
	}

	public void RemoveHolder(Transform holder)
	{
		holders.Remove(holder);
	}

	public void Update()
	{
		if (holders.Count > 0)
		{
			Vector3 targetPosition = holders[0].position;
			for (int i = 1; i < holders.Count; ++i)
			{
				targetPosition += holders[i].position;
			}

			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
		}
	}
}
