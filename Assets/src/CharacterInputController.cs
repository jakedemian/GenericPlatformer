using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour {
    private CharacterPhysicsController cPhysics;

	// Use this for initialization
	void Start () {
        cPhysics = GetComponent<CharacterPhysicsController>();
	}
	
	// Update is called once per frame
	void Update () {
        cPhysics.setVelocityX(5f);
	}
}
