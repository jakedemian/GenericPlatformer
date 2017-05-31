using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionData {
    public RaycastHit2D[] left;
    public RaycastHit2D[] right;
    public RaycastHit2D[] up;
    public RaycastHit2D[] down;

    /// <summary>
    ///     The constructor for CollisionData
    /// </summary>
    /// <param name="raycasts">The number of raycasts to project, per frame, per side.</param>
    public CollisionData(int raycasts) {
        left = new RaycastHit2D[raycasts];
        right = new RaycastHit2D[raycasts];
        up = new RaycastHit2D[raycasts];
        down = new RaycastHit2D[raycasts];
    }

    /// <summary>
    ///     Checks if any of the raycasts on one side have produced a valid hit.
    /// </summary>
    /// <param name="hits">The side of raycasts to check.</param>
    /// <param name="dir">The direction these raycasts were shot.</param>
    /// <returns>True if any of the raycasts on the given side were valid collisions, false otherwise.</returns>
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

    /// <summary>
    ///     Get the first found raycast hit in an array of hits.
    /// </summary>
    /// <param name="hits">The side of raycasts to check.</param>
    /// <param name="raycastDir">The direction these raycasts were shot.</param>
    /// <returns>A populated raycast hit if one was found, an empty one otherwise.</returns>
    public RaycastHit2D getRaycastHit(RaycastHit2D[] hits, Vector2 raycastDir) {
        RaycastHit2D rh = new RaycastHit2D();

        for(int i = 0; i < hits.Length; i++) {
            if(hits[i] && isCollision(hits[i], raycastDir)) {
                rh = hits[i];
                break;
            }
        }

        return rh;
    }

    // hide this
    private CollisionData() { }
}
