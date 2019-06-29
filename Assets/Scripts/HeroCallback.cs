using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class is used to signal end of animations
//function calls can be found in animation window

public class HeroCallback : MonoBehaviour {

    public Hero hero;

    public void DidChain(int chain)
    {
        hero.DidChain(chain);
    }

    public void DidJumpAttack()
    {
        hero.DidJumpAttack();
    }

    public void DidPickup()
    {
        hero.DidPickupWeapon();
    }
}
