﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanController : MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField] AI.DecisionTreeGraph graph;

    [Space(10)]
    [SerializeField] float sadSpeed;
    [SerializeField] float normalSpeed, happySpeed, prayLength, yellLength, idleLength, chanceThreshold, movingThreshold;

    AI.DecisionTreeBrain treeBrain;
    NavMeshAgent agent;
    Animator animator;

    private float mood;
    private float animationTimeTracker;

    private Vector3 previousPos;

    private void Start()
    {
        treeBrain = new AI.DecisionTreeBrain(this, graph);
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        RandomizeMood();
    }


    private void Update()
    {
        var action = treeBrain.Think();

        if(action != null)
        {
            action.Invoke();
        }

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animationTimeTracker -= Time.deltaTime;

        float movAmount = (transform.position - previousPos).magnitude/Time.deltaTime;
        previousPos = transform.position;

        if (movAmount > movingThreshold)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }


    }

    #region functions

    public bool InAnimation()
    {
        return animationTimeTracker > 0;
    }

    public bool HasPath()
    {
        return agent.hasPath;
    }

    public bool RandomChoice()
    {
        return Random.value > 0.5f;
    }

    public bool RandomLowPercentChoice()
    {
        return Random.value < chanceThreshold;
    }

    #endregion


    #region actions

    public void RandomizeMood()
    {
        SetMood((Mood)Random.Range(0, 3));
    }

    public void RandomizeDestination()
    {
        agent.destination = new Vector3(Random.value * 100 -50, 0, Random.value * 100 -50);
    }

    public void Pray()
    {
        animationTimeTracker = prayLength;
        animator.SetTrigger("Pray");
        agent.destination = transform.position;
    }

    public void Yell()
    {
        animationTimeTracker = yellLength;
        animator.SetTrigger("Yell");
        agent.destination = transform.position;
    }

    public void Idle()
    {
        animationTimeTracker = yellLength;
        agent.destination = transform.position;
    }

    #endregion


    private void SetMood(Mood m)
    {
        switch (m)
        {
            case Mood.Sad:
                agent.speed = sadSpeed;
                mood = 0;
                UpdateAnimatorMood();
                break;

            case Mood.Neutral:
                agent.speed = normalSpeed;
                mood = 0.5f;
                UpdateAnimatorMood();
                break;
            case Mood.Happy:
                agent.speed = happySpeed;
                mood = 1;
                UpdateAnimatorMood();
                break;
        }
    }

    private void UpdateAnimatorMood()
    {
        animator.SetFloat("Mood", mood);
    }

    private enum Mood
    {
        Sad,
        Neutral,
        Happy
    }


    private void OnGUI()
    {
        if (!debug)
        {
            return;
        }

        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
        pos.y = Screen.height - pos.y;
        GUI.color = Color.black;

        GUI.Label(new Rect(pos.x, pos.y, 200, 30), "In Animation: " + InAnimation());
        GUI.Label(new Rect(pos.x, pos.y+30, 200, 30), "Has Path: " + HasPath());
    }
}
