﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct PlaneRoute
{
    public Transform startPoint;
    public Transform endPoint;
    public float distance;
}


public class AirplaneMove : MonoBehaviour
{
    [SerializeField] private PlaneRoute[] routes;

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float startTime;

    [SerializeField] private float minInterval = 20f;
    [SerializeField] private float maxInterval = 50f;

    [SerializeField] private AudioSource Soundsource;

    private float distCover = 0;
    private float fracOfDist = 0;
    private int currentRoute = -1;
    private bool hasFlight = false;
    private List<PlaneRoute> flightList;

    void Start()
    {
        for(int i = 0; i < routes.Length; i++)
        {
            routes[i].distance = Vector3.Distance(routes[i].startPoint.position, routes[i].endPoint.position);
        }

        flightList = routes.ToList();

        GenerateNewRounte();

        Soundsource.Pause();
    }

    void Update()
    {
        FlightStart();
        FlightOngoing();
        FlightFinish();
    }

    void MoveObject()
    {
        distCover = (Time.time - startTime) * moveSpeed;

        fracOfDist = distCover / flightList[currentRoute].distance;

        transform.position = Vector3.Lerp(flightList[currentRoute].startPoint.position, flightList[currentRoute].endPoint.position, fracOfDist);
    }

    void GenerateNewRounte()
    {
        hasFlight = false;
        startTime = Time.time + Random.Range(minInterval, maxInterval);
        if(flightList.Count == 0)
        {
            flightList = routes.ToList();
        }
        currentRoute = Random.Range(0, flightList.Count);

        AdjustFlightFacing();
    }

    void AdjustFlightFacing()
    {
        Vector3 direction = flightList[currentRoute].endPoint.position - flightList[currentRoute].startPoint.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void FlightStart()
    {
        if (Time.time > startTime && !hasFlight)
        {
            hasFlight = true;
            Soundsource.Play();
        }
    }

    void FlightOngoing()
    {
        if (!hasFlight) return;
        MoveObject();
    }

    void FlightFinish()
    {
        if (transform.position != flightList[currentRoute].endPoint.position) return;

        Soundsource.Pause();
        flightList.RemoveAt(currentRoute);
        GenerateNewRounte();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < routes.Length; i++)
        {
            Gizmos.DrawLine(routes[i].startPoint.position, routes[i].endPoint.position);
        }
    }
}
