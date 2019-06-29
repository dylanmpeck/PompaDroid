using System.Collections;
using UnityEngine;

public class JumpColliderItem : MonoBehaviour {

    public int isTriggeredCount = 0;

    private void OnTriggerEnter(Collider collider)
    {
        isTriggeredCount++;
    }

    private void OnTriggerExit(Collider collider)
    {
        isTriggeredCount--;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
