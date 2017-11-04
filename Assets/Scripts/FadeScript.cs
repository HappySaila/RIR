using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour {
    public Image fadeBlack;
    public bool fadeBlackIn;
	// Use this for initialization
	void Start () {
        if (fadeBlackIn){
			StartCoroutine(FadeIn());
        } else {
            fadeBlack.color = Color.black;
            Invoke("StartFadeOut", 1f);
		}
            
	}

    void StartFadeOut(){
		StartCoroutine(FadeOut());
	}

    IEnumerator FadeOut(){
        fadeBlack.color = Color.Lerp(fadeBlack.color, Color.clear, Time.deltaTime * 2);
		yield return new WaitForEndOfFrame();
		if (fadeBlack.color.a > 0.01)
		{
            StartCoroutine(FadeOut());
		}
    }

	// Update is called once per frame
	IEnumerator FadeIn()
	{
		fadeBlack.color = Color.Lerp(fadeBlack.color, Color.black, Time.deltaTime * 2);
		yield return new WaitForEndOfFrame();
        if (fadeBlack.color.a < 0.99)
        {
			StartCoroutine(FadeIn());
		}
	}
}
