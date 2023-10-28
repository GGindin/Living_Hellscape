using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//https://answers.unity.com/questions/214300/serializable-class-using-generics.html for genrics and subclassing method
[CreateAssetMenu(fileName = "Droppable Object Table", menuName = "DroppableObjects/Table")]
public class DroppableObjectTable : DroppableObject
{
    [SerializeField]
    protected DroppableObject[] drops;

    [SerializeField]
    protected int dropCount;

    public override bool IsNull => false;

    public int DropCount { get => dropCount; set => dropCount = value; }

    public DroppableObject[] Drops { get => drops; }


    public IEnumerable<DroppableObject> Result()
    {
        List<DroppableObject> result = new List<DroppableObject>();
        List<DroppableObject> uniqueDrops = new List<DroppableObject>();

        foreach (DroppableObject drop in drops)
        {
            drop.OnPreResult(EventArgs.Empty);
        }

        foreach (DroppableObject drop in drops)
        {
            if (drop.AlwaysDrops && drop.IsEnabled)
            {
                AddToResults(result, uniqueDrops, drop);
            }
        }

        int alwaysDropCount = drops.Count(d => d.AlwaysDrops && d.IsEnabled);

        GetRandomDrops(result, uniqueDrops, alwaysDropCount);

        PostResultObjectEventArgs postResultEventArgs = new PostResultObjectEventArgs(result as List<DroppableObject>);

        foreach (DroppableObject drop in result)
        {
            drop.OnPostResult(postResultEventArgs);
        }

        return result;
    }

    public void GetRandomDrops(List<DroppableObject> result, List<DroppableObject> uniqueDrops, int alwaysDropCount)
    {
        int realDropCount = dropCount - alwaysDropCount;

        for (int i = 0; i < realDropCount; i++)
        {
            IEnumerable<DroppableObject> possibleDrops = drops.Where(d => d.IsEnabled && !d.AlwaysDrops);

            int hitValue = UnityEngine.Random.Range(0, possibleDrops.Sum(d => d.DropProbablity));

            int runningValue = 0;

            foreach (DroppableObject drop in possibleDrops)
            {
                runningValue += drop.DropProbablity;
                if (hitValue < runningValue)
                {
                    AddToResults(result, uniqueDrops, drop);
                    break;
                }
            }
        }
    }

    protected void AddToResults(List<DroppableObject> result, List<DroppableObject> uniqueDrops, DroppableObject drop)
    {
        if (!drop.IsUnique || !uniqueDrops.Contains(drop))
        {
            if (drop.IsUnique)
            {
                uniqueDrops.Add(drop);
            }

            if (!(drop is DroppableObjectNull))
            {
                if (drop is DroppableObjectTable)
                {
                    DroppableObjectTable dropTable = drop as DroppableObjectTable;
                    result.AddRange(dropTable.Result());
                }
                else
                {
                    result.Add(drop);
                    drop.OnHit(EventArgs.Empty);
                }
            }
            else
            {
                drop.OnHit(EventArgs.Empty);
            }
        }
    }
}