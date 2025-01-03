﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour {

    [SerializeField]
    private float fallSpeed;

    [SerializeField]
    private float fallDelay;

    [SerializeField, Tooltip("How long after hitting a tunnel floor the rock will disapear")]
    private float destroyDelay;

    [SerializeField, Tooltip("How close something needs to be to the rock to be considered directly under it.")]
    private float triggerRange;

    private DigdugManager gameManager;

    internal float TriggerRange {
        get { return triggerRange; }
        private set { triggerRange = value; }
    }

    private bool isFalling = false;

    private float fallStartTime;

    private LevelManager levelManager;

    private PlayerController playerController;

    private EnemyBehaviour[] enemies;

    private List<EnemyBehaviour> squashedEnemies;

    [SerializeField]
    private float gridlineOffset;

    // Use this for initialization
    void Start () {
        squashedEnemies = new List<EnemyBehaviour>();
        gameManager = FindObjectOfType<DigdugManager>();
        enemies = FindObjectsOfType<EnemyBehaviour>();
        levelManager = FindObjectOfType<LevelManager>();
        playerController = FindObjectOfType<PlayerController>();
        Vector2 nearestGridlines = levelManager.GetNearestGridlines(this.transform.position);
        this.transform.position = new Vector3(nearestGridlines.x, nearestGridlines.y + gridlineOffset, transform.position.z);//lets snap to an X gridline so we can place rocks haphazardly without worring if they'll fall correctly (directly down a tunnel). Also adjust Y to be in the best spot (so player can dig under and leave to the left or right with ease)
    }
	
	void Update () {

        if (isFalling) {

            if (Time.time - fallStartTime > fallDelay) {
                foreach (EnemyBehaviour enemy in enemies) {
                    if (enemy) {

                        if (!enemy.currentlyFallingRocks.Contains(this)) {
                            enemy.currentlyFallingRocks.Add(this);
                        }

                        if (IsBelow(enemy.transform.position, TriggerRange / 2) && !squashedEnemies.Contains(enemy)) {//OOF
                            squashedEnemies.Add(enemy);
                            enemy.Squash();
                            enemy.transform.parent = this.transform;
                            if (gameManager.PlayerOneTurn) {
                                PlayerStats.CurrentScoreP1 += PlayerStats.PointsPerSquash;
                            }
                            else {
                                PlayerStats.CurrentScoreP2 += PlayerStats.PointsPerSquash;
                            }
                        }

                    }
                }

                bool noneBelow = true;

                foreach (Vector2 pos in levelManager.dugPositions) {
                    if (IsBelow(pos, TriggerRange)) {
                        noneBelow = false;  
                    }
                }

                if (IsBelow(playerController.transform.position, TriggerRange / 2)) {
                    playerController.StartDeath();
                    noneBelow = true;
                }

                if (!noneBelow) {
                    this.transform.Translate(0, -(fallSpeed * Time.deltaTime), 0);
                }
                else {

                    foreach (EnemyBehaviour enemy in enemies) {
                        enemy.currentlyFallingRocks.Remove(this);
                    }

                    isFalling = false;
                    Destroy(GetComponent<BoxCollider2D>());
                    Destroy(this.gameObject, destroyDelay);

                }
            }
        }
        else {

            foreach (Vector2 pos in levelManager.dugPositions) {
                if (IsBelow(pos, TriggerRange)) {
                    if (playerController.CurrentlyFacing != PlayerController.FacingDirection.Up) {
                        isFalling = true;
                        fallStartTime = Time.time;
                    }
                }
            }

        }
	}

    internal bool IsBelow(Vector2 position, float range) {
        if (position.y < transform.position.y && Mathf.Abs(position.y - transform.position.y) <= range && Mathf.Abs(position.x - transform.position.x) <= range) {
            return true;
        }
        return false;
    }
}
