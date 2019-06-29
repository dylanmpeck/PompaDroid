using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour {

    public float minVisibleX;
    public float maxVisibleX;
    private float minValue;
    private float maxValue;
    public float cameraHalfWidth;
    public float offset;

    public Camera activeCamera;

    public Transform cameraRoot;
    public Transform leftBounds;
    public Transform rightBounds;

    //used for computing hero entrance
    public Transform introWalkStart;
    public Transform introWalkEnd;

    //used for hero exit
    public Transform exitWalkEnd;

	// Use this for initialization
	void Start () {
        activeCamera = Camera.main;

        cameraHalfWidth = 
            Mathf.Abs(activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x -
                      activeCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x) * 0.5f;
        minValue = minVisibleX + cameraHalfWidth;
        maxValue = maxVisibleX - cameraHalfWidth;

        Vector3 position;
        position = leftBounds.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth;
        leftBounds.transform.localPosition = position;

        position = rightBounds.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth;
        rightBounds.transform.localPosition = position;

        //sets the walkin 2 units to the left of camera edge
        position = introWalkStart.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth - 2.0f;
        introWalkStart.transform.localPosition = position;

        //sets the walk end 2 units to the right of camera edge
        position = introWalkEnd.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth + 2.0f;
        introWalkEnd.transform.localPosition = position;

        //sets the exit to 2 units right of camera's right edge
        position = exitWalkEnd.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth + 2.0f;
        exitWalkEnd.transform.localPosition = position;
	}
	
	public void SetXPosition(float x)
    {
        Vector3 trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x + offset, minValue, maxValue);
        cameraRoot.position = trans;
    }

    public void CalculateOffset(float actorPosition)
    {
        offset = cameraRoot.position.x - actorPosition;
        SetXPosition(actorPosition);
        StartCoroutine(EaseOffset());
    }

    private IEnumerator EaseOffset()
    {
        float t = 0.0f;
        float speed = 12.0f;

        while (offset != 0)
        {
            t += Time.deltaTime;
            offset = Mathf.Lerp(offset, 0, t / speed);
            //if (Mathf.Abs(offset) < 0.05f)
            //    offset = 0;
            yield return new WaitForFixedUpdate();
        }
    }

    public void EnableBounds(bool isEnabled)
    {
        rightBounds.GetComponent<Collider>().enabled = isEnabled;
        leftBounds.GetComponent<Collider>().enabled = isEnabled;
    }
}
