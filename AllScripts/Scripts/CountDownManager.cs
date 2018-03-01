using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownManager : MonoBehaviour {
	public float countDown = 180f;
	public MyNetworkManager manager;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (manager.isInGame) {
			StartCountDown ();
		}
	}
	public void StartCountDown(){
		countDown -= Time.deltaTime;
		if (countDown < 0)
			countDown = 0;
	}
}
