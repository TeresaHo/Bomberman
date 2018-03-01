using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour  {
	/* 生成一個座標的陣列 */
	public Transform[] SpawnPoints;
	/* 生成時間 */
	public float spawnTime = 5f;
	/* 生成物件 */
	public GameObject[] Items;
	/* 確定有沒有物件 */
	public bool hasItem;
	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnItems", spawnTime, spawnTime);
	}

	// Update is called once per frame
	void Update () {

	}
	void SpawnItems () {
		int spawnIndex = Random.Range(0, SpawnPoints.Length);
		int itemIndex  = Random.Range(0, Items.Length);
		if (Physics.CheckSphere (SpawnPoints [spawnIndex].position, 0.5f)) {

		} else {
			PhotonNetwork.Instantiate (Items [itemIndex].name, SpawnPoints [spawnIndex].position, SpawnPoints [spawnIndex].rotation,0);
		}
	}
}
