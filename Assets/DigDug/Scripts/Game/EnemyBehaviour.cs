using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
    public class EnemyBehaviour: MonoBehaviour {

    #region InstanceVars
    [SerializeField]
    private EnemyType enemyType;

    [SerializeField]
    private Vector2 travelDirection;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector2 randomIdleDurationRange;

    [SerializeField]
    private float ghostSpeedMultiplier;

    [SerializeField]
    private float fleeSpeedMultiplier;

    [SerializeField, Range(.1f, 8f)]
    private float fygarAttackRange;

    private float fygarAttackFrequency = 10;

    [SerializeField]
    private float fygarAttackDuration;

    [SerializeField]
    private float deflationSpeed;

    [SerializeField]
    private Sprite squashedSprite;

    internal enum Goal { Idle, Ghost, Chase, Attack, Flee }

    internal Goal CurrentGoal { get; private set; } = Goal.Idle;

    private enum FacingDirection { Left, Right }

    private FacingDirection currentlyFacing;

    private enum EnemyType { Fygar, Pooka }

    private int inflation = 0;

    internal int Inflation {
        get { return inflation; }
        set {
            timeSinceLastDeflation = Time.time;
            if (value < 0)
                inflation = 0;
            else
            inflation = value;
        }
    }

    private readonly Vector2 upDirection = new Vector2(0, 1000);
    private readonly Vector2 downDirection = new Vector2(0, -1000);

    private readonly Vector2 leftDirection = new Vector2(1000, 0);
    private readonly Vector2 rightDirection = new Vector2(-1000, 0);

    private Vector3 fleePos;

    private Vector2 travelTarget;
    private Vector2 TravelTarget {
        get { return travelTarget; }
        set { travelTarget = value; progressingTowardsTarget = true;  }
    }

    private bool progressingTowardsTarget = false;

    private float idleTime;
    private Vector2 ghostStartingPosition;

    private float lastDirectionDistance;

    private LevelManager levelManager;
    private PlayerController playerController;

    private Vector2 nullVector = new Vector2(Mathf.Infinity, Mathf.Infinity);

    private List<Vector2> previousTravelTargets;

    private bool isSquashed = false;

    internal bool isDying { get; private set; } = false;

    internal List<RockBehaviour> currentlyFallingRocks { get; private set; }

    private float fleeStartTime;

    private float timeSinceLastDeflation;

    private float fygarLastAttackTime;

    private DigdugManager gameManager;

    public Vector3 startPosition;

    public bool Paused { get; internal set; }

    private Animator animator;

    private bool attackQueued = false;

    private bool doneAttacking;

    private float idleDuration;

    private Vector2 lastPositon;

    #endregion

    void Start () {

        animator = GetComponent<Animator>();

        gameManager = FindObjectOfType<DigdugManager>();

        idleTime = Time.time;

        idleDuration += UnityEngine.Random.Range(randomIdleDurationRange.x, randomIdleDurationRange.y);

        fleeStartTime = Time.time;

        levelManager = FindObjectOfType<LevelManager>();
        playerController = FindObjectOfType<PlayerController>();

        currentlyFallingRocks = new List<RockBehaviour>();
        previousTravelTargets = new List<Vector2>();
        previousTravelTargets.Add(nullVector);
        previousTravelTargets.Add(nullVector);

        startPosition = this.transform.position;

    }
    void Update () {

        if (Paused) {
            return;
        }

        SetFacing();

        RotateToMatchFacing();

        if (Time.time - timeSinceLastDeflation > deflationSpeed) {
            Inflation -= 1;
        }

        animator.SetInteger("Inflation", Inflation);

        if (Inflation == 4) {

            if (isDying == false) {
                if (gameManager.PlayerOneTurn) {
                    PlayerStats.CurrentScoreP1 += PlayerStats.PointsPerKill;
                }
                else {
                    PlayerStats.CurrentScoreP2 += PlayerStats.PointsPerKill;
                }

                Die(1f);
            }

        }

        if (isSquashed || Inflation > 0) {
            return;
        }


        foreach (RockBehaviour rock in currentlyFallingRocks) {
            if (rock) {
                if (rock.transform.position.y > transform.position.y && Mathf.Abs(rock.transform.position.x - transform.position.x) < rock.TriggerRange) {
                    if (Time.time - fleeStartTime > 5) {
                        fleePos = rock.transform.position;
                        CurrentGoal = Goal.Flee;
                        fleeStartTime = Time.time;
                    }
                }
            }
        }

        if (CurrentGoal == Goal.Idle) {

            animator.SetTrigger("Run");


            List<Vector2> walkablePositions = FindWalkablePositions();
            Vector2 bestPosition = FindBestIdleTarget(walkablePositions);

            if (walkablePositions.Count > 0) {

                if (!progressingTowardsTarget) {

                    if (Vector2.Distance(bestPosition, travelDirection) > lastDirectionDistance) {
                        if (Time.time - idleTime > idleDuration) {
                            int rand = Random.Range(1, 5);
                            if (rand == 1) {
                                StartGhosting(new Vector2(playerController.transform.position.x, playerController.transform.position.y));
                                fygarLastAttackTime = Time.time;
                                return;
                            }
                        }

                        lastDirectionDistance = 0;

                        travelDirection = FindNewIdleDirection();

                    }

                    lastDirectionDistance = Vector2.Distance(bestPosition, travelDirection);

                    TravelTarget = bestPosition;

                }

            }
            else {
                StartGhosting(new Vector2(playerController.transform.position.x, playerController.transform.position.y));
                return;
            }

            MoveTowardsTarget();

        }
        else if (CurrentGoal == Goal.Ghost) {

            animator.ResetTrigger("Run");
            animator.SetTrigger("Ghost");

            MoveTowardsTarget();

            if (!progressingTowardsTarget || (!IsInTunnel().Equals(nullVector) && Vector2.Distance(IsInTunnel(), ghostStartingPosition) > 1.5)) {
                TravelTarget = FindBestChaseTarget(FindWalkablePositions());
                CurrentGoal = Goal.Chase;
                return;
            }

        }
        else if (CurrentGoal == Goal.Chase) {

            animator.SetTrigger("Run");

            if (enemyType == EnemyType.Fygar) {
                if (Time.time - fygarLastAttackTime > fygarAttackFrequency) {
                    fygarLastAttackTime = Time.time;
                    fygarAttackFrequency = UnityEngine.Random.Range(5, 15);
                    CurrentGoal = Goal.Attack;
                    attackQueued = true;
                    doneAttacking = true;
                }
            }

            if (!progressingTowardsTarget) {
                TravelTarget = FindBestChaseTarget(FindWalkablePositions());
                if (TravelTarget == nullVector || Vector2.Distance(previousTravelTargets[previousTravelTargets.Count-1], playerController.transform.position) > Vector2.Distance(this.transform.position, playerController.transform.position)) {

                    StartGhosting(new Vector2(playerController.transform.position.x, playerController.transform.position.y));
                    return;
                }
            }
            else if (!new Vector2(TravelTarget.x, TravelTarget.y).Equals(nullVector)){
                MoveTowardsTarget();
            }
            else {
                StartGhosting(new Vector2(playerController.transform.position.x, playerController.transform.position.y));
                return;
            }

        }
        else if (CurrentGoal == Goal.Attack) {

            if (doneAttacking) {
                if (attackQueued) {
                    DoAttack();
                }
            }

            if (!attackQueued && doneAttacking) {
                    CurrentGoal = Goal.Chase;
                    return;
            }
        }
        else if (CurrentGoal == Goal.Flee) {

            animator.SetTrigger("Run");

            if (Time.time - fleeStartTime < 4) {

                TravelTarget = FindNewFleeTarget();

                if (TravelTarget.Equals(nullVector) || currentlyFallingRocks.Count == 0) {
                    TravelTarget = FindBestChaseTarget(FindWalkablePositions());
                    CurrentGoal = Goal.Chase;
                    return;
                }

                MoveTowardsTarget();

            }
            else {
                TravelTarget = FindBestChaseTarget(FindWalkablePositions());
                CurrentGoal = Goal.Chase;
                return;
            }
        }

	}

    private void SetFacing() {
        if (DistanceToDirection(lastPositon, leftDirection) >= DistanceToDirection(this.transform.position, leftDirection)) {
            currentlyFacing = FacingDirection.Left;
        }
        else {
            currentlyFacing = FacingDirection.Right;
        }
    }

    private void RotateToMatchFacing() {
        if (!isSquashed) {
            if (currentlyFacing == FacingDirection.Right) {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                transform.localScale = new Vector3(-8, 8, 1);
            }
            else {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                transform.localScale = new Vector3(8, 8, 1);
            }
        }
    }

    private void StopAttacking() {
        doneAttacking = true;
    }

    private void DoAttack() {
        doneAttacking = false;
        animator.SetTrigger("Attack");
        attackQueued = false;

    }


    internal void ResetBehaviour() {
        transform.position = startPosition;
        idleTime = Time.time;
        CurrentGoal = Goal.Idle;
        lastDirectionDistance = 0;
    }

    internal void Squash() {
        if (!isSquashed) {
            animator.SetTrigger("Squashed");
            isSquashed = true;
            Die(2f);
        }
    }

    private void StartGhosting(Vector2 target) {
        TravelTarget = target;
        CurrentGoal = Goal.Ghost;
        ghostStartingPosition = ClosestPosition(levelManager.dugPositions);
    }

    internal void Die(float later) {
        isDying = true;
        Destroy(this.gameObject, later);
        gameManager.OnEnemyDeath();
    }

    private Vector2 IsInTunnel() {
        Vector2 closestTunnelPoint = ClosestPosition(levelManager.dugPositions);
        if (Vector2.Distance(this.transform.position, closestTunnelPoint) < levelManager.DisconnectedThreshold/2) {
            return closestTunnelPoint;
        }
        else {
            return nullVector;
        }
    }

    private bool HasReachedTarget() {
        if (Vector2.Distance(transform.position, TravelTarget) < .05f){
            return true;
        }
        return false;
    }

    private float DistanceToDirection(Vector2 pos, Vector2 direction) {

        if (direction == upDirection || direction == downDirection) {
            return Distance(pos.y, direction.y);
        }
        else {
            return Distance(pos.x, direction.x);
        }

    }

    private Vector2 FindNewIdleDirection() {
        foreach (Vector2 potentialPos in levelManager.dugPositions) {
            if (levelManager.IsConnected(this.transform.position, potentialPos)) {

                List<Vector2> possibleDirections = new List<Vector2>();

                if (travelDirection != upDirection && Distance(potentialPos.y, upDirection.y) < Distance(transform.position.y, upDirection.y)) {
                    possibleDirections.Add(upDirection);
                }
                if (travelDirection != downDirection && Distance(potentialPos.y, downDirection.y) < Distance(transform.position.y, downDirection.y)) {
                    possibleDirections.Add(downDirection);
                }
                if (travelDirection != leftDirection && Distance(potentialPos.x, leftDirection.x) < Distance(transform.position.x, leftDirection.x)) {
                    possibleDirections.Add(leftDirection);
                }
                if (travelDirection != rightDirection && Distance(potentialPos.x, rightDirection.x) < Distance(transform.position.x, rightDirection.x)) {
                    possibleDirections.Add(rightDirection);
                }

                int randIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
                if (possibleDirections.Count >= 1) {
                    return possibleDirections[randIndex];
                }

            }
        }
        return upDirection;
    }

    private Vector2 FindNewFleeTarget() {

        foreach (Vector2 potentialPos in levelManager.dugPositions) {
            if (levelManager.IsConnected(this.transform.position, potentialPos)) {
                if (Vector3.Distance(potentialPos, fleePos) > Vector3.Distance(transform.position, fleePos)) {
                    if (ClosestPosition(levelManager.dugPositions) != potentialPos) {
                        return potentialPos;
                    }
                }

            }
        }

        return nullVector;

    }

    private Vector2 FindBestIdleTarget(List<Vector2> walkablePositions) {

        Vector2 bestPostion = nullVector;

        foreach (Vector2 pos in walkablePositions) {
            if (DistanceToDirection(pos, travelDirection) < DistanceToDirection(bestPostion, travelDirection)) {
                bestPostion = pos;
            }
        }

        return bestPostion;

    }

    private Vector2 FindBestChaseTarget(List<Vector2> walkablePositions) {

        Vector2 bestPostion = nullVector;

        foreach (Vector2 pos in walkablePositions) {
            if (Vector2.Distance(pos, playerController.transform.position) < Vector2.Distance(bestPostion, playerController.transform.position) && pos != previousTravelTargets[previousTravelTargets.Count-2]) {
                bestPostion = pos;
            }
        }

        previousTravelTargets.Add(bestPostion);
        return bestPostion;

    }

    private void MoveTowardsTarget() {

        lastPositon = transform.position;

        if (CurrentGoal == Goal.Ghost) {
            this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(TravelTarget.x, TravelTarget.y, transform.position.z), speed * ghostSpeedMultiplier * Time.deltaTime);
        }
        else if (CurrentGoal == Goal.Flee) {
            this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(TravelTarget.x, TravelTarget.y, transform.position.z), speed * fleeSpeedMultiplier * Time.deltaTime);
        }
        else {
            this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(TravelTarget.x, TravelTarget.y, transform.position.z), speed * Time.deltaTime);
        }

        if (HasReachedTarget() == true) {
            progressingTowardsTarget = false;
        }

    }

    private List<Vector2> FindWalkablePositions() {
        List<Vector2> walkable = new List<Vector2>();
        foreach (Vector2 digPos in levelManager.dugPositions) {
            if (levelManager.IsConnected(digPos, new Vector2(transform.position.x, transform.position.y))) {
                walkable.Add(digPos);
            }
        }

        Vector2 closest = new Vector2(10000, 10000);
        foreach (Vector2 pos in walkable) {
            if (Vector2.Distance(pos, transform.position) < Vector2.Distance(closest, transform.position)) {
                closest = pos;
            }
        }

        walkable.Remove(closest);

        return walkable;
    }

    private Vector2 ClosestPosition(List<Vector2> locs) {
        Vector2 closest = nullVector;
        foreach (Vector2 loc in locs) {
            if (Vector2.Distance(loc, transform.position) < Vector2.Distance(closest, this.transform.position)) {
                closest = loc;
            }
        }
        return closest;
    }

    private float Distance(float num1, float num2) {
        return Mathf.Abs(num1 - num2);
    }
}
