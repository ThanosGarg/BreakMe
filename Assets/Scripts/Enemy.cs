using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Enemy : MonoBehaviour
{
    public Player player;
    public NavMeshAgent agent;
    void Start()
    {
        player= GameManger.instance.player;
        agent= GetComponent<NavMeshAgent>();
    }

void Update()
    {
        agent.SetDestination(player.transform.position);
    }
}
