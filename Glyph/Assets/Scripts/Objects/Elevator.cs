using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{
    public float speed = 2f;
    public bool goingUp = true;
    public List<Transform> elevatorWaypoints;

    private bool canUseElevator = false;
    private bool isMoving = false;
    private int currentWayPointIndex;

    private Transform playerOnElevator;

    private void Start()
    {
        transform.position = elevatorWaypoints[0].position;
        currentWayPointIndex = 0;
    }

    public bool CanInteract()
    {
        return canUseElevator && !isMoving && (playerOnElevator != null);
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        if (currentWayPointIndex == 0)
            goingUp = true;
        else if (currentWayPointIndex == elevatorWaypoints.Count - 1)
            goingUp = false;

        UseElevator();
    }

    private void UseElevator()
    {
        if (goingUp)
            currentWayPointIndex++;
        else
            currentWayPointIndex--;

        StartCoroutine(TravelElevatorToWaypoint(elevatorWaypoints[currentWayPointIndex].position));
    }

    private IEnumerator TravelElevatorToWaypoint(Vector3 targetPos)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            Vector3 moveStep = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            Vector3 delta = moveStep - transform.position;

            transform.position = moveStep;

            if (playerOnElevator != null)
                playerOnElevator.position += delta;

            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canUseElevator = true;
            playerOnElevator = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canUseElevator = false;
            if (playerOnElevator == collision.transform)
                playerOnElevator = null;
        }
    }

    public string GetDisplayName()
    {
        throw new NotImplementedException();
    }

    public bool Interact(GameObject interactor)
    {
        throw new NotImplementedException();
    }
}
