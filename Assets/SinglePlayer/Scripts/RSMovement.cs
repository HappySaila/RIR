using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RSMovement : MonoBehaviour
{
	[HideInInspector] public bool canMove = true;
	Transform AITarget;
	RSManager robotTarget;
	AIMovementData AIData;
	public float turnSpeed;
	public float moveSpeed;
	float maxVelocityChange = 5;
	float prevX;

	Animator anim;
	Rigidbody rigid;
	RSManager robotManager;
	NavMeshAgent agent;

	int numberOfAnimations = 4;
	int count = 0;
	public bool inBase;
	[HideInInspector] public bool isOnePlayer;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();
		rigid = GetComponent<Rigidbody> ();
		robotManager = GetComponentInParent<RSManager> ();
		agent = GetComponent<NavMeshAgent> ();
		AIData = GetComponent<AIMovementData> ();

		if (robotManager.isAI) {
			StartCoroutine (FindTarget ());
			anim.SetBool ("Forward", true);
			anim.SetFloat ("x", 0.5f);
		}

		Invoke ("SetPlayer", 0.1f);
	}

	void SetPlayer ()
	{
		isOnePlayer = GetComponentInParent<RSController> ().isPlayerOne;
		//isOnePlayer = !isOnePlayer;
		GetComponent<RSAttack> ().isPlayerOne = isOnePlayer;
		GetComponentInParent<FlyCamera> ().isPlayerOne = isOnePlayer;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (canMove) {
			if (!robotManager.isAI) {
				UpdateMovement ();
				AITarget = TimeMachine.blueTimeMachine.targetPosition;
			} else {
				AIControl ();
			}

			//clamp y position
			float currentY = transform.position.y;
			if (currentY > -0.5f) {
				transform.position = new Vector3 (transform.position.x, Mathf.Clamp (currentY, -3.5f, -0.5f), transform.position.z);
			}
		}
	}

	public void Die ()
	{
		//player has just died an must turn into a laborer
		canMove = false;
		anim.SetTrigger ("Die");

		//disable contraints on rigid body
		rigid.constraints = RigidbodyConstraints.None;
	}

	void UpdateMovement ()
	{
		float x = 0;
		float y = 0;
		if (!robotManager.isAI) {
			if (isOnePlayer) {
				y = Input.GetAxis ("Vertical2");
				x = Input.GetAxis ("Horizontal2");
			} else {
				y = Input.GetAxis ("Vertical");
				x = Input.GetAxis ("Horizontal");
			}
                
		}
            

		//update animator
		//will only animate for the character controlling the robot
		//therefore will only animate if the back camera is enabled
		Animate (x, y);

		//Look rotation
		transform.Rotate (0, x * turnSpeed * Time.deltaTime, 0);
            
		//Movement
		Vector3 targetVelocity = y * -transform.forward;
		targetVelocity *= moveSpeed;

		Vector3 velocityChange = targetVelocity - rigid.velocity;
		velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;

		rigid.AddForce (velocityChange, ForceMode.VelocityChange);
            
	}

	void Animate (float x, float y)
	{
		anim.SetBool ("Forward", y > 0);
		anim.SetBool ("Backward", y < 0);
		prevX = Mathf.Lerp (prevX, x, 5 * Time.deltaTime);
		anim.SetFloat ("x", (prevX + 1) / 2);
	}

	void Update ()
	{
		UpdateTaunt ();
	}

	public void UpdateTaunt ()
	{
		int taunt;
		if (isOnePlayer) {
			taunt = Input.GetKeyDown (KeyCode.Q) ? 1 : 0;
		} else {
			taunt = Input.GetButton ("Taunt") ? 1 : 0;
		}
		if (taunt > 0) {
			anim.SetInteger ("IdleNumber", count++ % numberOfAnimations);
			anim.SetTrigger ("RandomIdle");
		}
	}

	void AIControl ()
	{
		if (AITarget == null) {
			//Debug.Log("null");
			return;
		}


		if (!robotTarget.robotMovement.inBase) {
			Vector3 goToPos = new Vector3 (AITarget.position.x,0,AITarget.position.z);
			//Debug.Log("agentpos"+agent.transform.position+"myPos"+transform.position+" target pos"+AITarget.position+"gotTopos"+goToPos);
			//Debug.Log ("dis" + TargetDistance ());
			agent.SetDestination (goToPos);
		}




		Transform currentForward = transform;
		transform.LookAt (AITarget);
		transform.Rotate (0, 180, 0);


		if (robotTarget == null) {
			return;
		}


		//if (robotTarget.robotLaborerControl.isFighter) {//if enemy Fighter
		if(robotManager.isRed != robotTarget.isRed){
			//if distance is shorter then ram is distance - ram
			if (TargetDistance () < AIData.GetRamDistance ()) {
				robotManager.robotAttack.InitiateRam ();
			}
		}
	}

	float TargetDistance ()
	{
		return Vector3.Distance (transform.position, robotTarget.robotMovement.transform.position);
	}


	void GetAIFigterTarget ()
	{
		Collider[] cols = Physics.OverlapSphere (transform.position, 20);



	}


	bool MustGoForFighter;
	public int DistanceToPrioritizeFighters;

	void GetAITarget ()
	{
		//Debug.Log ("GetAITarget");
		MustGoForFighter = false;
		if (DistanceToPrioritizeFighters == 0) {
			DistanceToPrioritizeFighters = 60;
		}

		RSManager[] robots = FindObjectsOfType<RSManager> ();

		RSManager currentTarget = null;
		foreach (RSManager robot in robots) {
			
			//Debug.Log ("robot");
			if (robot.robotLaborerControl == null) {
			//	Debug.Log ("robot.robotLaborerControl == null");
				continue;
			}
			//if the current robot is not on our team
			else if (robot.robotMovement.inBase) {
				//	Debug.Log ("robot.robotMovement.inBase");
				continue;
			} else if (robotManager.isRed == robot.isRed && !robot.robotLaborerControl.isIdleLaborer) {//same team and not gray
				//Debug.Log ("red");
				continue;
			} 

			if (robot.robotLaborerControl.isFighter) {
				//Debug.Log (" Fighter spottted" +robot.robotLaborerControl.isFighter+" "+robot.isMainPlayer + " red:  " + robot.isRed+DistanceToPrioritizeFighters);
				//Debug.Log ((Vector3.Distance (transform.position, robot.transform.position))+""+ (Vector3.Distance (transform.position, robot.transform.position) <= DistanceToPrioritizeFighters));
				if (Vector3.Distance (transform.position, robot.rigid.transform.position) <= DistanceToPrioritizeFighters) {//"WillgoFOr Fighter");
					currentTarget = robot;
					MustGoForFighter = true;
					continue;
				}
			}
	
			if (currentTarget == null) {
				//	Debug.Log (" null");
				currentTarget = robot;
				//Debug.Log ("distancenull"+Vector3.Distance (transform.position, robot.rigid.transform.position) );

			} else if (!MustGoForFighter) {
				
				//if the robot is closer than the current target
				//Debug.Log ("isMainPlayer robot"+robot.isMainPlayer+Vector3.Distance (transform.position, robot.transform.position) );
				//Debug.Log ("player Transform" + robot.transform.position.x + " " + robot.transform.position.y + " " + robot.transform.position.z);
				//Debug.Log (" currentTarget" +Vector3.Distance (transform.position, currentTarget.transform.position));


				if (Vector3.Distance (transform.position, robot.rigid.transform.position) < Vector3.Distance (transform.position, currentTarget.rigid.transform.position)) {
					//Debug.Log ("distance"+Vector3.Distance (transform.position, robot.rigid.transform.position) );
					currentTarget = robot;
				}      
			} 

		}


		robotTarget = currentTarget;
		if (robotTarget != null) {
			AITarget = robotTarget.robotMovement.transform;
		} else {
			return;
		}
            
		if (robotTarget.robotMovement.inBase) {
			robotTarget = null;
			AITarget = GameObject.Find ("Arena").transform;
		}
	}

	public IEnumerator FindTarget ()
	{
		//Debug.Log ("FindTarget");
		yield return new WaitForSeconds (2);
		//Debug.Log ("FindTarget3333");
		if (robotManager.robotLaborerControl.isFighter) {
			GetAITarget ();

			StartCoroutine (FindTarget ());
		}

	}

	public void enterBase ()
	{
		if (robotManager.isMainPlayer) {
			audioMixerScript.INSTANCE.ChangeSnapShot (2);
			SoundManager.INSTANCE.PlayStab (GetComponentInParent<AudioSource> ());
		}
		inBase = true;
	}

	public void exitBase ()
	{
		if (robotManager.isMainPlayer) {
			audioMixerScript.INSTANCE.ChangeSnapShot (1);
			SoundManager.INSTANCE.PlayStab (GetComponentInParent<AudioSource> ());
		}
		inBase = false;
	}


}
