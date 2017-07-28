using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnConsumable : MonoBehaviour {

    public respawn spawnPointPrefab;

    public Holdable itemPrefab;

    public List<respawn> spawnPoints = new List<respawn>();

	void spawn()
    {
        for(int i = 0; i < spawnPoints.Count; i++)
        {
            respawn instance = spawnPoints[i];
            instance.itemPrefab = itemPrefab;
            instance.Respawn();
        }
    }

	void Start () {
        spawn();
	}
	
	void Update () {
	}

    void OnTriggerStay(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player)
        {
            if (Input.GetButtonDown(player.inputConfig.interact))
            {
                var pickerUpper = other.GetComponent<PickerUpper>();
                if (pickerUpper != null)
                {
                    if (pickerUpper.heldItem == null)
                    {
                        for (int index = 0; index < spawnPoints.Count; ++index)
                        {
                            respawn respawnInstance = spawnPoints[index];
                            if (respawnInstance.item != null)
                            {
                                pickerUpper.PickUp(respawnInstance.item);

                                respawnInstance.Consume();
                                respawnInstance.Invoke("Respawn", 5);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
