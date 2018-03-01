using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicControl : MonoBehaviour {
	
	// Use this for initialization
	private AudioSource bgMusicAudioSource;
	private AudioSource bgMusicAudioSource_1;
	public RectTransform volume;
	void Start () {
		bgMusicAudioSource_1 = GameObject.FindGameObjectWithTag ("BGM_1").GetComponent<AudioSource> ();
		bgMusicAudioSource_1.Pause ();
	}
	void Update () {

	}

	public void musicPause(){
		bgMusicAudioSource = GameObject.FindGameObjectWithTag ("BGM_2").GetComponent<AudioSource> ();

		//暫停音樂
		bgMusicAudioSource.Pause ();
		bgMusicAudioSource_1.UnPause ();
	}
	public void musicUnPause()
	{
		//繼續音樂
		bgMusicAudioSource.UnPause ();
	}
	// Update is called once per frame
	public void OnVolumeControl(){
		//bgMusicAudioSource.volume = volume.gameObject.GetComponent<Slider> ().value;
		bgMusicAudioSource_1.volume = volume.gameObject.GetComponent<Slider> ().value;
	}

}
