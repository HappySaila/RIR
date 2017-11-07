using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public Text winner;
    public MeshRenderer rend;
    public GameObject fadeOut;

    void Start(){
        if (PlayerPrefs.GetInt("Winner") == 1){
            winner.text =  "red team wins!";
            winner.color = Color.red;
            rend.material.color = Color.red;
        } else {
			winner.text = "blue team wins!";
            winner.color = Color.blue;
			rend.material.color = Color.blue;
		}
        audioMixerScript.INSTANCE.FadeInIntro();
        SpeechManager.instance.GameOverScene();
        TimeMachine.canEndGame = true;
	}
    public void ReturnToMainMenu(){
        Instantiate(fadeOut, transform.position, Quaternion.identity);
        Invoke("BackToMain", 3);
        SoundManager.INSTANCE.PlayButtonClicked(GetComponent<AudioSource>());
	}

    public void BackToMain(){
		SceneManager.LoadScene("MainMenu");
	}
        
}
