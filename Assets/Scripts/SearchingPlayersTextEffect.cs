using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchingPlayersTextEffect : MonoBehaviour {
    Text text;
    public string textString;
    public float tickSpeed;
    int i = 0;
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        StartCoroutine(Effect());
	}
	
    IEnumerator Effect(){
        yield return new WaitForSeconds(tickSpeed);
        if (i == 0)
        {
            text.text = textString + ".";
        }
        else if(i==1){
            text.text = textString + "..";
        }else {
            text.text = textString + "...";
            i = -1;
        }
        i++;
        StartCoroutine(Effect());
    }
}
