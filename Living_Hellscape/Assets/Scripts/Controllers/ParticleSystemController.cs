using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public static ParticleSystemController Instance { get; private set; }

    [SerializeField]
    ParticleSystem hitPrefab;

    List<ParticleSystem> hitParticleSystems = new List<ParticleSystem>();

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

        hitParticleSystems.Add(sys);
    }

    void CheckSystems()
    {
        for(int i = hitParticleSystems.Count - 1; i >= 0; i--)
        {
            var sys = hitParticleSystems[i];
            if (!sys.isPlaying)
            {
                hitParticleSystems.RemoveAt(i);
                Destroy(sys.gameObject);
            }
        }
    }

}
