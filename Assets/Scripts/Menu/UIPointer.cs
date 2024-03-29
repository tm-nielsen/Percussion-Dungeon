﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointer : MonoBehaviour
{
    public EventSystem eventSystem;

    private Animator anim;
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pos != transform.position)
        {
            pos = transform.position;
            anim.SetTrigger("flip");
        }
    }

    public void UpdatePosition(Vector2 v, Quaternion rot = default)
    {
        transform.position = new Vector2(v.x, transform.position.y);
        transform.rotation = rot;
    }
}
