using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public UICamera cam;

    private void Awake()
    {
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    //single player buttons
    public void SinglePlayerClicked(){
		SceneManager.LoadScene(2);
	}

    public void SplitScreenClicked(){
        SceneManager.LoadScene(1);
    }

    public void MultiPlayerClicked(){
		cam.LookAtMultiplayer();
	}

    public void BackToMainMenu(){
        cam.LookAtMainMenu();
    }

    //register join buttons
    public void RegisterClicked(){
        
    }

    public void LogInClicked(){
        cam.LookAtCreateRoom();
    }

    //join room buttons
    public void CreateClicked(){
        
    }

    public void JoinClicked(){
        
    }

    public void loggedIn(){
        
    }

}
