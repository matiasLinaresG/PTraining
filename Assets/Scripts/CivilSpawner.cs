using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CivilSpawner : MonoBehaviour
{
	[SerializeField] private Transform[] spawnPoints;
	[SerializeField] private CivilAI civilPrefab;
	[SerializeField] private float spawnInterval;
	[SerializeField] private int maxCivilNumber;
	[SerializeField] private Player player;

	private List<CivilAI> spawnedCivil = new List<CivilAI>();
	private float timeSinceLastSpawn;

	private void Start()
	{
		timeSinceLastSpawn = spawnInterval;
	}

	private void Update()
	{
		timeSinceLastSpawn += Time.deltaTime;
		if(timeSinceLastSpawn > spawnInterval)
		{
			timeSinceLastSpawn = 0f;
			if(spawnedCivil.Count < maxCivilNumber)
			{
				SpawnCivil();
			}
		}
	}

	private void SpawnCivil()
	{
		CivilAI civil = Instantiate(civilPrefab, transform.position, transform.rotation);
		int spawnPointindex = spawnedCivil.Count % spawnPoints.Length;
		civil.Init(player, spawnPoints[spawnPointindex]);
		spawnedCivil.Add(civil);
	}

	
}
