using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimate : MonoBehaviour {
    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(Animate());
	}

    IEnumerator Animate(){
        yield return new WaitForSeconds(Random.Range(5, 20));
        anim.SetInteger("IdleNumber", Random.Range(1,100) % 4);
        anim.SetTrigger("RandomIdle");
        StartCoroutine(Animate());
    }
	
	
}
