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

    IEnumerator Leave(){
        yield return new WaitForSeconds(0.5f);
		if (SpeechVisible)
		{
			SpeechA.SetTrigger("Out");
			FadeBlack.SetTrigger("Out");
			SpeechVisible = false;
		}

		if (RomanVisible)
		{
			RomanA.SetTrigger("Out");
			RomanVisible = false;
		}

		if (RobotVisible)
		{
			RobotA.SetTrigger("Out");
			RobotVisible = false;
		}
        SoundManager.INSTANCE.PlaySwoosh();
		yield return new WaitForSeconds(1);
        mainCanvas.enabled = false;
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

    #region welcome scenes
    IEnumerator Welcome()
    {
        Robot("Finally! Someone came to rescue us!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("What? Your time machine is broken too?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("You are going to be stuck here for a long time.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Roman("Get back to work!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome2()
    {
        Robot("Where were you?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("You missed such a good fight!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("Anyways, I think you are next!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome3()
    {
        Roman("A new worker?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Roman("Welcome to the end of your miserable life.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Roman("Get working!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome4()
    {
        Robot("Ah man! So much oil leaks in that last round.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("We got to build that time machine.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("Got to get back to the present!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome5()
    {
        Roman("Through that door robot.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("Yes sir. No time to waste. Got to go fight, fight,fight");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome6()
    {
        Roman("Robot! I have been meaning to tell you.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Roman("Your fighting has been really impressive. Keep it up!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("Yes sir!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome7()
    {
        Robot("Robot friend! Where have you been?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("Grab a hammer and help me build this time machine.");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome8()
    {
        Roman("There have been rumours that you and your friends have been trying to escape?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Roman("Is this true?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("Err - no. We just want to fight. Fight all day and all night!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }

    IEnumerator Welcome9()
    {
        Roman("Robot! Why are you wandering around?");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Roman("Get in the arena and fight!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        Robot("Yes sir!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForEndOfFrame();
        AllLeave();
    }
	#endregion

	#region GameOverScenes
	IEnumerator GameOver1()
	{
		Roman("That was a great fight!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Roman("Very strong and very entertaining.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Roman("Especially the part when the one robot rammed into the other one.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Roman("Truely great stuff!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		AllLeave();
	}

	IEnumerator GameOver2()
	{
		Robot("Yes! We win!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Robot("Our time machine is built and we are getting out of here!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Robot("Oil? Check. Lets go!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
        Roman("Great fight!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
        AllLeave();
	}

	IEnumerator GameOver3()
	{
		Robot("All robots okay?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Robot("Robot X492? How is your head?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Roman("Haha! Great fight! Another! Can we get another?");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		AllLeave();
	}

	IEnumerator GameOver4()
	{
		Robot("A fight to the death the wanted? A fight to the death they got!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Robot("Robot X492? Get in the time machine! We leaving.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Robot("Back to the present. Cant wait to see my wife bot and kid bots!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		AllLeave();
	}

	IEnumerator GameOver5()
	{
		Robot("My word! We won.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Roman("Great fight! At first I thought blue was going to win.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Roman("And then red was coming back.");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		Roman("And then back forth back forth. What an outcome!");
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		yield return new WaitForEndOfFrame();
		AllLeave();
	}
    #endregion
}
