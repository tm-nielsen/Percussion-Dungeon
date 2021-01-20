﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthBarDisplay : HealthDisplay
{
    public SpriteRenderer backFill;
    public int backFillHoldFrames = 0;
    public bool canDrain = false;

    private SpriteRenderer fill;
    private HealthBarDisplay next;
    private SegmentedHealthBarDisplay segBar;

    private float maxWidth, rat = 1, backRat = 1;
    private int timer = 0, index;
    // Start is called before the first frame update
    void Start()
    {
        fill = GetComponent<SpriteRenderer>();
        fill.drawMode = SpriteDrawMode.Tiled;
        maxWidth = fill.size.x;

        backFill.drawMode = SpriteDrawMode.Tiled;
    }

    private void FixedUpdate()
    {
        if (backRat > rat)
        {
            if (canDrain)
            {
                if (timer > backFillHoldFrames)
                {
                    backRat -= 0.05f;
                    backFill.size = new Vector2(backRat * maxWidth, backFill.size.y);
                }

                if (backRat <= rat)
                {
                    backRat = rat;
                    backFill.size = new Vector2(rat * maxWidth, fill.size.y);
                    timer = 0;
                    if (rat == 0 && segBar)
                    {
                        segBar.NotifyDrain(index);
                        canDrain = false;
                    }
                }
            }
            timer++;
        }
    }

    public override void UpdateDisplay(float ratio)
    {
        rat = ratio;
        if (rat > backRat)
        {
            backRat = rat;
            backFill.size = new Vector2(backRat * maxWidth, backFill.size.y);
            timer = 0;

            segBar.NotifyFill(index);
        }
        fill.size = new Vector2(ratio * maxWidth, fill.size.y);
    }

    public void Initialize(SegmentedHealthBarDisplay seg,  int n) { segBar = seg; index = n; }
}