using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDManager : MonoBehaviour {
    public static HUDManager instance;
	public Text BlueScore;
	public Text RedScore;

    private void Awake()
    {
        instance = this;
    }

    public void Start(){
        StartCoroutine(UpdateScore());
    }

    IEnumerator UpdateScore(){
        yield return new WaitForSeconds(1f);
        StartCoroutine(UpdateScore());
        RedScore.text = (int)(TimeMachine.redTimeMachine.currentProgress) + "%";
        BlueScore.text = (int)(TimeMachine.blueTimeMachine.currentProgress) + "%";
    }
}
