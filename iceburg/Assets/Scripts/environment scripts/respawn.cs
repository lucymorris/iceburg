using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawn : MonoBehaviour
{
    public Holdable entityPrefab;

    [System.NonSerialized]
    public bool spawned;
    [System.NonSerialized]
    public Holdable entity;

    public void Consume()
    {
        spawned = false;
    }

    public void Respawn()
    {
        if (entity == null)
        {
            spawned = true;

            entity = Object.Instantiate(entityPrefab);
        }
    }
}
