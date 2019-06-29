using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {

    bool isOpen;
    public GameObject prizePrefab;
    public Transform spawnPoint;

    public Sprite leftSpriteClose;
    public Sprite rightSpriteClose;
    public Sprite leftSpriteOpen;
    public Sprite rightSpriteOpen;
    public SpriteRenderer leftSpriteRenderer;
    public SpriteRenderer rightSpriteRenderer;

    public GameObject sparkPrefab;

    public bool CanBeOpened()
    {
        return (isOpen != true);
    }

    public void Hit(Vector3 hitPoint)
    {
        GameObject sparkObj = Instantiate(sparkPrefab);
        sparkObj.transform.position = hitPoint;
    }

    public void Open(Vector3 hitPoint)
    {
        isOpen = true;
        SetSprites(leftSpriteOpen, rightSpriteOpen);
        GameObject obj = Instantiate(prizePrefab);
        obj.transform.position = spawnPoint.transform.position;
    }

    private void SetSprites(Sprite leftSprite, Sprite rightSprite)
    {
        leftSpriteRenderer.sprite = leftSprite;
        rightSpriteRenderer.sprite = rightSprite;
    }
}
