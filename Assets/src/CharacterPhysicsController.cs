﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhysicsController : MonoBehaviour {
    public bool showDebugRaycasts = true;
    private bool allowUpwardCollisions = false;

    [HideInInspector]
    public CollisionData cols;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private int collisionState = CollisionStates.AIR;
    private int collisionLayer = 1 << 8;

    private const float GRAVITY_ACCELERATION = -18f;
    private const float WALL_SLIDE_ACCELERATION_DOWN = -8f;
    private const float WALL_SLIDE_DECELERATION_UP = -22f;
    private const int RAYCASTS_PER_DIRECTION = 5;
    private const float MIN_RAYCAST_LENGTH = 0.2f;
    private const float MAX_FALL_SPEED = -20f;
    private const float MAX_SLIDE_SPEED = -12f;
    private const float MAX_HORIZONTAL_SPEED_GROUND = 10f;
    private const float MAX_HORIZONTAL_SPEED_AIR = 15f;

    /// <summary>
    ///     START
    /// </summary>
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        cols = new CollisionData(RAYCASTS_PER_DIRECTION);
    }
	
	/// <summary>
    ///     UPDATE
    /// </summary>
    void Update () {
        updateCollisionData();

        // apply gravity
        if(collisionState == CollisionStates.AIR) {
            float newY = getVelocityY() + (GRAVITY_ACCELERATION * Time.deltaTime);
            setVelocityY(newY);
        } else if(collisionState == CollisionStates.WALL) {
            float newY;
            if(getVelocityY() <= 0) {
                newY = getVelocityY() + (WALL_SLIDE_ACCELERATION_DOWN * Time.deltaTime);
            } else {
                newY = getVelocityY() + (WALL_SLIDE_DECELERATION_UP * Time.deltaTime);
            }
            setVelocityY(newY);
        }

        // limit the player's fall speed
        if(collisionState == CollisionStates.AIR && getVelocityY() < MAX_FALL_SPEED) {
            setVelocityY(MAX_FALL_SPEED);
        } else if(collisionState == CollisionStates.WALL && getVelocityY() < MAX_SLIDE_SPEED) {
            setVelocityY(MAX_SLIDE_SPEED);
        }

        // limit the player's movement speed
        if(collisionState == CollisionStates.GROUND && Mathf.Abs(getVelocityX()) > MAX_HORIZONTAL_SPEED_GROUND) {
            var newVelX = getVelocityX() < 0 ? -MAX_HORIZONTAL_SPEED_GROUND : MAX_HORIZONTAL_SPEED_GROUND;
            setVelocityX(newVelX);
        } else if(collisionState == CollisionStates.AIR && Mathf.Abs(getVelocityX()) > MAX_HORIZONTAL_SPEED_AIR) {
            var newVelX = getVelocityX() < 0 ? -MAX_HORIZONTAL_SPEED_AIR : MAX_HORIZONTAL_SPEED_AIR;
            setVelocityX(newVelX);
        }
	}

    /// <summary>
    ///     Update the collision data for this frame and have the character react accordingly
    /// </summary>
    private void updateCollisionData() {
        updateCollisionRaycasts();

        if(cols.isCollisionAtDirection(cols.down, Vector2.down) && getVelocityY() <= 0f) {
            // ground
            collisionState = CollisionStates.GROUND;
            setVelocityY(0f);

            float collisionPointY = cols.getRaycastHit(cols.down, Vector2.down).point.y;
            float playerHeight = boxCollider.bounds.max.y - boxCollider.bounds.min.y;
            transform.position = new Vector2(transform.position.x, collisionPointY + (playerHeight / 2f));

            // still need to provide wall snapping, just don't set the state to WALL since we're already grounded
            if(cols.isCollisionAtDirection(cols.left, Vector2.left) && getVelocityX() <= 0f) {
                snapToLeftWall();
            } else if(cols.isCollisionAtDirection(cols.right, Vector2.right) && getVelocityX() >= 0f) {
                snapToRightWall();
            }
        } else if(cols.isCollisionAtDirection(cols.left, Vector2.left) && getVelocityX() <= 0f) {
            collisionState = CollisionStates.WALL;
            snapToLeftWall();
        } else if(cols.isCollisionAtDirection(cols.right, Vector2.right) && getVelocityX() >= 0f) {
            collisionState = CollisionStates.WALL;
            snapToRightWall();
        } else {
            collisionState = CollisionStates.AIR;
        }

        // separate, state independent case for hitting the ceiling.  if you hit the ceiling and you are moving up, reverse y direction.
        RaycastHit2D upCollision = cols.getRaycastHit(cols.up, Vector2.up);
        bool upColIsPlatform = upCollision && upCollision.collider.gameObject.GetComponent<ColliderController>().isPlatform();
        if((allowUpwardCollisions || !upColIsPlatform) && cols.isCollisionAtDirection(cols.up, Vector2.up) && getVelocityY() > 0f && (collisionState == CollisionStates.AIR || collisionState == CollisionStates.WALL)) {
            setVelocityY(-0.5f * getVelocityY());

            float collisionPointY = cols.getRaycastHit(cols.up, Vector2.up).point.y;
            float playerHeight = boxCollider.bounds.max.y - boxCollider.bounds.min.y;
            transform.position = new Vector2(transform.position.x, collisionPointY - (playerHeight / 2f));
        }
    }

    /// <summary>
    ///     Snap the box collider to the left-hand collision point.
    /// </summary>
    private void snapToLeftWall() {
        setVelocityX(0f);

        float collisionPointX = cols.getRaycastHit(cols.left, Vector2.left).point.x;
        float playerWidth = boxCollider.bounds.max.x - boxCollider.bounds.min.x;
        transform.position = new Vector2(collisionPointX + (playerWidth / 2f), transform.position.y);
    }

    /// <summary>
    ///     Snap the box collider to the right-hand collision point.
    /// </summary>
    private void snapToRightWall() {
        setVelocityX(0f);

        float collisionPointX = cols.getRaycastHit(cols.right, Vector2.right).point.x;
        float playerWidth = boxCollider.bounds.max.x - boxCollider.bounds.min.x;
        transform.position = new Vector2(collisionPointX - (playerWidth / 2f), transform.position.y);
    }

    /// <summary>
    ///     Update the character's raycasts and store them for this frame
    /// </summary>
    /// <param name="b"></param>
    private void updateCollisionRaycasts() {
        Bounds b = boxCollider.bounds;

        Vector2 bottomLeft = new Vector2(b.min.x, b.min.y);
        Vector2 topLeft = new Vector2(b.min.x, b.max.y);
        Vector2 bottomRight = new Vector2(b.max.x, b.min.y);
        Vector2 topRight = new Vector2(b.max.x, b.max.y);

        cols.left = generateRaycastsForDirection(bottomLeft, topLeft, Vector2.left);
        cols.right = generateRaycastsForDirection(bottomRight, topRight, Vector2.right);
        cols.up = generateRaycastsForDirection(topLeft, topRight, Vector2.up);
        cols.down = generateRaycastsForDirection(bottomLeft, bottomRight, Vector2.down);        
    }
    
    /// <summary>
    ///     Generate an array of raycast 2d hits between two points, projecting in a given direction.
    /// </summary>
    /// <param name="origin1">The first point</param>
    /// <param name="origin2">The last point</param>
    /// <param name="dir">The direction to shoot the raycasts</param>
    /// <returns>An array of the RaycastHit2D objects.</returns>
    private RaycastHit2D[] generateRaycastsForDirection(Vector2 origin1, Vector2 origin2, Vector2 dir) {
        RaycastHit2D[] res = new RaycastHit2D[RAYCASTS_PER_DIRECTION];
        Vector2 raycastOffset = (origin2 - origin1) / (RAYCASTS_PER_DIRECTION - 1);
        float magSpeed = Mathf.Sqrt(Mathf.Pow(getVelocityX(),2) + Mathf.Pow(getVelocityY(),2));
        float raycastLengthFactor = magSpeed / 10f;
        
        for(int i = 0; i < RAYCASTS_PER_DIRECTION; i++) {
            Vector2 origin = origin1 + (i * raycastOffset);
            res[i] = Physics2D.Raycast(origin, dir, MIN_RAYCAST_LENGTH + (MIN_RAYCAST_LENGTH * raycastLengthFactor), collisionLayer);

            if(showDebugRaycasts) {
                Color raycastClr = Color.black;
                if(res[i]) {
                    if(cols.isCollision(res[i], dir)) {
                        raycastClr = Color.green;
                    }
                }
                Debug.DrawRay(origin, dir * (MIN_RAYCAST_LENGTH + (MIN_RAYCAST_LENGTH * raycastLengthFactor)), raycastClr);
            }
        }

        return res;
    }

    public void setVelocityX(float x) {
        rb.velocity = new Vector2(x, rb.velocity.y);
    }

    public void setVelocityY(float y) {
        rb.velocity = new Vector2(rb.velocity.x, y);
    }

    public void setVelocity(float x, float y) {
        rb.velocity = new Vector2(x, y);
    }

    public float getVelocityX() {
        return rb.velocity.x;
    }

    public float getVelocityY() {
        return rb.velocity.y;
    }

    public Vector2 getVelocity() {
        return rb.velocity;
    }

    public int getPlayerCollisionState() {
        return collisionState;
    }
}
