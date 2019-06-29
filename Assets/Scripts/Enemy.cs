using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor {

    public EnemyAI ai;

    public static int TotalEnemies;
    public Walker walker;

    public bool stopMovementWhenHit = true;

    protected override void Start()
    {
        base.Start();
        lifeBar = GameObject.FindGameObjectWithTag("EnemyLifeBar").GetComponent<LifeBar>();
        lifeBar.SetProgress(currentLife);
    }

    public void RegisterEnemy()
    {
        TotalEnemies++;
    }

    protected override void Die()
    {
        base.Die();
        ai.enabled = false;
        walker.enabled = false;
        TotalEnemies--;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        walker.MoveTo(targetPosition);
    }

    //determines whether the enemy can walk to the right (positive offset) or the left (negative offset) of the targetPosition. This allows the enemy to move to a free space next to the hero.
    public void MoveToOffset(Vector3 targetPosition, Vector3 offset)
    {
        if (!walker.MoveTo(targetPosition + offset))
        {
            walker.MoveTo(targetPosition - offset);
        }
    }

    public void Wait()
    {
        walker.StopMovement();
    }

    public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        if (stopMovementWhenHit)
            walker.StopMovement();
        base.TakeDamage(value, hitVector, knockdown);
    }

    public override bool CanWalk()
    {
        return !baseAnim.GetCurrentAnimatorStateInfo(0).IsName("hurt") && 
                        !baseAnim.GetCurrentAnimatorStateInfo(0).IsName("getup");
    }
}
