using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        public float speed;
        public float maxSpeed = 6f;

        public NavMeshAgent navMeshAgent;
        public Animator animator;
        Health health;
        Vector3 velocity;
        Vector3 localVelocity;

        void Start()
        {
            health = GetComponent<Health>();
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();

            UpdateAnimator();
        }


        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);            
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }


        private void UpdateAnimator()
        {
            velocity = navMeshAgent.velocity;
            localVelocity = transform.InverseTransformDirection(velocity);

            speed = localVelocity.z;

            animator.SetFloat("forwardSpeed", speed);
        }
    }
}
