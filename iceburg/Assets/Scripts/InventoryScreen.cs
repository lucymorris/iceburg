using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScreen : MonoBehaviour {
	public Transform itemsContainer;

	public InventoryScreenEntry inventoryEntryPrefab;

	Inventory playerInventory;

	void Start()
	{
		GameObject player = GameObject.FindWithTag("Player");
		playerInventory = player.GetComponent<Inventory>();

		playerInventory.itemAdded += HandleItemAdded;
	}

	void HandleItemAdded()
	{
		for (int i = itemsContainer.childCount-1; i >= 0; --i)
		{
			Object.Destroy(itemsContainer.GetChild(i).gameObject);
		}
		for (int i = 0; i < playerInventory.items.Count; ++i)
		{
			InventoryScreenEntry entry = Instantiate(inventoryEntryPrefab, itemsContainer);
			entry.SetItem(playerInventory.items[i]);
		}
	}
}
