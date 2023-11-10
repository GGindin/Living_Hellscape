using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public static ParticleSystemController Instance { get; private set; }

    [SerializeField]
    ParticleSystem hitPrefab;

    [SerializeField]
    ParticleSystem scarePrefab;

    [SerializeField]
    ParticleSystem stunPrefab;

    [SerializeField]
    ParticleSystem holdableBreakPrefab;

    List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        CheckSystems();
    }

    public void AddHit(Vector3 position)
    {
        var sys = Instantiate(hitPrefab, position, Quaternion.identity, transform);

        particleSystems.Add(sys);
    }

    public void AddScare(Vector3 position)
    {
        /*
        var sys = Instantiate(scarePrefab, position, Quaternion.identity, transform);

        particleSystems.Add(sys);
        */
    }

    public void AddStun(Vector3 position)
    {
        /*
        var sys = Instantiate(stun, position, Quaternion.identity, transform);

        particleSystems.Add(sys);
        */
    }

    public void AddHoldableBreak(Vector3 position)
    {
        var sys = Instantiate(holdableBreakPrefab, position, Quaternion.identity, transform);

        particleSystems.Add(sys);
    }

    void CheckSystems()
    {
        for(int i = particleSystems.Count - 1; i >= 0; i--)
        {
            var sys = particleSystems[i];
            if (!sys.isPlaying)
            {
                particleSystems.RemoveAt(i);
                Destroy(sys.gameObject);
            }
        }
    }

}
