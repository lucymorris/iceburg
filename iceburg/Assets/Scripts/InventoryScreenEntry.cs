using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScreenEntry : MonoBehaviour {
    public Text nameText;

    public void SetItem(InventoryItem item)
    {
        string nameString;
        if (item.stackCount == 1)
        {
            nameString = string.Format("{0}", item.type);
        }
        else
        {
            nameString = string.Format("{0} x{1}", item.type, item.stackCount);
        }
        nameText.text = nameString;
    }
}
