using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class RandomiseParticleSystem : MonoBehaviour {

    Transform player;
    ParticleSystem particles;

	// Use this for initialization
	void Start ()
    {
        particles = GetComponent<ParticleSystem>();
        transform.localRotation = Quaternion.Euler(Random.insideUnitSphere * 360);
       // player = FindObjectOfType<PlayerControl>().transform;
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 diff = ((player.position + Vector3.up * 0.5f) - particles.transform.position) * 100;

        //ParticleSystem.ForceOverLifetimeModule force = particles.forceOverLifetime;

        //ParticleSystem.MinMaxCurve x = new ParticleSystem.MinMaxCurve();
        //x.constant = diff.x;

        //ParticleSystem.MinMaxCurve y = new ParticleSystem.MinMaxCurve();
        //y.constant = diff.y;


        //force.x = x;
        //force.y = y;
    }
}
