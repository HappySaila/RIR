using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RSManager : MonoBehaviour {
	public bool isAI;
	public bool isRed;
    public float SpeedCap;
	NavMeshAgent agent;

	[Tooltip("Collider that makes robot hover above ground.")] public SphereCollider hoverBase;

	[HideInInspector] public RSMovement robotMovement;
    [HideInInspector] public RSAttack robotAttack;
	[HideInInspector] public RobotFollow robotFollow;
	[HideInInspector] public Rigidbody rigid;
	[HideInInspector] public RSLaborerControl robotLaborerControl;

	AudioSource audioSource;
	public AudioListener audioListener;
	public AudioListener FlyCamaudioListener;
	[HideInInspector] public bool isMainPlayer;

	// Use this for initialization
	void Start () {
		robotMovement = GetComponentInChildren<RSMovement>();
        robotAttack = GetComponentInChildren<RSAttack>();
		robotFollow = GetComponentInChildren<RobotFollow>();
		rigid = GetComponentInChildren<Rigidbody>();

		robotLaborerControl = GetComponentInChildren<RSLaborerControl>();
		agent = GetComponentInChildren<NavMeshAgent>();

		if (isAI)
		{
            //turn off cameras
            robotFollow.DisableCameras();
			robotFollow.enabled = false;
			robotMovement.moveSpeed = 0;
		}

		//sound town
		audioSource= GetComponentInChildren<AudioSource>();
		isMainPlayer=false;
		//if in multiplayer dont need to do the is red check
		if(audioListener!=null && !isRed && !isAI){//at the start only the blue player in single and split screen
			isMainPlayer=true;
			audioListener.enabled = true;
		}


		
	
	}


	public void playSound(string sound){//this methord exsist so that the audio souce is accses through here

		switch (sound) {
			case "hit":
				SoundManager.INSTANCE.PlayRamHit (audioSource);
				break;
		case "collectLabourer":
				SoundManager.INSTANCE.PlayLabourPickUp (audioSource);
				break;
		case "hammer":
			SoundManager.INSTANCE.PlayHammer (audioSource);
				break;
			case "spannereeraSDASF":
				//SoundManager.INSTANCE.PlayRamHit (audioSource);
				break;

		}

	

	}

    public void SetPlayerOne(bool isPlayerOne){
        robotMovement.isOnePlayer = isPlayerOne;
        robotAttack.isPlayerOne = isPlayerOne;
    }

	public void Die()
	{
		
		//stop attacking
		robotAttack.enabled = false;

		//so there is still and audioListener
		if(audioListener!=null){
			
			if(audioListener.enabled){
				FlyCamaudioListener.enabled=true;
			}
		}

		//player has been hit and will turn into a laborer
		//stop movement
		robotMovement.Die();
		robotMovement.enabled = false;
		hoverBase.enabled = false;
		GetComponentInChildren<NavMeshAgent>().enabled = false;



        //set cameras - disable camera colliders and enable ghost traveling camera
        robotFollow.SetColliders(false);
        robotFollow.EnableFreeFly();


		//set player to laborer
		robotLaborerControl.isFighter = false;
		robotLaborerControl.CallSetLaborer();
		robotLaborerControl.isIdleLaborer = true;
        GetComponentInChildren<ColorRobot>().SetGrey();


        //detach robot from controlling parent
        if (GetComponentInParent<RSController>()!=null){
			GetComponentInParent<RSController>().Die();
		}

		robotFollow.enabled = false;
            
	}

    private void Update()
    {
        if (rigid.velocity.magnitude > SpeedCap && isAI){
            rigid.velocity = rigid.velocity.normalized * SpeedCap;
        }
    }
}
