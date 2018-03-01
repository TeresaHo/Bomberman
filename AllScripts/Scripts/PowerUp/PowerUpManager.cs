using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {
	private bool powerupActive;

	private bool devilwalk;
	private bool speedup;
	private bool addBombStr;
	private bool addBombNum;
	private bool pushBomb;
	private bool animal;
	private bool changeExplosionRadius;
	public float powerupLengthCounter;
	public bool freeze;
	private AbilityController logicman;

	public GameObject[] animalsPrefab;
	// Use this for initialization
	void Start () {
		logicman = gameObject.GetComponent<AbilityController> ();
	}

	// Update is called once per frame
	void Update () {
		if (powerupActive) {
			powerupLengthCounter -= Time.deltaTime;

			if (speedup && !logicman.onAnimal) {
				logicman.AddSpeed();
				speedup = false;
				powerupActive = false;
			}
			if (devilwalk) {
				logicman.DoDevil();
				devilwalk = false;
			}
			if (addBombStr) {
				logicman.AddBombStr ();
				addBombStr = false;
			}
			if (addBombNum) {
				logicman.AddBombNum ();
				addBombNum = false;
			}
			if (changeExplosionRadius) {
				if (!logicman.diffWayOfExplosion)
				logicman.AddBomRadius ();
				changeExplosionRadius = false;
			}
			if (pushBomb) {
				logicman.doPushable ();
				pushBomb = false;
			}
			if (animal) {
				animal = false;
				logicman.onAnimal = true;
			}
			if (freeze) {
				logicman.setSpeed (0);
			}
			if (powerupLengthCounter <= 0) {
				if (logicman.isNeg()) {
					logicman.DoDevil ();
				}
				if (logicman.Pushable) {
					logicman.doPushable();
				}
				if (logicman.speed == 0) {
					logicman.speed = logicman.oriSpeed;
					logicman.GetComponent<RigidBodyFPSWalker> ().dead = "LIVE";
				}
				if (logicman.diffWayOfExplosion)
					logicman.AddBomRadius ();
				devilwalk = false;
				freeze = false;
				powerupActive = false;
			}
		}
	}
	public void ActivatePowerUp(bool point, bool speed, bool strength, bool num, bool push, bool ani, float time,bool changeExplosionRadius) {
		devilwalk  = point;
		speedup    = speed;
		addBombNum = num;
		addBombStr = strength;
		pushBomb   = push;
		animal     = ani;
		powerupActive = true;
		powerupLengthCounter = time;
		this.changeExplosionRadius = changeExplosionRadius;
	}
	[PunRPC]
	public void getAnimal(int animalIndex,float aniSpeed) {
		if (animal) {
			Debug.Log (transform.forward);
			Vector3 pos = new Vector3 (transform.position.x+transform.forward.x, 1.5f, transform.position.z+transform.forward.z);
			GameObject animals = Instantiate(animalsPrefab [animalIndex],pos,transform.rotation);
			animals.transform.parent = transform;
			Controller control = animals.GetComponent<Controller> (); 
			transform.GetComponent<CapsuleCollider> ().height = 0.052f;
			logicman.setSpeed (aniSpeed);
			animals.name = "animal";
		}



	}

	public void setFreeze(float time){
		powerupActive = true;
		freeze = true;
		powerupLengthCounter = time;
	}
}
