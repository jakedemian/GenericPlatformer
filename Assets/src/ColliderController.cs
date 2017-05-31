using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour {
    private BoxCollider2D bc;

    private float leftBorderX;
    private float rightBorderX;
    private float topBorderY;
    private float bottomBorderY;

    void Start() {
        bc = GetComponent<BoxCollider2D>();

        Bounds b = bc.bounds;
        leftBorderX = b.min.x;
        rightBorderX = b.max.x;
        topBorderY = b.max.y;
        bottomBorderY = b.min.y;
    }

    public bool isPlatform() {
        bool res = false;

        Bounds b = bc.bounds;
        float width = b.max.x - b.min.x;
        float height = b.max.y - b.min.y;
        res = width >= height;

        return res;
    }

    public bool isWall() {
        bool res = false;

        Bounds b = bc.bounds;
        float width = b.max.x - b.min.x;
        float height = b.max.y - b.min.y;
        res = height >= width;

        return res;
    }

    public Vector2 getCollisionNormal(Vector2 collisionPoint) {
        Vector2 res = Vector2.zero;

        // TODO need to come up with special logic for the corners
        // down should always get priority, then left/right, then up

        if(collisionPoint.x == leftBorderX) {
            res = Vector2.left;
        } else if(collisionPoint.x == rightBorderX) {
            res = Vector2.right;
        } else if(collisionPoint.y == topBorderY) {
            res = Vector2.up;
        } else if(collisionPoint.y == bottomBorderY) {
            res = Vector2.down;
        }

        return res;
    }
}
