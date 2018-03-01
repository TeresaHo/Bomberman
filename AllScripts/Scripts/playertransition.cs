using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertransition : MonoBehaviour {
	
	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D))
			anim.SetInteger ("AnimPara", 1);
		else
			anim.SetInteger ("AnimPara", 0);
	}
	[PunRPC]
	void PlayAnim(){
		
	}
}
