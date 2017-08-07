using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawn : MonoBehaviour
{
	public Holdable itemPrefab;
	public float respawnSeconds = 5;

	[System.NonSerialized]
	public Holdable item;

	public void Start()
	{
		Respawn();
	}

	public void Consume()
	{
		if (item != null)
		{
			item = null;

			Invoke("Respawn", respawnSeconds);
		}
	}

	public void Respawn()
	{
		if (item == null)
		{
			item = Object.Instantiate(itemPrefab, this.transform);
			item.SetSpawner(this);
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}
