﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CivilAI : MonoBehaviour, ITakeDamage
{
    const string RUN_TRIGGER = "Run";
    const string CROUCH_TRIGGER = "Crouch";

    [SerializeField] private float startingHealth;
    [SerializeField] private float minTimeUnderCover;
    [SerializeField] private float maxTimeUnderCover;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float damage;
    [Range(0, 100)]
    [SerializeField] private ParticleSystem bloodSplatterFX;

    private NavMeshAgent agent;
    private Player player;
    private Transform occupiedCoverSpot;
    private Animator animator;
    private float _health;

    // Contadores
    private static int woundedCount = 0;
    private static int deadCount = 0;

    public float health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, startingHealth); }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _health = startingHealth;
        // Otras inicializaciones si es necesario
        animator.SetTrigger(RUN_TRIGGER);
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
        RotateTowardsPlayer();

        if (agent.isStopped == false && (transform.position - occupiedCoverSpot.position).sqrMagnitude <= 0.1f)
        {
            agent.isStopped = true;
            HideBehindCover();
        }
    }

    private void HideBehindCover()
    {
        animator.SetTrigger(CROUCH_TRIGGER);
        StartCoroutine(StayUnderCover());
    }

    private IEnumerator StayUnderCover()
    {
        float timeUnderCover = Random.Range(minTimeUnderCover, maxTimeUnderCover);
        yield return new WaitForSeconds(timeUnderCover);
        // Puedes agregar lógica adicional aquí si es necesario
        GetToCover();
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
        Debug.Log("Civil Health: " + health);

        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Civil destroyed!");
            deadCount++;
        }
        else
        {
            woundedCount++;
        }

        ParticleSystem effect = Instantiate(bloodSplatterFX, contactPoint, Quaternion.LookRotation(weapon.transform.position - contactPoint));
        effect.Stop();
        effect.Play();

        // Imprimir los contadores
        Debug.Log("Civiles heridos: " + woundedCount);
        Debug.Log("Civiles muertos: " + deadCount);
    }
}
