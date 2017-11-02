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
    public GameObject FadeCanvas;
    int sceneNumber = 0;
	AudioSource audioSource;

    private void Awake()
    {
		audioSource= GetComponentInChildren<AudioSource>();
		audioMixerScript.INSTANCE.ChangeSnapShot (0);
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)){
        }
    }

    private void Start()
    {
        masterServer = GetComponent<masterServerManager>();
    }

    //single player buttons
    public void SinglePlayerClicked(){
		FadeMusic();
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
        Invoke("LoadScene", 2f);
        sceneNumber = 1;
        FadeBlack();
	}

    public void LoadScene(){
        //will load scene(sceneNumber)
        SceneManager.LoadScene(sceneNumber);
        print("Load scene");
    }

    public void SplitScreenClicked(){
		FadeMusic();
		SoundManager.INSTANCE.PlayButtonClicked(audioSource);
		Invoke("LoadScene", 2f);
		sceneNumber = 2;
		FadeBlack();
    }

    public void MultiPlayerClicked(){
		
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
		if (!RoomErrorMessage.enabled)
		{
            masterServer.createRoom();
		}
    }

    public void JoinClicked(){
        cam.LookAtWaitingRoom();
    }

    public void loggedIn(){
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
        cam.LookAtCreateRoom();
    }

    public void GameOver(){
        cam.LookAtGameOverRoom();
    }


    void FadeMusic(){
        audioMixerScript.INSTANCE.Mute();
    }

    void FadeBlack(){
        Instantiate(FadeCanvas, transform.position, Quaternion.identity);
    }

}
