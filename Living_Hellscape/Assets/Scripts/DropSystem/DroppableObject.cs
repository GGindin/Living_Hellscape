using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class DroppableObject : ScriptableObject
{
    [SerializeField]
    protected int dropProbability;

    [SerializeField]
    protected bool isUnique;

    [SerializeField]
    protected bool alwaysDrop;

    [SerializeField]
    protected bool isEnabled;

    public abstract bool IsNull { get; }

    public virtual int DropProbablity { get => dropProbability; set => dropProbability = value; }
    public virtual bool IsUnique { get => isUnique; set => isUnique = value; }
    public virtual bool AlwaysDrops { get => alwaysDrop; set => alwaysDrop = value; }
    public virtual bool IsEnabled { get => isEnabled; set => isEnabled = value; }

    public event EventHandler PreResultEvent;
    public event EventHandler HitEvent;
    public event EventHandler<PostResultObjectEventArgs> PostResultEvent;
    public virtual void OnPreResult(EventArgs e)
    {
        EventHandler pre = PreResultEvent;

        if (PreResultEvent != null)
        {
            pre(this, e);
        }
    }

    public virtual void OnHit(EventArgs e)
    {
        EventHandler hit = HitEvent;

        if (hit != null)
        {
            hit(this, e);
        }
    }


    public virtual void OnPostResult(PostResultObjectEventArgs e)
    {
        EventHandler<PostResultObjectEventArgs> post = PostResultEvent;

        if (post != null)
        {
            post(this, e);
        }
    }
}

public class PostResultObjectEventArgs : EventArgs
{
    List<DroppableObject> results;

    public PostResultObjectEventArgs(List<DroppableObject> results)
    {
        this.results = results;
    }
}