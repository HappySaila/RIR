using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableSound : MonoBehaviour {

	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnCollisionEnter(Collision col)
	{
		audioSource.volume =col.relativeVelocity.sqrMagnitude;
		if(audioSource.volume>0.5f){
			//Debug.Log (col.relativeVelocity);
			SoundManager.INSTANCE.PlayRamHit (audioSource);
		}
	

	}
}
