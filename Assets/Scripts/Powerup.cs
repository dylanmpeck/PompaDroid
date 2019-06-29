using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {

    public GameObject rootObject;
    public GameObject shadowSprite;
    public Rigidbody body;
    public int uses = 20;
    public Hero user;
    public SpriteRenderer sprite;

    public AttackData attackData1;
    public AttackData attackData2;
    public AttackData attackData3;
	
	// Update is called once per frame
	protected virtual void Update() 
    {
        Vector3 spritePos = shadowSprite.transform.position;
        spritePos.y = 0;
        shadowSprite.transform.position = spritePos;
	}

    public void Use()
    {
        uses--;
        if (uses <= 0)
        {
            user.DropWeapon();
            StartCoroutine(DestroyAnimation());
        }
    }

    protected virtual void SetOpacity(float value)
    {
        Color color = sprite.color;
        color.a = value;
        sprite.color = color;
    }

    private IEnumerator DestroyAnimation(int amount = 5)
    {
        int i = amount;
        while (i > 0)
        {
            SetOpacity(0.5f);
            yield return new WaitForSeconds(0.2f);
            SetOpacity(1.0f);
            yield return new WaitForSeconds(0.2f);
            i--;
        }
        Destroy(rootObject);
    }

    public bool CanEquip()
    {
        return (uses > 0);
    }
}
