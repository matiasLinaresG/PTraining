﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, ITakeDamage
{
    const string RUN_TRIGGER = "Run";
    const string CROUCH_TRIGGER = "Crouch";
    const string SHOOT_TRIGGER = "Shoot";

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float startingHealth;
    [SerializeField] private float minTimeUnderCover;
    [SerializeField] private float maxTimeUnderCover;
    [SerializeField] private int minShotsToTake;
    [SerializeField] private int maxShotsToTake;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float damage;
    [Range(0, 100)]
    [SerializeField] private float shootingAccuracy;

    [SerializeField] private Transform shootingPosition;
    [SerializeField] private ParticleSystem bloodSplatterFX;

    [Header("Haptic Vest Events")]
    [SerializeField] private UnityEvent hapticEvent1;
    [SerializeField] private UnityEvent hapticEvent2;

    //[Header("Scene Transition")]
    //[SerializeField] private string nextSceneName; // Nombre de la siguiente escena

    private int remainingEnemies;
    private bool isShooting;
    private int currentShotsTaken;
    private int currentMaxShotsToTake;
    private NavMeshAgent agent;
    private Player player;
    private Transform occupiedCoverSpot;
    private Animator animator;

    private float _health;
    public float health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = Mathf.Clamp(value, 0, startingHealth);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        animator.SetTrigger(RUN_TRIGGER);
        _health = startingHealth;
        // remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void Init(Player player, Transform coverSpot)
    {
        occupiedCoverSpot = coverSpot;
        this.player = player;
        GetToCover();
    }

    private void GetToCover()
    {
        agent.isStopped = false;
        agent.SetDestination(occupiedCoverSpot.position);
    }

    private void Update()
    {
        if (agent.isStopped == false && (transform.position - occupiedCoverSpot.position).sqrMagnitude <= 0.1f)
        {
            agent.isStopped = true;
            StartCoroutine(InitializeShootingCO());
        }
        if (isShooting)
        {
            RotateTowardsPlayer();
        }
    }

    private IEnumerator InitializeShootingCO()
    {
        HideBehindCover();
        yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeUnderCover, maxTimeUnderCover));
        StartShooting();
    }

    private void HideBehindCover()
    {
        animator.SetTrigger(CROUCH_TRIGGER);
    }

    private void StartShooting()
    {
        isShooting = true;
        currentMaxShotsToTake = UnityEngine.Random.Range(minShotsToTake, maxShotsToTake);
        currentShotsTaken = 0;
        animator.SetTrigger(SHOOT_TRIGGER);

        //  PlayEnemyShootSound();
    }

    public void Shoot()
    {
        bool hitPlayer = UnityEngine.Random.Range(0, 100) < shootingAccuracy;

        if (hitPlayer)
        {

            RaycastHit hit;
            Vector3 direction = player.GetHeadPosition() - shootingPosition.position;
            if (Physics.Raycast(shootingPosition.position, direction, out hit))
            {
                Debug.DrawRay(shootingPosition.position, direction, Color.green, 2.0f);
                Player player = hit.collider.GetComponentInParent<Player>();

                if (player)
                {

                    player.TakeDamage(damage);

                    // Ejecutar uno de los eventos al azar
                    if (Random.Range(0, 1) == 0)
                    {
                        hapticEvent1.Invoke();
                    }
                    else
                    {
                        hapticEvent2.Invoke();
                    }
                }
                else
                {
                    Debug.LogWarning("Ray hit something, but it's not the player.");
                }
            }
            else
            {
                Debug.DrawRay(shootingPosition.position, direction, Color.red, 2.0f);
                Debug.LogWarning("Ray did not hit anything.");
            }
        }

        currentShotsTaken++;
        if (currentShotsTaken >= currentMaxShotsToTake)
        {
            StartCoroutine(InitializeShootingCO());
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = player.GetHeadPosition() - transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }

    public void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint)
    {
        health -= weapon.GetDamage();
        if (health <= 0)
        {
            Destroy(gameObject);
            //remainingEnemies--;
            //if (remainingEnemies <= 0)
            //{
            //    Invoke("LoadNextScene", 30f);
            //}
        }
        ParticleSystem effect = Instantiate(bloodSplatterFX, contactPoint, Quaternion.LookRotation(weapon.transform.position - contactPoint));
        effect.Stop();
        effect.Play();
    }

    /*
     private void LoadNextScene()
     {
         // Verificar si el nombre de la siguiente escena está configurado
         if (!string.IsNullOrEmpty(nextSceneName))
         {
             // Cargar la siguiente escena
             SceneManager.LoadScene(nextSceneName);
         }
         else
         {
             Debug.LogWarning("El nombre de la siguiente escena no está configurado en el inspector.");
         }
     } 


     private void PlayEnemyShootSound()
     {
         audioManager.Play("EnemyShootSound");
     }
      */
}