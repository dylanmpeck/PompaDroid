using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ActorCollider : MonoBehaviour {

    public Vector3 standingColliderCenter;
    public Vector3 standingColliderSize;

    public Vector3 downColliderCenter;
    public Vector3 downColliderSize;

    private BoxCollider actorCollider;

    private void Awake()
    {
        actorCollider = GetComponent<BoxCollider>();
    }

    public void SetColliderStance(bool isStanding)
    {
        if (isStanding)
        {
            actorCollider.center = standingColliderCenter;
            actorCollider.size = standingColliderSize;
        }
        else
        {
            actorCollider.center = downColliderCenter;
            actorCollider.size = downColliderSize;
        }
    }
}
