using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawn : MonoBehaviour
{
    public Holdable itemPrefab;

    [System.NonSerialized]
    public bool spawned;
    [System.NonSerialized]
    public Holdable item;

    public void Consume()
    {
        spawned = false;
    }

    public void Respawn()
    {
        if (item == null)
        {
            spawned = true;

            item = Object.Instantiate(itemPrefab, this.transform);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
