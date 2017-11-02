using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMenuManager : MonoBehaviour {
    public static BackToMenuManager instance;

	void Awake()
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

    public void LookAtGameOver(){
        UIManager.instance.GameOver();
    }
}
