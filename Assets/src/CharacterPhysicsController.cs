using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhysicsController : MonoBehaviour {
    public bool showDebugRaycasts = true;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private const float GRAVITY_VELOCITY = -9.8f;
    private const int RAYCASTS_PER_DIRECTION = 5;
    private const float RAYCAST_LENGTH = 0.3f;
    private CollisionData cols;
    private int collisionState = CollisionStates.AIR;
    private int collisionLayer = 1 << 8;

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
        if(collisionState != CollisionStates.GROUND) {
            float newY = getVelocityY() + (GRAVITY_VELOCITY * Time.deltaTime);
            setVelocityY(newY);
        }
	}

    /// <summary>
    /// 
    /// </summary>
    private void updateCollisionData() {
        updateCollisionRaycasts();

        // process cols
        if(cols.isCollisionAtDirection(cols.down, Vector2.down) && getVelocityY() <= 0f) {
            collisionState = CollisionStates.GROUND;
            setVelocityY(0f);

            float collisionPointY = cols.getRaycastHit(cols.down).point.y;
            float playerHeight = boxCollider.bounds.max.y - boxCollider.bounds.min.y;
            transform.position = new Vector2(transform.position.x, collisionPointY + playerHeight / 2f);
        } else if(cols.isCollisionAtDirection(cols.left, Vector2.left) && getVelocityX() <= 0f) {
            // on wall left (maybe combine with below)
        } else if(cols.isCollisionAtDirection(cols.right, Vector2.right) && getVelocityX() >= 0f) {
            // on wall right (maybe combine with above
        } else {
            // in air
        }

        // separate, state independent case for hitting the ceiling.  if you hit the ceiling and you are moving up, reverse y direction.
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <param name="origin1"></param>
    /// <param name="origin2"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private RaycastHit2D[] generateRaycastsForDirection(Vector2 origin1, Vector2 origin2, Vector2 dir) {
        RaycastHit2D[] res = new RaycastHit2D[RAYCASTS_PER_DIRECTION];

        Vector2 raycastOffset = (origin2 - origin1) / (RAYCASTS_PER_DIRECTION - 1);
        
        for(int i = 0; i < RAYCASTS_PER_DIRECTION; i++) {
            Vector2 origin = origin1 + (i * raycastOffset);
            res[i] = Physics2D.Raycast(origin, dir, RAYCAST_LENGTH, collisionLayer);

            if(showDebugRaycasts) {
                Debug.DrawRay(origin, dir * RAYCAST_LENGTH, Color.green);
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

    private struct CollisionStates {
        public static int GROUND = 0;
        public static int WALL = 1;
        public static int AIR = 2;
    }
}
