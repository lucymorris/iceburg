using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : MonoBehaviour
{
	public enum ItemType { Fruit, Ice }

	public List<PickerUpper> holders = new List<PickerUpper>();

	public ItemType type;

	public float smoothTime;

	public int holdersRequired = 1;

	private Vector3 currentVelocity;
	private respawn spawner;
	private Objective objective;

	public static int defaultLayer;
	public static int heldLayer;

	Rigidbody thisRigidbody;

	public void Awake()
	{
		thisRigidbody = GetComponent<Rigidbody>();

		defaultLayer = LayerMask.NameToLayer("Interactive");
		heldLayer = LayerMask.NameToLayer("HeldItem");
	}

	public void SetSpawner(respawn spawner)
	{
		if (spawner != null)
		{
			this.spawner = spawner;
			if (thisRigidbody != null)
			{
				thisRigidbody.isKinematic = true;
			}
		}
		else
		{
			Debug.LogError("Tried to set spawner to null!");
		}
	}

	public void SetObjective(Objective objective)
	{
		ClearHolders();
		this.objective = objective;
	}

	public void ClearObjective(Objective objective)
	{
		if (this.objective == objective)
		{
			this.objective = objective;
		}
	}

	public void AddHolder(PickerUpper holder, float holdDistance)
	{
		if (!this.IsBeingHeldBy(holder))
		{
			holders.Add(holder);
		}
	}

	public bool IsBeingHeldBy(PickerUpper holder)
	{
		for (int i = 0; i < holders.Count; ++i)
		{
			if (holders[i] == holder)
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
			if (holders[i] == holder)
			{
				holders[i].heldItem = null;
				holders.RemoveAt(i);
			}
		}
	}

	public void ClearHolders()
	{
		for (int i = holders.Count-1; i >= 0; --i)
		{
			holders[i].heldItem = null;
		}
		holders.Clear();
	}

	public void Update()
	{
		for (int i = holders.Count-1; i >= 0; --i)
		{
			if (Vector3.Distance(holders[i].pickUpTarget.position, transform.position) > holders[i].passiveRadius)
			{
				holders[i].heldItem = null;
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

			this.gameObject.layer = heldLayer;

			Vector3 targetPosition = holders[0].pickUpTarget.position;
			for (int i = 1; i < holders.Count; ++i)
			{
				targetPosition += holders[i].pickUpTarget.position;
			}
			targetPosition = targetPosition / (float)holders.Count;
			if (thisRigidbody != null)
			{
				thisRigidbody.isKinematic = true;
				thisRigidbody.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*smoothTime));
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*smoothTime);
			}
		}
		else
		{
			this.gameObject.layer = defaultLayer;

			if (objective != null || spawner != null)
			{
				if (thisRigidbody != null)
				{
					thisRigidbody.isKinematic = true;
				}
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
}
