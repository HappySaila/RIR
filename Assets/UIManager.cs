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

    public void SinglePlayerClicked(){
        cam.LookAtMultiplayer();
    }

    public void MultiToMain(){
        print("Hello");
        cam.LookAtMainMenu();
    }


    public void loggedIn(){
        
    }

}
