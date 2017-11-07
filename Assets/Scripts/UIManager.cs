using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public UICamera cam;
	public Transform[] waitingRoomSpawnPositions;
    public InputField RegisterPassword;
    public InputField RegisterPasswordConfirm;
    public Text RegisterErrorMessage;
    public Text RoomErrorMessage;
    public Text RoomNumber;
    public Text RoomName;
    public Text WaitingRoomName;
    public GameObject FadeCanvas;
    public GameObject FadeFromBlackPrefab;
    public GameObject WaitingRobot;
    GameObject FadeCanvasInstance;
    string sceneName = "MainMenu";
    AudioSource audioSource;
    public GameObject forgeCanvas;
    GameObject forgeCanvasInstance;
    int waitingRobots=0;

    private void Awake()
    {
		audioSource= GetComponentInChildren<AudioSource>();
        if (instance == null){
            instance = this;
            PlayerPrefs.SetInt("GameOver", 0);
			SpeechManager.instance.StartScene();
		} else {
            Destroy(gameObject);
        }
	}

    private void Start()
    {
        Invoke("CheckConnection", 1f);
		audioMixerScript.INSTANCE.ChangeSnapShot(0);
	}

    void CheckConnection(){
		if (MasterServerScript.instance != null)
		{
			cam.LookAtMultiplayer();
		}
    }
    //single player buttons
    public void SinglePlayerClicked(){
		FadeMusic();
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
        Invoke("LoadScene", 2f);
        sceneName = "SinglePlayer";
        FadeBlack();
	}

    public void LoadScene(){
        //will load scene(sceneNumber)
        SceneManager.LoadScene(sceneName);
    }

    public void SplitScreenClicked(){
		FadeMusic();
		SoundManager.INSTANCE.PlayButtonClicked(audioSource);
		Invoke("LoadScene", 2f);
		sceneName = "SplitScreen";
		FadeBlack();
    }

    public void MultiPlayerClicked(){
        SoundManager.INSTANCE.PlayButtonClicked (audioSource);
        if (MasterServerScript.instance == null){
			FadeBlack();
			forgeCanvasInstance = (GameObject)Instantiate(forgeCanvas, transform.position, transform.rotation);
		}
        cam.LookAtMultiplayer();
    }

	public void QuitClicked(){
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
		Application.Quit ();
	}

    public void MultiplayerConnected(){
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
        if (ValidateRoomSize() && ValidateRoomName()){
			cam.LookAtWaitingRoom();
			MasterServerScript.instance.createRoomButtonPressed(RoomName.text, int.Parse(RoomNumber.text));
			StartCoroutine(SpawnWaitingRobots());
			WaitingRoomName.text = RoomName.text;
			SoundManager.INSTANCE.PlayButtonClicked(audioSource);
		}
    }

    bool ValidateRoomName(){
        if (RoomName.text == "Room name..." || RoomName.text.Length == 0){
            RoomErrorMessage.text = "Enter a room name";
            return false;
        } else {
            RoomErrorMessage.text = "";
            return true;
        }
    }

    public void JoinClicked()
    {
        if (GlobalVariables.instance.existingRooms.Exists(i => i.RoomName == RoomName.text))
        {
            MasterServerScript.instance.joinRoomButtonPressed(RoomName.text);
            cam.LookAtWaitingRoom();
            StartCoroutine(SpawnWaitingRobots());
            RoomErrorMessage.text = "";
            WaitingRoomName.text = RoomName.text;
        } else {
            RoomErrorMessage.text = "room doesnt exists";
        }
    }

    IEnumerator SpawnWaitingRobots(){
		yield return new WaitForSeconds(0.5f);
        if (waitingRobots < MasterServerScript.instance.myRoom.Players.Count){
            Debug.LogFormat("Name: {0}", MasterServerScript.instance.myRoom.Players[waitingRobots].Name);
            //we need to spawn a player
            SpawnRobot(waitingRobots, MasterServerScript.instance.myRoom.Players[waitingRobots].Name);
            waitingRobots++;
        }

        if (waitingRobots < MasterServerScript.instance.myRoom.RoomSize){
            //starting match making
            StartCoroutine(SpawnWaitingRobots());
        }

    }

    public bool ValidateRoomSize(){
        string s = RoomNumber.text;
        if (s=="1" || s == "2" || s == "3" || s == "4"){
			RoomErrorMessage.text = "";
			return true;
        } else {
            RoomErrorMessage.text = "room size must be between one and four";
            return false;
        }
    }

    public void loggedIn(){
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
		GetComponent<RegisterScript>().error.text = "";
        GetComponent<LoginScript>().error.text = "";
        cam.LookAtCreateRoom();
        SpeechManager.instance.HowToMultiplayer();
    }

    public void GameOver(){
        cam.LookAtGameOverRoom();
    }


    void FadeMusic(){
        audioMixerScript.INSTANCE.Mute();
    }

    void FadeBlack(){
        FadeCanvasInstance = (GameObject) Instantiate(FadeCanvas, transform.position, Quaternion.identity);
    }

    public void FadeFromBlack(){
        DestroyFadeBlackCanvas();
        //Instantiate(FadeFromBlackPrefab, transform.position, Quaternion.identity);
    }

    public void DestroyFadeBlackCanvas(){
        if (FadeCanvas!=null){
			Destroy(FadeCanvasInstance);
		}
	}
            

    public void SpawnRobot(int playerNumber, string Name){
		BMSLogger.Instance.LogFormat("Player number {0}", playerNumber);
		Transform spawnPos = waitingRoomSpawnPositions[playerNumber];
		GameObject robot = Instantiate(WaitingRobot, spawnPos.position, spawnPos.rotation);
		robot.GetComponent<RenameRobot>().Rename(Name);
    }

}
