using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasures : MonoBehaviour {
	public GameObject[] Items;
	public int itemIndex;
	private bool hasCreated;
	public GameObject treasure;
	public MyNetworkManager manager;
	// Use this for initialization
	void Start () {
		Debug.Log ("START");
		hasCreated = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void CreateTreasure(){
			itemIndex  = Random.Range(0, Items.Length);
			//Instantiate (Items [itemIndex], transform.position, transform.rotation);
			treasure=(GameObject)PhotonNetwork.Instantiate (Items [itemIndex].name, transform.position, transform.rotation,0);
			Destroy (gameObject);
		


	}
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Explosion")&&!hasCreated) {
			Debug.Log ("treasure");
			Instantiate (treasure, transform.position, transform.rotation);
			hasCreated=true;
			Destroy (gameObject);

		}


	}
		
}
