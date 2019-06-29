using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HeroDetector : MonoBehaviour {

    public bool heroIsNearby;

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Hero")
            heroIsNearby = true;
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Hero")
            heroIsNearby = false;
    }
}
