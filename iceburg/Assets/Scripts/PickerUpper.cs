using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerUpper : MonoBehaviour {
    public System.Action itemAdded = delegate {};

    public Holdable heldItem;

    public void PickUp(Holdable item)
    {
      heldItem = item;
      item.AddHolder(this.transform);
    }

    public void Drop()
    {
      heldItem = null;
      heldItem.RemoveHolder(this.transform);
    }

}
