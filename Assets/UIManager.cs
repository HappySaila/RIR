using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public UICamera cam;
    public InputField RegisterPassword;
    public InputField RegisterPasswordConfirm;
    public Text RegisterErrorMessage;
	public Text RoomErrorMessage;
	public Text RoomNumber;

	AudioSource audioSource;

    private void Awake()
    {
		audioSource= GetComponentInChildren<AudioSource>();

        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    //single player buttons
    public void SinglePlayerClicked(){
		audioMixerScript.INSTANCE.ChangeSnapShot (0);
		SoundManager.INSTANCE.PlayRamHit (audioSource);
		SceneManager.LoadScene(2);

	}

    public void SplitScreenClicked(){
		audioMixerScript.INSTANCE.ChangeSnapShot (0);
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
        SceneManager.LoadScene(1);
    }

    public void MultiPlayerClicked(){
		audioMixerScript.INSTANCE.ChangeSnapShot (0);
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
		cam.LookAtMultiplayer();
	}

    public void BackToMainMenu(){
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
        cam.LookAtMainMenu();
    }

    //register join buttons
    public void LogInClicked(){
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
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
        
    }

    public void JoinClicked(){
        
    }

    public void loggedIn(){
        cam.LookAtCreateRoom();
    }

}
