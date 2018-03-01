using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	public Animator anim;
	public bool walk;
	public float inputX;
	public float inputY;
	public float inputZ;
	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		setwalk ();
		if (walk && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) {
			anim.SetBool ("AniPara", true);
		}
	}

	void setwalk() {
		anim.SetBool ("AniPara", false);
	}
}
