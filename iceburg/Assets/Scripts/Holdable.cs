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

	List<Renderer> renderers = new List<Renderer>();

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

			bool includeInactive = true;
			gameObject.GetComponentsInChildren<Renderer>(includeInactive, renderers);

			for (int i = 0; i < renderers.Count; ++i)
			{
				renderers[i].enabled = false;
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
			this.objective = null;
		}
	}

	public bool IsBeingHeldByObjective(Objective objective)
	{
		return this.objective == objective;
	}

	public void AddHolder(PickerUpper holder, float holdDistance)
	{
		if (!this.IsBeingHeldBy(holder))
		{
			if (this.objective != null)
			{
				this.objective.RemoveItem(this);
			}
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

				for (int i = 0; i < renderers.Count; ++i)
				{
					renderers[i].enabled = true;
				}
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

			if (spawner != null)
			{
				if (thisRigidbody != null)
				{
					thisRigidbody.isKinematic = true;
				}
			}
			else if (objective != null)
			{
				if (thisRigidbody != null)
				{
					Vector3 directionToCenter = objective.transform.position - transform.position;
					directionToCenter *= directionToCenter.magnitude;
					if (directionToCenter.magnitude > 1.0f)
					{
						directionToCenter.Normalize();
					}

					thisRigidbody.AddForce(directionToCenter, ForceMode.VelocityChange);
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
