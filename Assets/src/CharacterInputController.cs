using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour {
    private CharacterPhysicsController physics;

    private const float HORIZONTAL_GROUND_SPEED = 7f;
    private const float HORIZONTAL_AIR_SPEED = 9f;
    private const float JUMP_VELOCITY = 10f;

	// Use this for initialization
	void Start () {
        physics = GetComponent<CharacterPhysicsController>();
    }

    // Update is called once per frame
    void Update () {
        float xInput = Input.GetAxisRaw("Horizontal");

        if(physics.getPlayerCollisionState() == CollisionStates.GROUND) {
            physics.setVelocityX(HORIZONTAL_GROUND_SPEED * xInput);
        } else {
            physics.setVelocityX(HORIZONTAL_AIR_SPEED * xInput);
        }

        bool jumpTrigger = Input.GetButtonDown("Jump");
        if(jumpTrigger) {
            physics.setVelocityY(JUMP_VELOCITY);
        }
    }
}
