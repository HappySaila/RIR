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
    public GameObject FadeCanvas;
    public GameObject FadeFromBlackPrefab;
    public GameObject WaitingRobot;
    GameObject FadeCanvasInstance;
    int sceneNumber = 0;
    AudioSource audioSource;
    public GameObject forgeCanvas;
    GameObject forgeCanvasInstance;

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

    private void Start()
    {
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
        FadeBlack();
		SoundManager.INSTANCE.PlayButtonClicked (audioSource);
		forgeCanvasInstance = (GameObject)Instantiate(forgeCanvas, transform.position, transform.rotation);
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
        if (ValidateRoomSize()){
			cam.LookAtWaitingRoom();
			MasterServerScript.instance.createRoomButtonPressed(RoomName.text, int.Parse(RoomNumber.text));
		}
    }

    public void JoinClicked(){
        SpawnRobots(4);
		cam.LookAtWaitingRoom();
        return;
        MasterServerScript.instance.joinRoomButtonPressed(RoomName.text);
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
        cam.LookAtCreateRoom();
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
        Instantiate(FadeFromBlackPrefab, transform.position, Quaternion.identity);
    }

    public void DestroyFadeBlackCanvas(){
        Destroy(FadeCanvasInstance);
    }

    public void SpawnRobots(int i){
        foreach (Transform t in waitingRoomSpawnPositions){
            GameObject robot = Instantiate(WaitingRobot, t.position, t.rotation);
            robot.GetComponent<RenameRobot>().Rename("Ana da bot");
        }
    }
}
