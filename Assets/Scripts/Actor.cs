using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Actor : MonoBehaviour {

    public GameObject hitSparkPrefab;

    protected bool canFlinch = true;

    public Animator baseAnim;
    public Rigidbody body;
    public SpriteRenderer shadowSprite;
    public SpriteRenderer baseSprite;

    public float speed = 2;
    protected Vector3 frontVector;

    public bool isGrounded;
    public bool isAlive = true;

    public float maxLife = 100.0f;
    public float currentLife = 100.0f;

    //holds attack info. class declaration at bottom
    public AttackData normalAttack;

    //variables for knockdown
    protected Coroutine knockdownRoutine;
    public bool isKnockedOut;

    //variables for health bar
    public LifeBar lifeBar;
    public Sprite actorThumbnail;

    public GameObject hitValuePrefab;

    public AudioClip deathClip;
    public AudioClip hitClip;

    public AudioSource audioSource;

    protected ActorCollider actorCollider;

    protected virtual void Start()
    {
        currentLife = maxLife;
        isAlive = true;
        baseAnim.SetBool("IsAlive", isAlive);

        actorCollider = GetComponent<ActorCollider>();
        actorCollider.SetColliderStance(true);
    }

    public virtual void Update()
    {
        //keeps shadow grounded
        Vector3 shadowSpritePosition = shadowSprite.transform.position;
        shadowSpritePosition.y = 0;
        shadowSprite.transform.position = shadowSpritePosition;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = true;
            baseAnim.SetBool("IsGrounded", isGrounded);
            DidLand();
        }
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = false;
            baseAnim.SetBool("IsGrounded", isGrounded);
        }
    }

    protected virtual void DidLand()
    {

    }

    public void FlipSprite(bool isFacingLeft)
    {
        if (isFacingLeft)
        {
            frontVector = new Vector3(-1, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            frontVector = new Vector3(1, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public virtual void Attack()
    {
        baseAnim.SetTrigger("Attack");
    }

    public virtual void DidHitObject(Collider collider, Vector3 hitPoint, Vector3 hitVector)
    {
        Actor actor = collider.GetComponent<Actor>();
        if (isAlive && !isKnockedOut && actor != null && actor.CanBeHit() && collider.tag != gameObject.tag)
        {
            if (collider.attachedRigidbody != null)
                HitActor(actor, hitPoint, hitVector);
        }
    }

    protected virtual void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        //Debug.Log(gameObject.name + " HIT " + actor.gameObject.name);
        actor.EvaluateAttackData(normalAttack, hitVector, hitPoint);
        PlaySFX(hitClip);
    }

    public virtual void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        FlipSprite(hitVector.x > 0);
        currentLife -= value;

        if (isAlive && currentLife <= 0)
            Die();
        else if (knockdown)
        {
            if (knockdownRoutine == null)
            {
                Vector3 pushbackVector = (hitVector + Vector3.up * 0.75f).normalized;
                body.AddForce(pushbackVector * 250);
                knockdownRoutine = StartCoroutine(KnockdownRoutine());
            }
        }
        else if (canFlinch)
            baseAnim.SetTrigger("IsHurt");

        //update lifebar
        lifeBar.EnableLifeBar(true);
        lifeBar.SetProgress(currentLife / maxLife);
        Color color = baseSprite.color;
        if (currentLife < 0)
            color.a = 0.75f;
        lifeBar.SetThumbnail(actorThumbnail, color);
    }

    protected virtual void Die()
    {
        //stop any running knockdown routines if actor is dead
        if (knockdownRoutine != null)
            StopCoroutine(knockdownRoutine);

        isAlive = false;
        baseAnim.SetBool("IsAlive", isAlive);
        StartCoroutine(DeathFlicker());
        PlaySFX(deathClip);
        actorCollider.SetColliderStance(false);
    }

    protected virtual void SetOpacity(float value)
    {
        Color color = baseSprite.color;
        color.a = value;
        baseSprite.color = color;
    }

    private IEnumerator DeathFlicker()
    {
        int i = 5;
        while (i > 0)
        {
            SetOpacity(0.5f);
            yield return new WaitForSeconds(0.1f);
            SetOpacity(1.0f);
            yield return new WaitForSeconds(0.1f);
            i--;
        }
        if (gameObject != null)
            Destroy(gameObject, 0.1f);
    }

    protected virtual IEnumerator KnockdownRoutine()
    {
        isKnockedOut = true;
        baseAnim.SetTrigger("Knockdown");
        actorCollider.SetColliderStance(false);
        yield return new WaitForSeconds(1.0f);
        actorCollider.SetColliderStance(true);
        baseAnim.SetTrigger("GetUp");
        knockdownRoutine = null;
    }

    public void DidGetUp()
    {
        isKnockedOut = false;
    }

    public bool CanBeHit()
    {
        if (isAlive && isKnockedOut == false)
            return true;
        return false;
    }

    public virtual bool CanWalk()
    {
        return true;
    }

    public virtual void FaceTarget(Vector3 targetPoint)
    {
        FlipSprite(transform.position.x - targetPoint.x > 0);
    }

    //applies the pushback force variable of the AttackData to the Rigidbody of this Actor.
    //multiplies data.force by the direction of the hit, because the actor needs to lunge backward in the opposite direction of the hero’s punch
    //Lastly, it executes TakeDamage using the AttackData’s attackDamage and hitVector values.
    public virtual void EvaluateAttackData(AttackData data, Vector3 hitVector, Vector3 hitPoint)
    {
        body.AddForce(data.force * hitVector);
        TakeDamage(data.attackDamage, hitVector, data.knockdown);
        ShowHitEffects(data.attackDamage, hitPoint);
    }

    protected void ShowHitEffects(float value, Vector3 position)
    {
        GameObject sparkObj = Instantiate(hitSparkPrefab);
        sparkObj.transform.position = position;

        //Creates a new instance of the hitValuePrefab and sets its text to the amount of
        //damage taken. After one second, it triggers the DestroyTimer script.
        GameObject obj = Instantiate(hitValuePrefab);
        obj.GetComponent<Text>().text = value.ToString();
        obj.GetComponent<DestroyTimer>().EnableTimer(1.0f);

        //Finds WorldCanvas by looking for the object tagged with the WorldCanvas tag. Then
        //it becomes the child of the damage value GameObject.Lastly, the damage value is
        //positioned in the place of the hit.
        GameObject canvas = GameObject.FindGameObjectWithTag("WorldCanvas");
        obj.transform.SetParent(canvas.transform);
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.transform.position = position;
    }

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

[System.Serializable]
public class AttackData
{
    public float attackDamage = 10;
    public float force = 50;
    public bool knockdown = false;
}