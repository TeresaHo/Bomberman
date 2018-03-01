using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour {
	private PowerUpManager manager;
	public bool freeze;
	private float time = 7f;
	// Use this for initialization
	void Start () {
		freeze = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Player") && other.GetComponent<RigidBodyFPSWalker> ().dead.Equals ("LIVE")) {
			
			manager = other.gameObject.GetComponent<PowerUpManager> ();
			if (manager != null && freeze) {
				other.GetComponent<RigidBodyFPSWalker> ().dead = "FREEZE";
				manager.setFreeze (time);
				Invoke ("setAct", 15f);
				gameObject.SetActive (false);
			}
		}
	}
	void setAct() {
		gameObject.SetActive (true);
	}
}
