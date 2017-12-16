using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

	public Levels levels;
	public GameObject explosion;
	private AudioSource audioSource;
	public AudioClip goalSound;
	public AudioClip respawnSound;

	void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Death") {
			transform.position = Vector3.zero;
			PlaySound(respawnSound, 0.5f);
		}
		if (other.gameObject.tag == "Goal") {
			PlaySound(goalSound, 1f);
			Instantiate(explosion, other.transform.position, Quaternion.identity);
			other.gameObject.SetActive(false);
			StartCoroutine(levels.NextLevel(other.gameObject));
		}
		if (other.gameObject.tag == "Spring") {
			GetComponent<Rigidbody>().AddForce(Vector3.up * 1200, ForceMode.Impulse);
			PlaySound(respawnSound, 1.5f);
		}
	}

	public void PlaySound(AudioClip soundClip, float pitch) {
		audioSource.clip = soundClip;
		audioSource.pitch = pitch;
		audioSource.Play();
	}

	void Update() {	
		if (Input.GetButtonDown("Restart")) {
			transform.position = Vector3.zero;
		}
	}
}
