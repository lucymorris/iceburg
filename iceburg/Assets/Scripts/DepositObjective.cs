using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositObjective : Objective
{
	public int requiredAmount;
	public Holdable.ItemType requiredType;

	public bool destroyItems;

	NPC owner;
	List<Holdable> heldItems = new List<Holdable>();

	Collider triggerCollider;

	public void Awake()
	{
		triggerCollider = GetComponent<Collider>();
		triggerCollider.enabled = false;
	}

	public override void SetOwner(NPC owner)
	{
		this.owner = owner;
	}

	public override string FormatDialog(string format)
	{
		return string.Format(format, requiredAmount, requiredType);
	}

	public override void Activate()
	{
		triggerCollider.enabled = true;
	}

	public override bool IsComplete()
	{
		if (heldItems.Count == requiredAmount)
		{
			return true;
		}
		return false;
	}

	public override void PostCompletion()
	{
		if (destroyItems)
		{
			for (int i = heldItems.Count-1; i >= 0; --i)
			{
				Object.Destroy(heldItems[i].gameObject);
			}
		}
		heldItems.Clear();
		triggerCollider.enabled = false;
	}

	public void CheckCompletion()
	{
		if (IsComplete())
		{
			owner.CompleteObjective(this);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Holdable item = other.GetComponent<Holdable>();
		if (item != null)
		{
			if (item.type == requiredType)
			{
				AddItem(item);
			}
		}

		CheckCompletion();
	}

	public void OnTriggerExit(Collider other)
	{
		Holdable item = other.GetComponent<Holdable>();
		if (item != null)
		{
			if (item.type == requiredType)
			{
				RemoveItem(item);
			}
		}
	}

	private void AddItem(Holdable item)
	{
		heldItems.Add(item);
		item.SetObjective(this);
	}

	private void RemoveItem(Holdable item)
	{
		if (heldItems.Remove(item))
		{
			item.ClearObjective(this);
		}
	}
}
