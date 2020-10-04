﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown;

    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public int damage;

    private float timeSinceAttack = 0.0f;

    Vector2 intialPosition;
    Quaternion intialRotation;

    private void Awake()
    {
        intialPosition = transform.position;
        intialRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Reset Player state to initial
            transform.position = intialPosition;
            transform.rotation = intialRotation;

            timeSinceAttack = 0.0f;

            var loopers = GameObject.FindObjectsOfType<MotionLooper>();
            foreach (var looper in loopers)
            {
                looper.Loop();
            }
        }


        if (Input.GetKey(KeyCode.A) && timeSinceAttack <= 0)
        {
            timeSinceAttack = attackCooldown;

            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
            if (enemiesToDamage.Length == 0)
                return;

            // Damage enemies
            bool hasHit = false;
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].isTrigger)
                    continue;

                enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
                hasHit = true;
            }

            // Shake camera only when hit
            if (hasHit) Camera.main.GetComponent<CameraShaker>().Shake();
        }
        else
            timeSinceAttack -= Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}