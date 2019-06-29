using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitForwarder : MonoBehaviour {

    public Actor actor;
    public Collider triggerCollider;

    void OnTriggerEnter(Collider hitCollider)
    {
        Vector3 direction = new Vector3(hitCollider.transform.position.x -
                                        actor.transform.position.x, 0, 0);
        direction.Normalize();

        BoxCollider collider = triggerCollider as BoxCollider;
        Vector3 centerPoint = this.transform.position;
        if (collider)
        {
            centerPoint = transform.TransformPoint(collider.center);
        }

        Vector3 startPoint = hitCollider.ClosestPointOnBounds(centerPoint);
        actor.DidHitObject(hitCollider, startPoint, direction);
    }
}
