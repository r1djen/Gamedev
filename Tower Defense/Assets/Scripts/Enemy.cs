﻿using UnityEngine;

public class Enemy : MonoBehaviour
{    
    [SerializeField]
    Transform exit; // точка выхода уничтожение противников// снижение хп 
    [SerializeField]
    Transform[] wayPoints;
    [SerializeField]
    float navigation;
    [SerializeField]
    int health;
    [SerializeField]
    int rewardAmount;


    int target = 0;
    Transform enemy;// считываться положение противника 
    Collider2D enemyCollider;
    Animator anim;
    float navigationTime = 0; // обновление противников в пространстве
    bool isDead = false;

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }
    void Start()
    {
        enemy = GetComponent<Transform>();
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        Manager.Instance.RegisterEnemy(this);
    }

   
    void Update()
    {
        if (wayPoints != null && isDead == false)
        {
            navigationTime += Time.deltaTime; // двигаемся к следующей точке
            if (navigationTime > navigation)
            {
                if (target < wayPoints.Length)
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
                }
                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exit.position, navigationTime);
                }
                navigationTime = 0;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "MoveingPoint")
        {
            target += 1;
        }
        else if(collision.tag == "Finish")
        {
            Manager.Instance.RoundEscaped += 1;
            Manager.Instance.TotalEscaped += 1;
            Manager.Instance.UnregisterEnemy(this);
            Manager.Instance.IsWaveOver();
        }
        else if(collision.tag == "Projectile")
        {
            Projectile newP = collision.gameObject.GetComponent<Projectile>();
            EnemyHit(newP.AttackDamage);
            Destroy(collision.gameObject);
        }
    }

    public void EnemyHit(int hitPoints)
    {
        if(health - hitPoints > 0)
        {
            health -= hitPoints;
            //hurt
            Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
            anim.Play("Hurt");
        }
        else
        {
            //die
            anim.SetTrigger("didDie");
            Die();
            
        }
    }

    public void Die()
    {
        isDead = true;
        enemyCollider.enabled = false;
        Manager.Instance.TotalKilled += 1;
        Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
        Manager.Instance.addMoney(rewardAmount);
        Manager.Instance.IsWaveOver();
    }

}
