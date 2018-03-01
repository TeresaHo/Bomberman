using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
public class RigidBodyFPSWalker : AbilityController {
	

	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool grounded = false;

	public GameObject fpsCamera;
	public GameObject standbyCamera;
	public AudioClip screamAudioClip;
	private AudioSource bgAudioSource;
	public CountDownManager countDownManager;
	//public bool backward = false;
	//public bool pickUp = false;
	public bool isBubbled = false;
	//控制哪個相機的視角
	bool cameraState=false;
	SpawnSpot[] spawnSpots;
	void Awake () {
		GetComponent<Rigidbody>().freezeRotation = true;
		GetComponent<Rigidbody>().useGravity = false;
	}
	void Start () {
		
	}
	void FixedUpdate () {
		
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = GetComponent<Rigidbody>().velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
		if (grounded) {
			// Jump
			if (canJump && Input.GetButton("Jump")) {
				GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
			}
		}

		// We apply gravity manually for more tuning control
		GetComponent<Rigidbody>().AddForce(new Vector3 (0, -gravity * GetComponent<Rigidbody>().mass, 0));

		grounded = false;

		//如果已經死亡可以開啟另一個相機
		if (dead.Equals("DEAD") && Input.GetKeyDown (KeyCode.F)) {
			if (cameraState == false) {
				transform.FindChild ("Camera").gameObject.SetActive (false);
				GameObject.Find("StandByCamera").GetComponent<Camera>().enabled = true;
				cameraState = true;
			} else {
				transform.FindChild ("Camera").gameObject.SetActive (true);
				GameObject.Find("StandByCamera").GetComponent<Camera>().enabled = false;
				cameraState = false;
			}
		}
		/*
		if (pickUp == true && Input.GetKeyDown (KeyCode.C)) {
			Vector3 pos = new Vector3 (0, 2f, 0);
			GameObject bullet= PhotonNetwork.Instantiate ("Glove", transform.position, Quaternion.identity,0);
			bullet.GetComponent<Rigidbody> ().AddForce (transform.forward * 800.0f);
			Destroy (transform.FindChild ("Glove").gameObject);
			pickUp = false;
		}*/
		ExitGames.Client.Photon.Hashtable mode= new ExitGames.Client.Photon.Hashtable ();
		mode.Add ("mode", dead);
		PhotonNetwork.player.SetCustomProperties (mode);
	}

	void OnCollisionStay () {
		grounded = true;    
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
	void OnGUI(){
		GUILayout.BeginArea (new Rect (0, 0,200,500));
		GUILayout.Box (/*new Rect (10, 10, 200, 40), */"Mode: " + dead+" Time: "+(int)GameObject.Find("CountDownManager").GetComponent<CountDownManager>().countDown);
		if(Input.GetKey(KeyCode.LeftShift)){
			GUILayout.Box("Speed: "+speed);
			GUILayout.Box("Bomb: "+numberOfBomb);
			GUILayout.Box("Bomb Strength: "+strengthOfBomb);
		}
		GUILayout.EndArea ();
	}

	[PunRPC]
	public void applyDead(){
			//把放炸彈跟人物顯示關掉
		if (isBubbled == false && !onAnimal) {
			isBubbled = true;
			//gameObject.GetComponent<WebBomb> ().enabled = false;
			transform.FindChild ("BombBubble").gameObject.SetActive (true);
			//3秒後重生
			Invoke ("Die", 20f);

			
		}
		if (onAnimal) {
			GameObject animal =transform.FindChild ("animal").gameObject;
			Destroy (animal.gameObject);
			setSpeed (5);
			Invoke ("changeAnimalState", 3f);
		}

			

		
	}

	public void changeAnimalState(){
		onAnimal = false;
	}
	[PunRPC]
	public void DestroyAnimal(){
		GameObject animal =transform.FindChild ("animal").gameObject;
		Destroy (animal.gameObject);
	}

	[PunRPC]
	public void RestartBomb(){
		//把放炸彈跟人物顯示關掉

		gameObject.GetComponent<RigidBodyFPSWalker>().isBubbled = false;
		//gameObject.GetComponent<WebBomb> ().enabled = true;
		transform.FindChild ("BombBubble").gameObject.SetActive (false);

	}
	public void Die(){
		if(isBubbled == true){
			bgAudioSource = gameObject.GetComponent<AudioSource> ();
			bgAudioSource.PlayOneShot(screamAudioClip);
			gameObject.GetComponent<WebBomb> ().enabled = false;
			transform.FindChild ("BombBubble").gameObject.SetActive (false);
			dead= "DEAD";
			Invoke ("ReSpawn",3f);

		}

	}
	public void ReSpawn(){
		//重生
		//CANNOT SEEN BY OTHERS AND CANNOT PUT THE BOMB;

		foreach (Transform child in transform) {
			/*if (child.gameObject.GetComponent<MeshRenderer> () != null)
				child.gameObject.GetComponent<MeshRenderer> ().enabled = false;
			if (child.gameObject.GetComponent<SkinnedMeshRenderer> () != null)
				child.gameObject.GetComponent<SkinnedMeshRenderer> ().enabled = false;*/
			if(child.name!="Camera")
			child.gameObject.SetActive (false);
		}
		gameObject.GetComponent<PowerUpManager> ().enabled = false;
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		SpawnSpot newSpawn = spawnSpots [Random.Range (0, spawnSpots.Length)];
		transform.position = newSpawn.transform.position;
		transform.rotation = newSpawn.transform.rotation;
	}

}
