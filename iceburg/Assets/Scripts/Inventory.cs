using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Fruit
}

[System.Serializable]
public class InventoryItem
{
	public ItemType type;
	public int stackCount;
}

public class Inventory : MonoBehaviour {
    public List<InventoryItem> items = new List<InventoryItem>();

    public System.Action itemAdded = delegate {};

    public void Add(ItemType itemType)
    {
    	bool existing = false;
    	for (int i = 0; i < items.Count; ++i)
    	{
    		if (items[i].type == itemType)
    		{
    			items[i].stackCount++;
    			existing = true;
    		}
    	}
    	if (!existing)
    	{
    		InventoryItem newEntry = new InventoryItem();
    		newEntry.type = itemType;
    		newEntry.stackCount = 1;
    		items.Add(newEntry);
    	}

    	itemAdded();
    }
}
