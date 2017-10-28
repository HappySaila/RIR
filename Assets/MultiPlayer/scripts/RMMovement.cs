using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMMovement : MonoBehaviour {

    public float turnSpeed;
    public float moveSpeed;
    float maxVelocityChange = 5;
    float prevX;

    public float x = 0;
    public float y = 0;

    Animator anim;
    public Rigidbody rigid;

    int numberOfAnimations = 4;
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 move(int team)
    {
        if(team != 0)
            UpdateMovement();
        return transform.position;
    }

    void UpdateMovement()
    {
        x = 0;
        y = 0;
        y = Input.GetAxis("Vertical2");
        x = Input.GetAxis("Horizontal2");
        prevX = x;
        //update animator
        //will only animate for the character controlling the robot
        //therefore will only animate if the back camera is enabled
        Animate(x, y);

        //Look rotation
        transform.Rotate(0, x * turnSpeed * Time.deltaTime, 0);

        //Movement
        Vector3 targetVelocity = y * -transform.forward;
        targetVelocity *= moveSpeed;

        Vector3 velocityChange = targetVelocity - rigid.velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rigid.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Animate(float x, float y)
    {
        anim.SetBool("Forward", y > 0);
        anim.SetBool("Backward", y < 0);
        prevX = Mathf.Lerp(prevX, x, 5 * Time.deltaTime);
        anim.SetFloat("x", (prevX + 1) / 2);
    }

    public void move(Vector3 networkPosition,float x, float y)
    {
        transform.position = networkPosition;
        Animate(x, y);
    }

    public void Die()
    {
        anim.SetTrigger("Die");
        //disable contraints on rigid body
        rigid.constraints = RigidbodyConstraints.None;
    }

    public void setPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void setRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
    public Quaternion getRotation()
    {
        return transform.rotation;
    }
}
