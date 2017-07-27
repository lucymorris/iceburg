using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnConsumable : MonoBehaviour {

    public respawn fruit;

    public ItemType itemType;

    int spawnNum = 4;

    List<respawn> respawnScripts = new List<respawn>();

	void spawn()
    {
        for(int i = 0; i < spawnNum; i++)
        {
            Vector3 fruitPos = pickSpawnPosition();
            respawn instance = Instantiate(fruit, fruitPos, Quaternion.identity);
            instance.Respawn();
            respawnScripts.Add(instance);
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
                        for (int index = 0; index < respawnScripts.Count; ++index)
                        {
                            respawn respawnInstance = respawnScripts[index];
                            if (respawnInstance.entity != null)
                            {
                                pickerUpper.heldItem = respawnInstance.entity;
                                respawnInstance.entity = null;

                                respawnInstance.Consume();
                                respawnInstance.transform.position = pickSpawnPosition();
                                respawnInstance.Invoke("Respawn", 5);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    Vector3 pickSpawnPosition()
    {
        Vector3 fruitPos = new Vector3(this.transform.position.x + Random.Range(-0.1f, 0.15f),
                                        this.transform.position.y + Random.Range(0.0f, 0.05f),
                                        this.transform.position.z + Random.Range(-0.1f, 0.15f));
        return fruitPos;
    }

}
