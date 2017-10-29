using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public static HUDManager instance;
	public Text BlueScore;
	public Text RedScore;

    private void Awake()
    {
        instance = this;
    }

    public void SetScores(){
		RedScore.text = TimeMachine.redTimeMachine.AvailableLaborers.Count + "";
		BlueScore.text = TimeMachine.blueTimeMachine.AvailableLaborers.Count + "";
    }
}
