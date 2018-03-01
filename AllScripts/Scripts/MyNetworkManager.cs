using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyNetworkManager : Photon.PunBehaviour{
	public Camera standbyCamera;
	SpawnSpot[] spawnSpots;
	public string roomName = "Room 01";
	public string playerName = "Player 001";
	private AudioListener listener;
	public bool isConnected = false;
	public bool isInRoom= false;
	public bool isInGame = false;
	public bool isGameOver = false;

	int[] playerUsed ;

	public int playerPrefabsIndex=0;
	//player reference
	public InputField nameInputField;
	public InputField roomInputField;

	public RectTransform MenuPanel;
	public RectTransform RoomListPanel;
	public RectTransform RoomPanel;
	public RectTransform RedWinPanel;
	public RectTransform BlueWinPanel;
	public RectTransform DrawPanel;
	// Use this for initialization
	public int numberPlayers;

	//Button
	/*public Texture joinButtonImg;*/
	public GUIStyle buttonStyle;
	public Button joinButton;
	public Button LeaveButton;
	public Button playerButton;
	public Button readyButton;
	public Button setButton;
	public RectTransform Scroll;
	public RectTransform setting;

	public int redTeam;
	public int blueTeam;
	//Sprites
	public Sprite Player1;
	public Sprite Player2;
	public Sprite Player3;
	public Sprite Player4;
	public Sprite Player5;
	public Sprite Player6;
	public Sprite Player7;
	public Sprite Player8;

	public string winTeam;
	GameObject myPlayerGO;

	public CountDownManager countDownManager;
	public float count = 180f;

	public Text speedValue;
	public Text bombValue;
	public Text strengthValue;
	public Text points;

	public int speed;
	public int bomb;
	public int strength;
	public int totalPoints;

	public Transform[] Cubes;

	void Update(){
		if (isInRoom) {
			if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers) {
				PhotonNetwork.room.IsOpen = false;
			}
		}
		if (isInGame) {
			redTeam = 0;
			blueTeam = 0;
			foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
				if (pl.GetTeam ().ToString () == "red")
					redTeam++;
				else if (pl.GetTeam ().ToString () == "blue")
					blueTeam++;
			}
			int red = redTeam;
			int blue = blueTeam;
			foreach (PhotonPlayer player in PhotonNetwork.playerList) {
				if (player.GetTeam ().ToString () == "red" && player.CustomProperties ["mode"].ToString () == "DEAD")
					red--;
				else if (player.GetTeam ().ToString () == "blue" && player.CustomProperties ["mode"].ToString () == "DEAD")
					blue--;
			}
			Debug.Log (red + " / " + blue);
			if (red == 0) {
				Debug.Log ("Red Lose");
				myPlayerGO.GetComponent<RigidBodyFPSWalker> ().enabled = false;
				myPlayerGO.GetComponent<WebBomb> ().enabled = false;
				myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (true);
				myPlayerGO.transform.FindChild ("Camera").gameObject.GetComponent<SimpleSmoothMouseLook> ().lockCursor = false;
				winTeam = "blue";
				Invoke ("ChangeToBlue", 2f);
			} else if (blue == 0) {
				Debug.Log ("Blue Lose");
				myPlayerGO.GetComponent<RigidBodyFPSWalker> ().enabled = false;
				myPlayerGO.GetComponent<WebBomb> ().enabled = false;
				myPlayerGO.transform.FindChild ("Camera").gameObject.GetComponent<SimpleSmoothMouseLook> ().lockCursor = false;
				myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (true);
				winTeam = "red";
				Invoke ("ChangeToRed", 2f);
			}
			else if (countDownManager.countDown == 0) {
				if (red < blue) {
					Debug.Log ("Red Lose");
					myPlayerGO.GetComponent<RigidBodyFPSWalker> ().enabled = false;
					myPlayerGO.GetComponent<WebBomb> ().enabled = false;
					myPlayerGO.transform.FindChild ("Camera").gameObject.GetComponent<SimpleSmoothMouseLook> ().lockCursor = false;
					myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (true);
					winTeam = "blue";
					Invoke ("ChangeToBlue", 2f);
				} else if (blue > red) {
					Debug.Log ("Blue Lose");
					myPlayerGO.GetComponent<RigidBodyFPSWalker> ().enabled = false;
					myPlayerGO.GetComponent<WebBomb> ().enabled = false;
					myPlayerGO.transform.FindChild ("Camera").gameObject.GetComponent<SimpleSmoothMouseLook> ().lockCursor = false;
					myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (true);
					winTeam = "red";
					Invoke ("ChangeToRed", 2f);
				} else {
					Debug.Log ("Draw");
					myPlayerGO.GetComponent<RigidBodyFPSWalker> ().enabled = false;
					myPlayerGO.GetComponent<WebBomb> ().enabled = false;
					myPlayerGO.transform.FindChild ("Camera").gameObject.GetComponent<SimpleSmoothMouseLook> ().lockCursor = false;
					myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (true);
					winTeam = "draw";
					Invoke ("ChangeToDraw", 2f);
				}
			}
		
		} 
	}

	public void ChangeToRed(){
		RedWinPanel.gameObject.SetActive (true);
		myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (false);
		myPlayerGO.transform.FindChild ("Camera").gameObject.SetActive (false);
		standbyCamera.enabled = true;
		isInGame = false;
		isGameOver = true;

	}
	public void ChangeToBlue(){
		BlueWinPanel.gameObject.SetActive (true);
		myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (false);
		myPlayerGO.transform.FindChild ("Camera").gameObject.SetActive (false);
		standbyCamera.enabled = true;
		isInGame = false;
		isGameOver = true;
	}
	public void ChangeToDraw(){
		DrawPanel.gameObject.SetActive (true);
		myPlayerGO.transform.FindChild ("GameOver").gameObject.SetActive (false);
		myPlayerGO.transform.FindChild ("Camera").gameObject.SetActive (false);
		standbyCamera.enabled = true;
		isInGame = false;
		isGameOver = true;
	}
	//Buttons
	void Start(){
		
	}
	public void OnStart () {
		PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.ConnectUsingSettings ("Logic Bomb 1.0");
		roomName = "Room " + Random.Range (0, 999);
		//PhotonNetwork.offlineMode = true;
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		playerUsed = new int[10] ;
		MenuPanel.gameObject.SetActive (false);
		RoomListPanel.gameObject.SetActive (true);
		setButton.gameObject.SetActive (true);
		nameInputField.onEndEdit.RemoveAllListeners ();
		nameInputField.text = "Player " + Random.Range (0, 999); 
	}

	public void OnHelpButton(){
		if (Scroll.gameObject.activeSelf == false)
			Scroll.gameObject.SetActive (true);
		else
			Scroll.gameObject.SetActive (false);
	}

	public void OnSetButton(){
		if (setting.gameObject.activeSelf) {
			setting.gameObject.SetActive (false);
		} else {
			setting.gameObject.SetActive (true);
		}
	}

	public void OnCreateButton(){
		playerName = nameInputField.text;
		roomName = roomInputField.text;
		//RoomListPanel.gameObject.SetActive (false);
		PhotonNetwork.JoinOrCreateRoom (roomName,null,null);
	}

	public void OnJoinButton(){
		//SpawnMyplayer ();
		ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable ();
		hash.Add ("state", "start");
		PhotonNetwork.room.SetCustomProperties (hash);
		PhotonNetwork.room.IsOpen = false;
		PhotonNetwork.room.IsVisible = false;
		/*foreach (Transform cube in Cubes) {
			cube.GetComponent<Treasures> ().CreateTreasure ();
		}*/
	}
	public void OnReadyButton(){
		ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable ();
		hash.Add ("state", "ready");
		PhotonNetwork.player.SetCustomProperties (hash);
		readyButton.interactable = false;
	}
	public void OnLeaveButton(){
		isGameOver = false;
		isInRoom = false;
		readyButton.gameObject.SetActive (false);
		RedWinPanel.gameObject.SetActive (false);
		BlueWinPanel.gameObject.SetActive (false);
		DrawPanel.gameObject.SetActive (false);
		joinButton.gameObject.SetActive (false);
		RoomPanel.gameObject.SetActive (false);
		PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
		PhotonNetwork.LeaveRoom ();
		OnLeftRoom ();
	}

	public void OnPlayerButton(){
		//can't choose the same player
		playerPrefabsIndex=(playerPrefabsIndex+1)%8;
		if (playerUsed [playerPrefabsIndex] == 1) {
			playerPrefabsIndex=(playerPrefabsIndex+1)%8;;
			for (int i = 0; i <7; i++) {
				if (playerUsed [playerPrefabsIndex] != 1)
					break;
				else
					playerPrefabsIndex=(playerPrefabsIndex+1)%8;;
			}
		}
			
		ExitGames.Client.Photon.Hashtable index = new ExitGames.Client.Photon.Hashtable ();
		index.Add ("PlayerIndex", playerPrefabsIndex);
		PhotonNetwork.player.SetCustomProperties (index);

		if (playerPrefabsIndex == 0) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player1;
		} else if (playerPrefabsIndex == 1) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player2;
		} else if (playerPrefabsIndex == 2) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player3;
		} else if (playerPrefabsIndex == 3) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player4;
		} else if (playerPrefabsIndex == 4) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player5;
		} else if (playerPrefabsIndex == 5) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player6;
		} else if (playerPrefabsIndex == 6) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player7;
		} else {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player8;
		}
	}

	public void OnAddSpeed(){
		if (totalPoints > 0) {
			totalPoints--;
			speed++;
			points.text = totalPoints.ToString ();
			speedValue.text = speed.ToString ();
		}
	}
	public void OnAddBomb(){
		if (totalPoints > 0) {
			totalPoints--;
			bomb++;
			points.text = totalPoints.ToString ();
			bombValue.text = bomb.ToString ();
		}
	}
	public void OnAddStrength(){
		if (totalPoints > 0) {
			totalPoints--;
			strength++;
			points.text = totalPoints.ToString ();
			strengthValue.text = strength.ToString ();
		}
	}
	public void OnMinusStrength(){
		if (strength > 1) {
			totalPoints++;
			strength--;
			points.text = totalPoints.ToString ();
			strengthValue.text = strength.ToString ();
		}
	}
	public void OnMinusSpeed(){
		if (speed > 1) {
			totalPoints++;
			speed--;
			points.text = totalPoints.ToString ();
			speedValue.text = speed.ToString ();
		}
	}
	public void OnMinusBomb(){
		if (bomb > 1) {
			totalPoints++;
			bomb--;
			points.text = totalPoints.ToString ();
			bombValue.text = bomb.ToString ();
		}
	}


	public override void OnConnectedToMaster(){
		OnJoinedLobby ();
		//isConnected = true;
	}
	void OnGUI(){
		if (!isInGame)
			GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
		if (isConnected) {
			GUILayout.BeginArea (new Rect (Screen.width / 2-250, Screen.height / 2-50,500,500));
			foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
				Debug.Log ("Room");
				if (GUILayout.Button (game.Name + " " + game.PlayerCount + "/" + game.MaxPlayers)) {
					//get the player name from the input field
					playerName = nameInputField.text;

					PhotonNetwork.JoinOrCreateRoom (game.Name,null,null);
				}
			}
			GUILayout.EndArea ();
		}
		if (isInRoom) {
			//LeaveButton.gameObject.SetActive (true);
			GUILayout.BeginArea (new Rect (Screen.width / 2-250, Screen.height / 2-200,500,500));
			if (PhotonNetwork.isMasterClient) {
				joinButton.gameObject.SetActive (true);
				joinButton.interactable = false;
				bool ready = true;
				if (PhotonNetwork.room.PlayerCount == 1) {
					ready = false;
				} else {
					foreach (PhotonPlayer pl in PhotonNetwork.otherPlayers) {
						if (pl.CustomProperties ["state"].ToString () == "wait") {
							ready = false;
							break;
						}
					}
				}
				if (ready && !joinButton.IsInteractable ()) {
					joinButton.interactable = true;
				}
			} else {
				readyButton.gameObject.SetActive (true);
			}
			foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
				GUILayout.Box (pl.NickName+" | "+pl.GetTeam().ToString(),buttonStyle);				
			}
			// if the state of the room is start Game , the game start
			if (PhotonNetwork.room.CustomProperties ["state"].ToString () == "start") {
				//OnJoinButton ();
				SpawnMyplayer ();
				isInRoom = false;
			}
			GUILayout.EndArea ();
			//Update the player been choose
			for(int i=0;i<10;i++)
			{
				playerUsed [i] = 0;
				
			}
			foreach (PhotonPlayer player in PhotonNetwork.otherPlayers) {
				//Debug.Log (player.CustomProperties ["PlayerIndex"].GetHashCode());
				playerUsed [(int)player.CustomProperties ["PlayerIndex"]] = 1;
			}


		}
		if (isGameOver) {
			if (winTeam == "draw") {
				GUILayout.BeginArea (new Rect (Screen.width / 2 - 300, Screen.height / 2 - 200, 200, 500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("blue" == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.NickName, buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("red" == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.NickName, buttonStyle);

				}
				GUILayout.EndArea ();
				GUILayout.BeginArea (new Rect (Screen.width / 2 - 150, Screen.height / 2 - 200, 200, 500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("blue" == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.GetTeam ().ToString (), buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("red" == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.GetTeam ().ToString (), buttonStyle);

				}
				GUILayout.EndArea ();
				GUILayout.BeginArea (new Rect (Screen.width / 2, Screen.height / 2 - 200, 200, 500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("blue" == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.CustomProperties ["mode"].ToString (), buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("red" == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.CustomProperties ["mode"].ToString (), buttonStyle);

				}
				GUILayout.EndArea ();
				GUILayout.BeginArea (new Rect (Screen.width / 2 + 150, Screen.height / 2 - 200, 200, 500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("blue" == pl.GetTeam ().ToString ())
						GUILayout.Box ("Draw", buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if ("red" == pl.GetTeam ().ToString ())
						GUILayout.Box ("Draw", buttonStyle);

				}
				GUILayout.EndArea ();
			} else {
				GUILayout.BeginArea (new Rect (Screen.width / 2-300, Screen.height / 2-200,200,500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.NickName,buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam != pl.GetTeam ().ToString ())
						GUILayout.Box (pl.NickName,buttonStyle);

				}
				GUILayout.EndArea ();
				GUILayout.BeginArea (new Rect (Screen.width / 2-150, Screen.height / 2-200,200,500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.GetTeam().ToString(),buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam != pl.GetTeam ().ToString ())
						GUILayout.Box (pl.GetTeam().ToString(),buttonStyle);

				}
				GUILayout.EndArea ();
				GUILayout.BeginArea (new Rect (Screen.width / 2, Screen.height / 2-200,200,500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam == pl.GetTeam ().ToString ())
						GUILayout.Box (pl.CustomProperties["mode"].ToString(),buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam != pl.GetTeam ().ToString ())
						GUILayout.Box (pl.CustomProperties["mode"].ToString(),buttonStyle);

				}
				GUILayout.EndArea ();
				GUILayout.BeginArea (new Rect (Screen.width / 2+150, Screen.height / 2-200,200,500));
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam == pl.GetTeam ().ToString ())
						GUILayout.Box ("Win",buttonStyle);

				}
				foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
					if (winTeam != pl.GetTeam ().ToString ())
						GUILayout.Box ("Lose",buttonStyle);

				}
				GUILayout.EndArea ();
			}
		}

	}


	public override void OnJoinedLobby(){
		isConnected = true;
		RoomListPanel.gameObject.SetActive (true);
		Debug.Log ("Lobby");
		roomInputField.onEndEdit.RemoveAllListeners ();
		roomInputField.text = "Room " + Random.Range (0, 999); 
		//PhotonNetwork.JoinRandomRoom ();
	}
	void OnPhotonRandomJoinFailed(){
		Debug.Log ("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom (null);
	}
	public override void OnJoinedRoom()
	{

		readyButton.interactable = true;
		totalPoints = 8;
		points.text = totalPoints.ToString ();
		speed = 3;
		speedValue.text = speed.ToString ();
		bomb = 1;
		bombValue.text = bomb.ToString ();
		strength = 3;
		strengthValue.text = strength.ToString (); 

		RoomListPanel.gameObject.SetActive(false);
		RoomPanel.gameObject.SetActive (true);
		isInRoom = true;
		//set the room state to wait
		ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable ();
		hash.Add ("state", "wait");
		PhotonNetwork.room.SetCustomProperties (hash);
		ExitGames.Client.Photon.Hashtable dead= new ExitGames.Client.Photon.Hashtable ();
		dead.Add ("mode", "Live");
		PhotonNetwork.player.SetCustomProperties (dead);
		PhotonNetwork.player.SetCustomProperties (hash);
		PhotonNetwork.room.MaxPlayers = 8;
		//can't choose the same player;
		for(int i=0;i<10;i++)
		{
			playerUsed [i] = 0;

		}
		foreach (PhotonPlayer player in PhotonNetwork.otherPlayers) {
			//Debug.Log (player.CustomProperties ["PlayerIndex"].GetHashCode());
			playerUsed [(int)player.CustomProperties ["PlayerIndex"]] = 1;
		}
		if (playerUsed [playerPrefabsIndex] == 1) {
			playerPrefabsIndex=(playerPrefabsIndex+1)%8;;
			for (int i = 0; i <7; i++) {
				if (playerUsed [playerPrefabsIndex] != 1)
					break;
				else
					playerPrefabsIndex=(playerPrefabsIndex+1)%8;;
			}
		}
		ExitGames.Client.Photon.Hashtable index = new ExitGames.Client.Photon.Hashtable ();
		index.Add ("PlayerIndex", playerPrefabsIndex);
		PhotonNetwork.player.SetCustomProperties (index);


		if (playerPrefabsIndex == 0) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player1;
		} else if (playerPrefabsIndex == 1) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player2;
		} else if (playerPrefabsIndex == 2) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player3;
		} else if (playerPrefabsIndex == 3) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
			playerButton.GetComponent<Image> ().sprite = Player4;
		} else if (playerPrefabsIndex == 4) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player5;
		} else if (playerPrefabsIndex == 5) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player6;
		} else if (playerPrefabsIndex == 6) {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player7;
		} else {
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);
			playerButton.GetComponent<Image> ().sprite = Player8;
		}


		Debug.Log("OnJoinedRoom");
		PhotonNetwork.playerName = playerName;
		isConnected = false;


	}

	public void SpawnMyplayer()
	{
		listener = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioListener> ();
		listener.enabled = false;
		countDownManager.countDown = count;
		isInGame = true;
		redTeam = 0;
		blueTeam = 0;
		foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
			if (pl.GetTeam ().ToString () == "red")
				redTeam++;
			else if (pl.GetTeam ().ToString () == "blue")
				blueTeam++;
		}
		setButton.gameObject.SetActive (false);
		RoomPanel.gameObject.SetActive (false);
		if (spawnSpots == null) {
			Debug.LogError ("no SpawnSpot");
			return;
		}
		Debug.Log (spawnSpots.Length);
		CheckPlayers ();
		Debug.Log (numberPlayers);

		//use the playerID to decide where to  spawn
		SpawnSpot mySpawnSpot = spawnSpots[playerPrefabsIndex%8];

		if(playerPrefabsIndex == 0)
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("logicman_3", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0);
		else if(playerPrefabsIndex == 1)
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("logicman_1", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0); 
		else if(playerPrefabsIndex == 2)
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("logicman_4", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0);
		else if(playerPrefabsIndex == 3)
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("logicman2", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0);
		else if(playerPrefabsIndex == 4)
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("Pirate", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0); 
		else if(playerPrefabsIndex == 5)
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("Policeman", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0);
		else if(playerPrefabsIndex == 6)
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("ScientistRig", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0);
		else
			myPlayerGO = (GameObject)PhotonNetwork.Instantiate("TrainDriver", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation,0);
		myPlayerGO.gameObject.GetComponent<AudioListener> ().enabled = true;

		//ENABLED THE DISABLED COMPONENT IN THE PLAYER
		//為了不會控制到別人的PLAYER
		myPlayerGO.GetComponent<RigidBodyFPSWalker> ().enabled = true;
		myPlayerGO.GetComponent<WebBomb> ().enabled = true;
		myPlayerGO.GetComponent<playertransition> ().enabled = true;
		myPlayerGO.transform.FindChild ("Camera").gameObject.SetActive (true);
		myPlayerGO.GetComponent<RigidBodyFPSWalker> ().speed = speed;
		myPlayerGO.GetComponent<RigidBodyFPSWalker> ().oriSpeed = speed;
		myPlayerGO.GetComponent<RigidBodyFPSWalker> ().strengthOfBomb = strength;
		myPlayerGO.GetComponent<RigidBodyFPSWalker> ().numberOfBomb = bomb;
		myPlayerGO.GetComponent<RigidBodyFPSWalker> ().currentBomb = bomb;
		standbyCamera.enabled = false;
		
	}

	public int CheckReadyPlayers()
	{
		//CHECK HOW MANY PLAYERS ready IN THE ROOM
		int	readyPlayers = 0;
		foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
			if (pl.CustomProperties["ready"] == "Ready")
				readyPlayers++;
			
		}
		return readyPlayers;

	}
	void CheckPlayers()
	{
		//CHECK HOW MANY PLAYERS IN THE ROOM
		foreach (PhotonPlayer pl in PhotonNetwork.playerList) {
			numberPlayers++;

		}
	}
}
