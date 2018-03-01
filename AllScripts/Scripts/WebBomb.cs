using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebBomb : MonoBehaviour {

	private AbilityController logicman;

	void Start()
	{
		/* 拿到 player object */
		logicman = gameObject.GetComponent<AbilityController> ();
	}
		
	void Update () {
		/* 檢查可不可以放炸彈 */
		if (Input.GetMouseButtonDown (0) /*&& transform.GetComponent<RigidBodyFPSWalker>().grounded*/ && logicman.isAbaleBomb() && gameObject.GetComponent<RigidBodyFPSWalker>().isBubbled==false) {
			/* 可放炸彈數減少 */
			logicman.useBomb ();
			/* 放炸彈 */
			DropBumb ();
			/* 可以放的炸彈數在炸彈引爆後恢復 */
			Invoke("plusBombNum", 3f);
		}
	}
	public void DropBumb()
	{
		Vector3 newPos = new Vector3 ( transform.position.x,  1.8f,  transform.position.z);
		/* 複製炸彈 (包括底下所有的 component) */
		GameObject bomb = PhotonNetwork.Instantiate("Bomb", newPos, transform.rotation,0);
		/* 檢查是有成功生成炸彈的 */
		if (bomb != null) {
			/* 把炸彈底下的 WebExplode 裡面的炸彈威力設定為放炸彈者的炸彈威力 */
			WebExplode exploder = bomb.GetComponent<WebExplode> ();
			exploder.bombStr  = logicman.strengthOfBomb;
			exploder.wayOfExplosion = logicman.diffWayOfExplosion;
		}
	}

	//player 碰到炸彈後會死掉
	public void OnTriggerEnter(Collider other){
		/* 人撞到 Explosion 會死掉 */
		if ((other.CompareTag ("Explosion")||other.CompareTag ("BigExplosion")) && (gameObject.GetComponent<RigidBodyFPSWalker>().dead.Equals("LIVE")||gameObject.GetComponent<RigidBodyFPSWalker>().dead.Equals("FREEZE"))) {
			Debug.Log ("hit");
			/* 呼叫在 RigidbodyFPSWalker 的 applyDead */
			transform.GetComponent<PhotonView> ().RPC ("applyDead", PhotonTargets.AllBuffered);

		} 


	}
	public void OnCollisionEnter(Collision other)
	{
		if (other.collider.CompareTag("Player")&& gameObject.GetComponent<RigidBodyFPSWalker>().isBubbled == true) {
			Debug.Log ("hit");
			//call在 RigidbodyFPSWalker
			transform.GetComponent<PhotonView> ().RPC ("RestartBomb", PhotonTargets.AllBuffered);
		} 
	}
	void plusBombNum() {
		logicman.releaseBomb ();	
	}
}
