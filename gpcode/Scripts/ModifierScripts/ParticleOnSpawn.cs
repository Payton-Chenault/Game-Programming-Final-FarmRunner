using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Modifier/Particle On Object Spawn")]
public class ParticleOnSpawn : MonoBehaviour
{
    public ParticleSystem particleToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(particleToSpawn, transform.position, particleToSpawn.transform.rotation);
    }
}
