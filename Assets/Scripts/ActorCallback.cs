using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCallback : MonoBehaviour {

    public Actor actor;

    public void DidGetUp()
    {
        actor.DidGetUp();
    }
}
