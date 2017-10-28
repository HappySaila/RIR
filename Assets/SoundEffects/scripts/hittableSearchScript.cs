using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hittableSearchScript : MonoBehaviour
{

	GameObject[] hittableGameObjects;
	public bool HaveColidersOnEveryHittable;

	void Start ()
	{
		
		if (HaveColidersOnEveryHittable) {
			hittableGameObjects = GameObject.FindGameObjectsWithTag ("Hittable");
			foreach (GameObject hittable in hittableGameObjects) {
				hittable.AddComponent<AudioSource> ();
				hittable.AddComponent<HittableSound> ();

			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
