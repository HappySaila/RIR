using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechManager : MonoBehaviour
{
    public static SpeechManager instance;
    public Text speechText;
    public Canvas mainCanvas;
    public Animator SpeechA;
    public Animator RobotA;
    public Animator RomanA;
    public Animator FadeBlack;

    bool RobotVisible;
    bool SpeechVisible;
    bool RomanVisible;

    void Robot(string s)
    {
        StartCoroutine(RobotSpeak(s));
    }

    void Roman(string s)
    {
        StartCoroutine(RomanSpeak(s));
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator RobotSpeak(string text)
    {
        speechText.text = text;
        if (!RobotVisible)
        {
            RobotA.SetTrigger("In");
            RobotVisible = true;
            SoundManager.INSTANCE.PlayRobotScene();
        }

        yield return new WaitForSeconds(1);

        if (!SpeechVisible)
        {
            SpeechA.SetTrigger("In");
            FadeBlack.SetTrigger("In");
            SpeechVisible = true;
            mainCanvas.enabled = true;
        }

    }

    IEnumerator RomanSpeak(string text)
    {
        speechText.text = text;

        if (!RomanVisible)
        {
            RomanA.SetTrigger("In");
            RomanVisible = true;
            SoundManager.INSTANCE.PlayGladiatorScene();
        }

        yield return new WaitForSeconds(1);

        if (!SpeechVisible)
        {
            SpeechA.SetTrigger("In");
			FadeBlack.SetTrigger("In");
			SpeechVisible = true;
			mainCanvas.enabled = true;
		}
    }

    void AllLeave()
    {
        StartCoroutine(Leave());
    }

	public void StartCantPlaySplitScreen (){
		StartCoroutine (StartCantPlaySplitScreenE ());
	}

	IEnumerator StartCantPlaySplitScreenE()
	{
		//yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("Hay.You can't do that!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Roman("You first need to plug in an xbox remote to do that.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(2f);;
		AllLeave();
	}

    IEnumerator Leave(){
        yield return new WaitForSeconds(0.5f);
		if (SpeechVisible)
		{
			SpeechA.SetTrigger("Out");
			FadeBlack.SetTrigger("Out");
			SpeechVisible = false;
		}

        RomanLeave();

        RobotLeave();
		
		SoundManager.INSTANCE.PlaySwoosh();
		yield return new WaitForSeconds(1);
		mainCanvas.enabled = false;
		SpeechA.SetTrigger("Reset");
		RomanA.SetTrigger("Reset");
		RobotA.SetTrigger("Reset");
		FadeBlack.ResetTrigger("Reset");
	}

    public void RobotLeave(){
		if (RobotVisible)
		{
			RobotA.SetTrigger("Out");
			RobotVisible = false;
		} 
    }

    public void RomanLeave(){
		if (RomanVisible)
		{
			RomanA.SetTrigger("Out");
			RomanVisible = false;
		}
    }

    public void StartScene(){
        StartCoroutine(StartSceneE());
    }

    IEnumerator StartSceneE()
    {
        yield return new WaitForSeconds(1);
        int i = Random.Range(0, 100) % 9;
        switch (i){
            case 0:
                StartCoroutine(Welcome());
				break;
			case 1:
				StartCoroutine(Welcome2());
				break;
			case 2:
				StartCoroutine(Welcome3());
				break;
			case 3:
				StartCoroutine(Welcome4());
				break;
			case 4:
				StartCoroutine(Welcome5());
				break;
			case 5:
				StartCoroutine(Welcome6());
				break;
			case 6:
				StartCoroutine(Welcome7());
				break;
			case 7:
				StartCoroutine(Welcome8());
				break;
			case 8:
				StartCoroutine(Welcome9());
				break;
        }
    }

    public void GameOverScene(){
		int i = Random.Range(0, 100) % 5;
		switch (i)
		{
			case 0:
                StartCoroutine(GameOver1());
				break;
			case 1:
                StartCoroutine(GameOver2());
				break;
			case 2:
                StartCoroutine(GameOver3());
				break;
			case 3:
                StartCoroutine(GameOver4());
				break;
			case 4:
				StartCoroutine(GameOver5());
				break;
		}
    }

    public void HowToPlay(){
        StartCoroutine(HowToPlayE());
    }

    public void HowToMultiplayer(){
        StartCoroutine(HowToMultiplayerE());
    }

    IEnumerator HowToPlayE(){
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("You new huh?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f); ;
		Robot("Well then, Welcome to Robots in Rome!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f); ;
		Robot("Your objective is simple. Gather enough robots to build your time machine.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f); ;
		Robot("Once your time machine is built, we can go back to our time.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
        Robot("To move your bot, use the WASD controls.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("And to attack, use the space bar.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
        Robot("You got it?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("WASD - movement\nSPACE - attack\nE - look behind\nQ - taunt");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
        RobotLeave();
        Roman("Have any friends?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
        Roman("Make sure you connect an external controller.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Roman("To interact with your robot with the XBOX controller, just do the following.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Roman("Left stick - movement\nA button- attack\nX button- look behind\nB button - taunt");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Roman("Now get in there and fight!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("Yes sir!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		AllLeave();
    }

    IEnumerator HowToMultiplayerE(){
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("To run multiplayer you are going to need a host.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("To run a host you are going to have to open a new instance of Robots in Rome. Click multiplayer and click host on the Forge interface.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("The instance hosting the game will not be interactable.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("All clients will have to connect to the hosting machines IP address.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("If the host is found, it will be filled in automatically in the IP address feild in the Forge interface.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Robot("Once that is all done you will be connected!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);
		RobotLeave();
        Roman("To start a game you are going to have to create or join a room.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Roman("If you create a room, your friends can join it and you can fight along side each other.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Roman("If you create a room of size three, once your room is full it will search for another room of size three and join the two rooms for " +
              "an epic three vs three battle.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
		Roman("Make sure the room names match when you are joining.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
        Robot("Yes sir!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);
        AllLeave();
    }

    public void Hosting(){
        Robot("Hosting - Game no longer active. \nStart up client instances and connect!");
        Roman("Hosting - Game no longer active. \nStart up client instances and connect!");
    }

    #region welcome scenes
    IEnumerator Welcome()
    {
        Robot("Finally! Someone came to rescue us!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);
        Robot("What? Your time machine is broken too?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("You are going to be stuck here for a long time.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Roman("Get back to work!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome2()
    {
        Robot("Where were you?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("You missed such a good fight!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("Anyways, I think you are next!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome3()
    {
        Roman("A new worker?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Roman("Welcome to the end of your miserable life.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Roman("Get working!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome4()
    {
        Robot("Ah man! So much oil leaks in that last round.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("We got to build that time machine.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("Got to get back to the present!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome5()
    {
        Roman("Through that door robot.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("Yes sir. No time to waste. Got to go fight, fight,fight");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome6()
    {
        Roman("Robot! I have been meaning to tell you.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Roman("Your fighting has been really impressive. Keep it up!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("Yes sir!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome7()
    {
        Robot("Robot friend! Where have you been?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("Grab a hammer and help me build this time machine.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome8()
    {
        Roman("There have been rumours that you and your friends have been trying to escape?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Roman("Is this true?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("Err - no. We just want to fight. Fight all day and all night!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }

    IEnumerator Welcome9()
    {
        Roman("Robot! Why are you wandering around?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Roman("Get in the arena and fight!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        Robot("Yes sir!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.1f);;
        AllLeave();
    }
	#endregion

	#region GameOverScenes
	IEnumerator GameOver1()
	{
		Roman("That was a great fight!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Roman("Very strong and very entertaining.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Roman("Especially the part when the one robot rammed into the other one.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Roman("Truely great stuff!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		AllLeave();
	}

	IEnumerator GameOver2()
	{
		Robot("Yes! We win!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Robot("Our time machine is built and we are getting out of here!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Robot("Oil? Check. Lets go!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
        Roman("Great fight!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
        AllLeave();
	}

	IEnumerator GameOver3()
	{
		Robot("All robots okay?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Robot("Robot X? How is your head?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Roman("Haha! Great fight! Another! Can we get another?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		AllLeave();
	}

	IEnumerator GameOver4()
	{
		Robot("A fight to the death they wanted? A fight to the death they got!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Robot("Robot X? Get in the time machine! We leaving.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Robot("Back to the present. Cant wait to see my wife bot and kid bots!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		AllLeave();
	}

	IEnumerator GameOver5()
	{
		Robot("My word! We won.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Roman("Great fight! At first I thought blue was going to win.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Roman("And then red was coming back.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		Roman("And then back forth back forth. What an outcome!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForSeconds(0.1f);;
		AllLeave();
	}
    #endregion
}
