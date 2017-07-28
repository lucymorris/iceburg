using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerUpper : MonoBehaviour {
    public System.Action itemAdded = delegate {};

    public Holdable heldItem;

    public Transform pickUpTarget;

    public void PickUp(Holdable item)
    {
      heldItem = item;
      item.transform.parent = null;
      item.AddHolder(this.pickUpTarget);
    }

    public void Drop()
    {
      heldItem = null;
      heldItem.RemoveHolder(this.pickUpTarget);
    }

}
