using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerUpper : MonoBehaviour
{
    public System.Action itemAdded = delegate {};

    public Transform pickUpTarget;

    public Transform passiveRadiusIndicator;
    public Transform activeRaidusIndicator;
    
    public float expandTime = 0.5f;

    [Header("Optional")]
    public Holdable heldItem;

    [System.NonSerialized]
    public float pickUpDistance;

    [System.NonSerialized]
    public float activeRadius;

    [System.NonSerialized]
    public float passiveRadius;


    bool active;
    float becomeActiveTime;

    public void PickUp(Holdable item)
    {
      heldItem = item;
      item.transform.parent = null;
      item.AddHolder(this, pickUpDistance);
    }

    public void Drop()
    {
      if (heldItem != null)
      {
        heldItem.RemoveHolder(this);
        heldItem = null;
      }
    }

    public void Activate()
    {
      active = true;
    }

    public void Update()
    {
      if (active)
      {
        if (becomeActiveTime == 0)
        {
          // Set initial progress
          becomeActiveTime = (activeRadius / pickUpDistance) * expandTime;
        }

        becomeActiveTime += Time.deltaTime;

        float m = Mathf.Clamp01(becomeActiveTime / expandTime);

        activeRadius = Mathf.SmoothStep(0, pickUpDistance, m);
        passiveRadius = activeRadius;

        // activeRaidusIndicator.gameObject.SetActive(true);
      }
      else
      {
        becomeActiveTime -= Time.deltaTime;
        if (becomeActiveTime < 0)
        {
          becomeActiveTime = 0;
        }

        float m = Mathf.Clamp01(becomeActiveTime / expandTime);

        activeRadius = Mathf.SmoothStep(0, pickUpDistance, m);
      }
      if (active || heldItem != null)
      {
        active = false;

        // passiveRadiusIndicator.gameObject.SetActive(true);
      }
      else
      {
        becomeActiveTime = 0;
        passiveRadius = Mathf.Lerp(passiveRadius, 0, Time.deltaTime);
        //passiveRadiusIndicator.gameObject.SetActive(false);
      }

      float activeScaleLinear = activeRadius * 2.0f;
      float passiveScaleLinear = passiveRadius * 2.0f;
      Vector3 activeScale = new Vector3(activeScaleLinear, activeScaleLinear, 1);
      passiveRadiusIndicator.localScale = activeScale;
      Vector3 passiveScale = new Vector3(passiveScaleLinear, passiveScaleLinear, 1);
      activeRaidusIndicator.localScale = passiveScale;
    }
}
