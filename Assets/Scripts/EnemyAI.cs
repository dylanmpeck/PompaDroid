using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour {

    public enum EnemyAction
    {
        None,
        Wait,
        Attack,
        Chase,
        Roam
    }

    public class DecisionWeight
    {
        public int weight;
        public EnemyAction action;
        public DecisionWeight(int weight, EnemyAction action)
        {
            this.weight = weight;
            this.action = action;
        }
    }

    //These variables reference the hero and enemy objects. The float variables will be used to calculate if the enemy will hit the hero if it chooses to attack.
    Enemy enemy;
    GameObject heroObj;

    public float attackReachMin;
    public float attackReachMax;
    public float personalSpace;

    public HeroDetector detector;

    List<DecisionWeight> weights;

    public EnemyAction currentAction = EnemyAction.None;

    private float decisionDuration;

    private void Start()
    {
        weights = new List<DecisionWeight>();
        enemy = GetComponent<Enemy>();
        heroObj = GameObject.FindGameObjectWithTag("Hero");
    }

    private void Chase()
    {
        Vector3 directionVector = heroObj.transform.position - transform.position;
        directionVector.z = directionVector.y = 0;
        directionVector.Normalize();

        //Flip the direction vector and multiply this with the personalSpace property, which sets the robot’s destination to a point in front of the hero
        directionVector *= -1f;
        directionVector *= personalSpace;

        //This affords the robot a more natural attack position, instead of always placing it all at the exact same point in front of the hero.
        directionVector.z += Random.Range(-0.4f, 0.4f);

        enemy.MoveToOffset(heroObj.transform.position, directionVector);
        decisionDuration = Random.Range(0.2f, 0.4f);
    }

    private void Wait()
    {
        decisionDuration = Random.Range(0.2f, 0.5f);
        enemy.Wait();
    }

    private void Attack()
    {
        enemy.FaceTarget(heroObj.transform.position);
        enemy.Attack();
        decisionDuration = Random.Range(1.0f, 1.5f);
    }

    private void Roam()
    {
        float randomDegree = Random.Range(0, 360);
        Vector2 offset = new Vector2(Mathf.Sin(randomDegree), Mathf.Cos(randomDegree));
        float distance = Random.Range(1, 3);
        offset *= distance;

        Vector3 directionVector = new Vector3(offset.x, 0, offset.y);
        enemy.MoveTo(enemy.transform.position + directionVector);

        decisionDuration = Random.Range(0.3f, 0.6f);
    }

    private void DecideWithWeights(int attack, int wait, int chase, int move)
    {
        weights.Clear();

        if (attack > 0)
            weights.Add(new DecisionWeight(attack, EnemyAction.Attack));
        if (chase > 0)
            weights.Add(new DecisionWeight(chase, EnemyAction.Chase));
        if (wait > 0)
            weights.Add(new DecisionWeight(wait, EnemyAction.Wait));
        if (move > 0)
            weights.Add(new DecisionWeight(move, EnemyAction.Roam));

        int total = attack + chase + wait + move;
        int intDecision = Random.Range(0, total - 1);

        foreach (DecisionWeight weight in weights)
        {
            intDecision -= weight.weight;
            if (intDecision <= 0)
            {
                SetDecision(weight.action);
                break;
            }
        }
    }

    private void SetDecision(EnemyAction action)
    {
        currentAction = action;
        if (action == EnemyAction.Attack)
            Attack();
        else if (action == EnemyAction.Chase)
            Chase();
        else if (action == EnemyAction.Roam)
            Roam();
        else if (action == EnemyAction.Wait)
            Wait();
    }

    private void Update()
    {
        float sqrDistance = Vector3.SqrMagnitude(
            heroObj.transform.position - transform.position);

        bool canReach = attackReachMin * attackReachMin < sqrDistance
            && sqrDistance < attackReachMax * attackReachMax;

        //makes sure robot is close enough on z axis
        bool samePlane = Mathf.Abs(heroObj.transform.position.z - transform.position.z) < 0.5f;

        if (canReach && currentAction == EnemyAction.Chase)
            SetDecision(EnemyAction.Wait);

        if (decisionDuration > 0.0f)
            decisionDuration -= Time.deltaTime;
        else
        {
            if (!detector.heroIsNearby) // hero is not close
                DecideWithWeights(0, 20, 80, 0); //decide between doing nothing and chasing
            else
            {
                if (samePlane)
                {
                    if (canReach)
                        DecideWithWeights(70, 15, 0, 15); // wants to attack
                    else
                        DecideWithWeights(0, 10, 80, 10); // wants to move into position
                }
                else
                    DecideWithWeights(0, 20, 60, 20); // wants to move into same plane position
            }
        }
    }
}
