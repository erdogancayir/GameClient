using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComeBackToGame : MonoBehaviour
{
    private Vector3 playerBeginPosition;

    private void Start()
    {
        playerBeginPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = playerBeginPosition;
        }
    }
}