using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : MonoBehaviour
{
	[System.Serializable]
	public struct Holder
	{
		public PickerUpper transform;
		public float holdDistance;

		public Holder(PickerUpper transform, float holdDistance)
		{
			this.transform = transform;
			this.holdDistance = holdDistance;
		}
	}
	public List<Holder> holders = new List<Holder>();

	public float smoothTime;

	public int holdersRequired = 1;

	private Vector3 currentVelocity;
	private respawn spawner;

	Rigidbody thisRigidbody;

	public void Awake()
	{
		thisRigidbody = GetComponent<Rigidbody>();
	}

	public void SetSpawner(respawn spawner)
	{
		this.spawner = spawner;
	}

	public void AddHolder(PickerUpper holder, float holdDistance)
	{
		if (!this.IsBeingHeldBy(holder))
		{
			holders.Add(new Holder(holder, holdDistance));
		}
	}

	public bool IsBeingHeldBy(PickerUpper holder)
	{
		for (int i = 0; i < holders.Count; ++i)
		{
			if (holders[i].transform == holder)
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveHolder(PickerUpper holder)
	{
		for (int i = holders.Count-1; i >= 0; --i)
		{
			if (holders[i].transform == holder)
			{
				holders[i].transform.heldItem = null;
				holders.RemoveAt(i);
			}
		}
	}

	public void Update()
	{
		for (int i = holders.Count-1; i >= 0; --i)
		{
			if (Vector3.Distance(holders[i].transform.pickUpTarget.position, transform.position) > holders[i].holdDistance)
			{
				holders[i].transform.heldItem = null;
				holders.RemoveAt(i);
			}
		}

		// if (spawner != null)
		// {
		// 	if (thisRigidbody != null)
		// 	{
		// 		thisRigidbody.isKinematic = true;
		// 	}
		// }
		// else
		if (holders.Count >= holdersRequired)
		{
			if (spawner != null)
			{
				spawner.Consume();
				spawner = null;
			}
			if (thisRigidbody != null)
			{
				thisRigidbody.isKinematic = true;
			}
			Vector3 targetPosition = holders[0].transform.pickUpTarget.position;
			for (int i = 1; i < holders.Count; ++i)
			{
				targetPosition += holders[i].transform.pickUpTarget.position;
			}
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*smoothTime);
		}
		else
		{
			if (thisRigidbody != null)
			{
				thisRigidbody.isKinematic = false;
			}
		}
	}
}
