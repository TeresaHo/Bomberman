using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {
	/* 倒著走 */
	public bool devilwalk;
	/* 加速 */
	public bool speedup;
	/* 增加炸彈威力 */
	public bool addBombStr;
	/* 增加可以放的炸彈數 */
	public bool addBombNum;
	/* 可以推炸彈 */
	public bool pushBomb;
	/*炸彈範圍變圓*/
	public bool changeExplosionRadius;
	/* 吃到動物 */
	public bool animal;
	public float speed;
	/* 道具可以持續的時間 */
	public float powerupLength;
	/* 定義道具旋轉速度 */
	public float RotateSpeed = 50f;
	/* 定義道具消失的時間 */
	public float DestroyTime = 5f;
	/* 道具控制管理員 */
	private PowerUpManager manager;
	// Use this for initialization
	public int animalIndex;

	void Start () {
		//Invoke ("triggerObject", 1f);
	}

	// Update is called once per frame
	void Update () {
		/* 讓道具旋轉 */
		transform.Rotate(Vector3.up * Time.deltaTime * RotateSpeed,Space.World);
	}
	void OnTriggerEnter(Collider other) {
		/* 人撿到道具 */
		if (other.CompareTag("Player")&& other.GetComponent<RigidBodyFPSWalker>().dead.Equals("LIVE")) {
			/* manager 控制道具功能 */
			//manager = other.gameObject.GetComponent<PowerUpManager> ();
			/* 確認 manager 有得到值 */
				
				/* 傳進去所有可能的道具參數 */
				Debug.Log (transform.parent);

				other.gameObject.GetComponent<PowerUpManager> ().ActivatePowerUp (devilwalk, speedup, addBombStr, addBombNum, pushBomb, animal, powerupLength,changeExplosionRadius);
				if (animal&&!(other.gameObject.GetComponent<RigidBodyFPSWalker>().onAnimal)&&transform.parent==null) {
					RotateSpeed = 0f;
					gameObject.GetComponent<Controller> ().walk = true;
					other.gameObject.GetComponent<PhotonView>().RPC("getAnimal", PhotonTargets.AllBuffered,animalIndex,speed);
					Debug.Log ("1111");
				} 
					/* 把道具吃掉 */
					Destroy (gameObject);

			
		} else if (other.CompareTag("Explosion")) {
			/* 道具被炸彈炸到就不見了 */
			//Destroy(gameObject);
		}
		//gameObject.SetActive (false);
	}

	void triggerObject() {
		gameObject.GetComponent<SphereCollider> ().isTrigger = true;
	}
}