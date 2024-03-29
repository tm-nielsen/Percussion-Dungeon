﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Object to follow")]
    public GameObject player;
    [Header("Fraction by which to follow in each direction")]
    public Vector2 followMod;
    [Range(0, 1f)]
    public float maxYRatio;

    public float shakeMultiplier, shakeFrequency;

    private float maxYDist;

    private Vector2 shakeP = Vector2.zero, shakeV, overridePos;

    private bool overrideFollow = false;

    private void Awake()
    {
        if (player == null)
        {
            PlayerController pcon = GameObject.FindObjectOfType<PlayerController>();
            if(pcon)
                player = pcon.gameObject;
        }

        maxYDist = GetComponent<Camera>().orthographicSize * maxYRatio;

        LoadingScreen l = GameObject.FindObjectOfType<LoadingScreen>();
        if (l)
            l.OnLoad(this);
    }

    /// <summary>
    /// sets the camera position a fraction of the distance away from centered on the player, 
    /// a fraction of velocity is used as a predictive offset fro where the camera should aim for
    /// </summary>
    void FixedUpdate()
    {
        if (player == null)
        {
            PlayerController pcon = GameObject.FindObjectOfType<PlayerController>();
            if (pcon)
                player = pcon.gameObject;
        }

        Vector2 pos;
        if (overrideFollow)
            pos = overridePos;
        else if (player)
            pos = player.transform.position;
        else
            return;

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, pos.x, followMod.x),
            Mathf.Lerp(transform.position.y, pos.y, followMod.y), -10);
        

        RunShake();

        if (transform.position.y > pos.y + maxYDist)
            transform.position = new Vector3(transform.position.x,
                pos.y + maxYDist, -10);
        if (transform.position.y < pos.y - maxYDist)
            transform.position = new Vector3(transform.position.x,
                pos.y - maxYDist, -10);
    }

    private void RunShake()
    {
        shakeP += shakeV * shakeFrequency;
        shakeV -= shakeP / 10;
        shakeV *= .8f;// / Mathf.Max(Mathf.Sqrt(shakeFrequency), 1);

        transform.position += (Vector3)shakeP;
    }

    public void Shake(Vector2 dir, float amount)
    {
        shakeV += (dir.normalized + Random.insideUnitCircle + dir.normalized * amount / 2)
            * shakeMultiplier / 200;
        shakeP += shakeV;
    }

    public void OverrideFollow(Vector2 offset, bool setPos = false)
    {
        if (setPos)
            transform.position = (Vector3)offset - Vector3.forward * 10;
        overridePos = offset;
        overrideFollow = true;
    }

    public void ResetFollowOverride()
    {
        overrideFollow = false;
    }
}
