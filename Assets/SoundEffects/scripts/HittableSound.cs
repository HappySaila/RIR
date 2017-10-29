using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this script exsists to be added to all hittable objects by the hittablesearch script in order for them to make sounds on impact.
public class HittableSound : MonoBehaviour
{

	AudioSource audioSource;
	private bool alowedToMakeNoise;



	// Use this for initialization
	void Start ()
	{
		audioSource = gameObject.GetComponent<AudioSource> ();

		alowedToMakeNoise = false;
		StartCoroutine (DelayedNoise ());

	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public void OnCollisionEnter (Collision col)
	{
		
		if (col.relativeVelocity.sqrMagnitude > 0.5f &&alowedToMakeNoise) {
			audioSource.volume = (col.relativeVelocity.sqrMagnitude / 5) + 0.5f;
			audioSource.pitch = Random.Range (-2.9f, 2.9f);
			SoundManager.INSTANCE.PlayRamHit (audioSource);
		}
	
		//Debug.Log (col.relativeVelocity+" "+audioSource.volume);
	}
	IEnumerator DelayedNoise ()
	{	
		yield return new WaitForSeconds (1.5f);
		alowedToMakeNoise = true;

	}

}
