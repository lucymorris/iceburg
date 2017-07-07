using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnConsumable : MonoBehaviour {

    public GameObject fruit;
    int spawnNum = 4;

	void spawn()
    {
        for(int i = 0; i < spawnNum; i++)
        {
            Vector3 fruitPos = new Vector3(this.transform.position.x + Random.Range(-0.1f, 0.15f),
                                            this.transform.position.y + Random.Range(0.0f, 0.05f),
                                            this.transform.position.z + Random.Range(-0.1f, 0.15f));
            Instantiate(fruit,fruitPos, Quaternion.identity);
        }
    }

	void Start () {
        spawn();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
