using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleUnscaledTime : MonoBehaviour {

	ParticleSystem particles;

	void Start() {
		particles = GetComponent<ParticleSystem>();
		Destroy(this.gameObject, particles.main.duration);
	}

	void Update () {
		if (Time.timeScale == 0) {
            particles.Simulate(Time.unscaledDeltaTime, true, false);
        }
	}
}
