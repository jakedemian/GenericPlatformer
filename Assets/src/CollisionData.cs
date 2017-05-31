using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionData {
    public RaycastHit2D[] left;
    public RaycastHit2D[] right;
    public RaycastHit2D[] up;
    public RaycastHit2D[] down;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="raycasts"></param>
    public CollisionData(int raycasts) {
        left = new RaycastHit2D[raycasts];
        right = new RaycastHit2D[raycasts];
        up = new RaycastHit2D[raycasts];
        down = new RaycastHit2D[raycasts];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hits"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public bool isCollisionAtDirection(RaycastHit2D[] hits, Vector2 dir) {
        bool res = false;
        for(int i = 0; i < hits.Length; i++) {
            if(hits[i] && isCollision(hits[i], dir)) {
                res = true;
                break;
            }
        }
        return res;
    }

    /// <summary>
    ///     Checks if there is a collision at a given raycast hit.  It only counts as a collision if the 
    ///     raycast direction and the collision normal vector are opposite.
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="raycastDir"></param>
    /// <returns></returns>
    private bool isCollision(RaycastHit2D hit, Vector2 raycastDir) {
        bool res = false;
        if(hit) {
            ColliderController cc = hit.collider.gameObject.GetComponent<ColliderController>();
            res = Vector2.Dot(raycastDir, cc.getCollisionNormal(hit.point)) == -1;
        }
        return res;
    }

    public RaycastHit2D getRaycastHit(RaycastHit2D[] hits) {
        RaycastHit2D rh = new RaycastHit2D();

        for(int i = 0; i < hits.Length; i++) {
            if(hits[i]) {
                rh = hits[i];
                break;
            }
        }

        return rh;
    }

    // hide this
    private CollisionData() { }
}
