using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class audioMixerScript : MonoBehaviour {
	public static audioMixerScript INSTANCE;

	public AudioMixerSnapshot[] snapshots;
	// Use this for initialization


	void Awake(){
		DontDestroyOnLoad (gameObject);
		if (INSTANCE == null) {
			INSTANCE = this;
		} else {
			Destroy (gameObject);
		}

	}

	public void ChangeSnapShot(int position){
		snapshots[position].TransitionTo(0);
	}






}
