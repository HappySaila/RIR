using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public UICamera cam;
    public masterServerManager masterServer;
    public InputField RegisterPassword;
    public InputField RegisterPasswordConfirm;
    public Text RegisterErrorMessage;
	public Text RoomErrorMessage;
	public Text RoomNumber;

    private void Awake()
    {
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        masterServer = GetComponent<masterServerManager>();
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
    public void LogInClicked(){
        cam.LookAtCreateRoom();
    }

    public void checkRoomNumber(){
        if (RoomNumber.text.Equals("1") || RoomNumber.text.Equals("2") || RoomNumber.text.Equals("3") || RoomNumber.text.Equals("4")){
			RoomErrorMessage.text = "";
            RoomErrorMessage.enabled = false;
        } else {
			RoomErrorMessage.text = "Please enter a room number between 1-4.";
			RoomErrorMessage.enabled = true;
        }
    }

	public bool isPasswordsMatching()
	{
        if (!RegisterPassword.text.Equals(RegisterPasswordConfirm.text))
		{
			RegisterErrorMessage.enabled = true;
			RegisterErrorMessage.text = "Passwords do not match.";
            return false;
		}
		else
		{
			RegisterErrorMessage.enabled = false;
			RegisterPassword.text = "";
            return true;
		}
	}

    public void PasswordsMatching(){
        if (!RegisterPassword.text.Equals(RegisterPasswordConfirm.text)){
            RegisterErrorMessage.enabled = true;
            RegisterErrorMessage.text = "Passwords do not match.";
        } else {
            RegisterErrorMessage.enabled = false;
            RegisterErrorMessage.text = "";
		}
    }

    //join room buttons
    public void CreateClicked(){
		if (!RoomErrorMessage.enabled)
		{
            masterServer.createRoom();
		}
    }

    public void JoinClicked(){
        if (!RoomErrorMessage.enabled){
            masterServer.joinRoom();
        }
    }

    public void loggedIn(){
        cam.LookAtCreateRoom();
    }

}
