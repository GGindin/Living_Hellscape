using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhostManager : MonoBehaviour
{
    public static EnemyGhostManager Instance { get; private set; }

    [SerializeField]
    GhostEnemy ghostEnemyPrefab;

    [SerializeField]
    float spawnDistance;

    [SerializeField]
    float spawnInterval;

    List<GhostEnemy> ghosts = new List<GhostEnemy>();

    float lastSpawnTime = float.MinValue;

    bool playerInGhostMode => PlayerManager.Instance.Active == PlayerManager.Instance.GhostInstance;

    public float SqrSpawnDist => spawnDistance * spawnDistance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartFadeIn()
    {
        //activate ghosts
        //reposition ghosts
        RepositionGhosts();
        ActiveGhostsCheck();

        for (int i = 0; i < ghosts.Count; i++)
        {
            var ghost = ghosts[i];
            ghost.StopAllCoroutines();
            StartCoroutine(ghost.ProcessFadeIn());
        }
    }

    public void StartFadeOut()
    {
        StartCoroutine(ProcessFadeOut());

        for (int i = 0; i < ghosts.Count; i++)
        {
            var ghost = ghosts[i];
            ghost.StopAllCoroutines();
            StartCoroutine(ghost.ProcessFadeOut());
        }
    }


    IEnumerator ProcessFadeOut()
    {
        float current = GhostWorldFilterController.Instance.TransitionLength;
        yield return new WaitForSeconds(current);
        DeactiveGhostsCheck();
    }
    

    private void Update()
    {
        if (GameController.Instance.StopUpdates) return;
        if (PlayerManager.Instance.Active == null) return;
        if (playerInGhostMode)
        {
            CheckForSpawn();
        }
        else if(PlayerManager.Instance.PlayerHasControl)
        {
            CheckForDestroy();
        }
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.StopUpdates) return;
        if (PlayerManager.Instance.Active == null) return;
        if (playerInGhostMode)
        {
            for (int i = 0; i < ghosts.Count; i++)
            {
                ghosts[i].RoomFixedUpdate();
            }
        }
    }

    private void DeactiveGhostsCheck()
    {
        for(int i = 0; i < ghosts.Count; i++)
        {
            var ghost = ghosts[i];
            if (ghost.gameObject.activeInHierarchy)
            {
                ghost.gameObject.SetActive(false);
            }
        }
    }

    private void ActiveGhostsCheck()
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            var ghost = ghosts[i];
            if (!ghost.gameObject.activeInHierarchy)
            {
                ghost.gameObject.SetActive(true);
            }
        }
    }

    public void RepositionGhosts()
    {
        for(int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].RepositionGhost();
        }
    }

    private void CheckForDestroy()
    {
        for(int i = ghosts.Count - 1; i >= 0; i--)
        {
            var ghost = ghosts[i];
            ghost.UpdateDistanceWhenPlayerInBody();
            if(ghost.CurrentSqrDistToPlayer > SqrSpawnDist)
            {
                Destroy(ghost.gameObject);
                ghosts.RemoveAt(i);
            }
        }
    }

    void CheckForSpawn()
    {
        var duration = Time.time - lastSpawnTime;
        if(duration > spawnInterval)
        {
            SpawnGhost();
            lastSpawnTime = Time.time;
        }
    }

    void SpawnGhost()
    {
        var dir = Vector2.up;
        dir = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * dir;

        var spawnPos = dir * spawnDistance + PlayerManager.Instance.GhostPosition;

        var ghost = Instantiate(ghostEnemyPrefab, spawnPos, Quaternion.identity, transform);

        ghosts.Add(ghost);
    }
}
