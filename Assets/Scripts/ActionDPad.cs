using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionDPad : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, 
                            IPointerDownHandler, IPointerUpHandler
{
    public enum ActionPadDirection
    {
        Up = 1,
        UpRight = 2,
        Right = 3,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
        None = 999
    };

    [SerializeField]
    float radius = 1;

    [HideInInspector]
    bool isHeld;

    [SerializeField]
    Sprite[] directionalSprites;

    [Serializable]
    public class JoystickMoveEvent : UnityEvent<ActionPadDirection> { }
    public JoystickMoveEvent OnValueChange;

    private ActionPadDirection UpdateTouchSprite(Vector2 direction)
    {

        /*
        1.Calculates the angle of the direction vector and converts it to degrees using the
            built -in Mathf.Atan method.
        2.Normalizes the angle to a value between 0 and 360 by adding 360 when the angle is
            less than zero.
        3.Converts angle to an ActionPadDirection after checking if the angle is between a
         certain range of values. See the diagram below for the values.
        4.Updates the Image using the directionalSprites array and returns
            currentPadDirection.*/

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360;

        ActionPadDirection currentPadDirection = ActionPadDirection.None;
        if (angle <= 22.5f || angle > 337.5f)
            currentPadDirection = ActionPadDirection.Up;
        else if (angle > 22.5 && angle <= 67.5)
            currentPadDirection = ActionPadDirection.UpRight;
        else if (angle > 67.5 && angle <= 112.5)
            currentPadDirection = ActionPadDirection.Right;
        else if (angle > 112.5 && angle <= 157.5)
            currentPadDirection = ActionPadDirection.DownRight;
        else if (angle > 157.5 && angle <= 202.5)
            currentPadDirection = ActionPadDirection.Down;
        else if (angle > 202.5 && angle <= 247.5)
            currentPadDirection = ActionPadDirection.DownLeft;
        else if (angle > 247.5 && angle <= 292.5)
            currentPadDirection = ActionPadDirection.Left;
        else if (angle > 292.5 && angle <= 337.5)
            currentPadDirection = ActionPadDirection.UpLeft;

        int index = 0;
        if (currentPadDirection != ActionPadDirection.None)
            index = (int)currentPadDirection;

        GetComponent<Image>().sprite = directionalSprites[index];
        return currentPadDirection;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        /*1.OnBeginDrag converts the touch position to a local Vector2 using the
             RectTransformUtility.ScreenPointToLocalPointInRectangle method.
         2.If the magnitude of the calculated vector is less than the radius variable, then the
             vector is reset so that it’s between a value of 0 and 1.isHeld is set true, then this
            statement updates the d-pad sprite and invokes the OnValueChange event.*/

        if (!IsActive())
            return;

        RectTransform thisRect = transform as RectTransform;
        Vector2 touchDir;
        bool didConvert = 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, 
                                      eventData.enterEventCamera, out touchDir);

        if (touchDir.sqrMagnitude > radius * radius)
        {
            touchDir.Normalize();
            isHeld = true;
            ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
            OnValueChange.Invoke(currentDirection);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        /*OnEndDrag handles updates when the drag action is complete. It calls the
        OnValueChange event with a value of ActionPadDirection.None. Then it updates the
        sprite to the first in the array — the “no-direction-pressed” sprite.*/

        OnValueChange.Invoke(ActionPadDirection.None);
        GetComponent<Image>().sprite = directionalSprites[0];
    }

    public void OnDrag(PointerEventData eventData)
    {
        /*OnDrag handles drag actions that go across the screen. It checks if the isHeld
        variable is set to true, and if it is, processes the touch like in the OnBeginDrag
        method.*/

        if (isHeld)
        {
            RectTransform thisRect = transform as RectTransform;
            Vector2 touchDir;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
            (thisRect, eventData.position, eventData.enterEventCamera, out touchDir);
            touchDir.Normalize();

            ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
            OnValueChange.Invoke(currentDirection);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //for tapping rather than dragging
        /*OnPointerDown converts the touch position to the local position on the ActionDPad
        RectTransform. It then updates the d-pad sprite using UpdateTouchSprite, and then invokes the
        OnValueChange event method with the calculated ActionPadDirection.*/


        RectTransform thisRect = transform as RectTransform;
        Vector2 touchDir;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, eventData.enterEventCamera, out touchDir);
        touchDir.Normalize();

        ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
        OnValueChange.Invoke(currentDirection);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        /*OnPointerUp simply does the same thing as OnEndDrag. It returns the d-pad to its
        neutral state.*/

        OnValueChange.Invoke(ActionPadDirection.None);
        GetComponent<Image>().sprite = directionalSprites[0];
    }
}
