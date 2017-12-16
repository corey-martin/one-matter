using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Levels : MonoBehaviour {

    private List<GameObject> levels = new List<GameObject>();
    private int levelIndex = 0;
    private List<GameObject> realities = new List<GameObject>();
    private int realityIndex = 0;

    public Transform player;
    private Respawn respawn;
    public GameObject cam;
    private Vector3 camPosition;
    private float nextCamReset = 0;
    public GameObject playIcon;
    public GameObject pauseIcon;

    // ui
    public Text levelText;
    public Text dimensionText;
    public Text realityText;
    public Text spaceToStartText;
    public GameObject nonTitleGroup;
    public GameObject credits;

    public Timer timer;

    public AudioSource freezeAudioSource;
    public bool preventFreeze = false;

    private AudioSource audioSource;
    public AudioClip[] dimensionAudio;
    private float dimensionPitch;
    private float t = 0;
    private bool pitchUp = true;

    public GameObject[] tutorials;
    private bool showTutorial = true;

    private bool paused = false;
    private bool isFrozen = false;

    void Awake() {
    	PlayerPrefs.DeleteAll();
    }

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform) {
			levels.Add(child.gameObject);
			child.gameObject.SetActive(false);
		}
		playIcon.SetActive(true);
		pauseIcon.SetActive(false);
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = dimensionAudio[realityIndex];
		respawn = player.GetComponent<Respawn>();
		camPosition = cam.transform.position;
    	if (PlayerPrefs.HasKey("LevelIndex") && PlayerPrefs.GetInt("LevelIndex") != 0) {
    		showTutorial = false;
    		levelIndex = PlayerPrefs.GetInt("LevelIndex");
    		timer.timePassed = PlayerPrefs.GetFloat("CurrentTime");
    		levels[levelIndex].SetActive(true);
    	} else {
    		levels[0].SetActive(true);
    	}
		foreach (GameObject tutorial in tutorials) {
			tutorial.SetActive(false);
		}

		GetRealities();
		PauseGame();
	}

	void StartForReal() {
		paused = false;
		Time.timeScale = 1;    	
		dimensionText.gameObject.SetActive(false);
    	nonTitleGroup.SetActive(true);
    	if (levelIndex == 0) {
			Tutorial(0);
		}
		levelText.text = "LV" + (levelIndex + 1).ToString();
    	spaceToStartText.gameObject.SetActive(false);
		if (PlayerPrefs.GetInt("beatit") == 1) {
			StartCoroutine(AnimBestTime());
		}
		timer.isCounting = true;
	}

	void PauseGame() {
		paused = true;
		Time.timeScale = 0;
    	dimensionText.text = "ONE MATTER";
    	dimensionText.gameObject.SetActive(true);
    	spaceToStartText.gameObject.SetActive(true);
    	if (PlayerPrefs.HasKey("beatit")) {
    		spaceToStartText.text = "PRESS SPACE TO CONTINUE\n PRESS ESCAPE TO QUIT\n PRESS R TO RESTART";
		} else if (PlayerPrefs.HasKey("LevelIndex")) {
    		spaceToStartText.text = "PRESS SPACE TO CONTINUE\n PRESS ESCAPE TO QUIT";
		} else {
    		spaceToStartText.text = "PRESS SPACE TO START";
		}
    	nonTitleGroup.SetActive(false);
		timer.isCounting = false;
		PlayerPrefs.SetFloat("CurrentTime", timer.timePassed);
	}

	void Tutorial(int index) {
		foreach (GameObject tutorial in tutorials) {
			tutorial.SetActive(false);
		}
		if (showTutorial) {
			tutorials[index].SetActive(true);
		}
	}

	IEnumerator AnimBestTime() {
		if (PlayerPrefs.HasKey("BestTime")) {
			timer.BestTime(false);
			yield return StartCoroutine(WaitForRealSeconds(3f));
		}
		timer.isPlaying = true;
	}

	void Update() {
		if (Input.GetKeyDown("left ctrl")) {
			FreezeTime();
		}	

		if (isFrozen) {
			if (Input.GetKeyDown("right")) {
				ChangeRealities(1);
			} else if (Input.GetKeyDown("left")) {
				ChangeRealities(-1);
			}
		}

		if (PlayerPrefs.HasKey("beatit")) {
			if (Input.GetKeyDown("r") || credits.activeSelf && Input.GetKeyDown("space")) {
				credits.SetActive(false);
				levels[levelIndex].SetActive(false);
				levelIndex = 0;
				GetRealities();
				levels[levelIndex].SetActive(true);
				player.position = Vector3.zero;
				Time.timeScale = 1;
				levelText.text = "LV" + (levelIndex + 1).ToString();
				StartCoroutine(AnimateTitle(levels[levelIndex].transform.name));
				timer.timePassed = 0;
				if (PlayerPrefs.GetInt("beatit") == 1) {
					timer.isPlaying = true;
					showTutorial = false;
				}
				preventFreeze = false;
			}
		}

		if (paused) {
			if (Input.GetKeyDown("space") || Input.GetKeyDown("left ctrl")) {
				StartForReal();
			}
		}

		if (Input.GetKeyDown("escape") && !preventFreeze) {
			if (paused) {
				Application.Quit();
			} else {
				PauseGame();
			}
		}

		// audio wow effect
		if (!preventFreeze) {
			if (pitchUp && t > 0.98f) {
				pitchUp = false;
			} 
			if (!pitchUp && t < 0.02f) {
				pitchUp = true;
			}
			t += (pitchUp) ? 0.015f : -0.015f;
			dimensionPitch = Mathf.Lerp(0.095f, 0.1f, t);
			audioSource.pitch = dimensionPitch;
		}

		// jitter camera when paused
		if (Time.timeScale == 0) {
			float random = Random.Range(-0.01f, 0.01f);
			cam.transform.position += new Vector3(random,random,0);
			if (Time.unscaledTime > nextCamReset) {
				nextCamReset = Time.unscaledTime + 1;
				cam.transform.position = camPosition;
			}
		} else {
			cam.transform.position = camPosition;
		}
 	}

	void FreezeTime() {
		bool playerIsColliding = false;
		Vector3 hitLocation = player.position + Vector3.up;
    	Collider[] hits = Physics.OverlapSphere(hitLocation, 0.25f);

    	foreach (Collider hit in hits) {
    		if (hit.transform != player.transform && hit.gameObject.tag != "Goal") {
				playerIsColliding = true;
    		}
		}

		if (playerIsColliding) {
			respawn.PlaySound(respawn.respawnSound, 0.5f);
		}

		if (!preventFreeze && !playerIsColliding) {
			freezeAudioSource.pitch = Random.Range(0.9f, 1f);
			freezeAudioSource.Play();
			if (Time.timeScale == 0) {
				Time.timeScale = 1;
				playIcon.SetActive(true);
				pauseIcon.SetActive(false);
				if (levelIndex == 1) {
					Tutorial(1);
				}
				isFrozen = false;
			} else {
				Time.timeScale = 0;
				playIcon.SetActive(false);
				pauseIcon.SetActive(true);
				if (levelIndex == 1) {
					Tutorial(2);
				}
				isFrozen = true;
			}
		}
	}

	void ChangeRealities(int direction) {
		realities[realityIndex].SetActive(false);
		if (direction == 1 && realityIndex < (realities.Count - 1) ||
			direction == -1 && realityIndex > 0) {
			realityIndex += direction;
		} else if (direction == -1 && realityIndex == 0) {
			realityIndex = realities.Count - 1;
		} else {
			realityIndex = 0;
		}
		realities[realityIndex].SetActive(true);
		audioSource.clip = dimensionAudio[realityIndex];
		audioSource.Play();
		realityText.text = (realityIndex + 1).ToString() + "/" + realities.Count.ToString();
	}

	void GetRealities() {
		realities.Clear();
		bool first = true;
		foreach (Transform child in levels[levelIndex].transform) {
			realities.Add(child.gameObject);
			child.gameObject.SetActive(first);
			first = false;
		}
		realityIndex = 0;
		audioSource.clip = dimensionAudio[realityIndex];
		audioSource.Play();
		realityText.text = (realityIndex + 1).ToString() + "/" + realities.Count.ToString();
	}
	
	public IEnumerator NextLevel(GameObject goal) {
		preventFreeze = true;
		Time.timeScale = 0;
		audioSource.pitch = 0.04f;
		yield return StartCoroutine(WaitForRealSeconds(0.5f));

		audioSource.pitch = 0.065f;
		for (int i = 0; i < 15; i++) {
			cam.transform.position += new Vector3(2,0,2);
			yield return StartCoroutine(WaitForRealSeconds(0.01f));
			cam.transform.position += new Vector3(-2,0,-2);
			yield return StartCoroutine(WaitForRealSeconds(0.01f));
		}
		cam.transform.position = camPosition;
		goal.SetActive(true);
		audioSource.pitch = 0.095f;
		pitchUp = true;

		levels[levelIndex].SetActive(false);
		if (levelIndex < (levels.Count - 1)) {
			levelIndex++;
			GetRealities();
			levels[levelIndex].SetActive(true);
			player.position = Vector3.zero;
			Time.timeScale = 1;
			preventFreeze = false;
			StartCoroutine(AnimateTitle(levels[levelIndex].transform.name));
			foreach (GameObject tutorial in tutorials) {
				tutorial.SetActive(false);
			}
			levelText.text = "LV" + (levelIndex + 1).ToString();
		} else {
			levelIndex = 0;
			credits.SetActive(true);
			nonTitleGroup.SetActive(false);

			timer.isPlaying = false;
			if (PlayerPrefs.HasKey("BestTime")) {
				if (PlayerPrefs.GetFloat("BestTime") > timer.timePassed) {
					PlayerPrefs.SetFloat("BestTime", timer.timePassed);
				}
			} else {
				PlayerPrefs.SetFloat("BestTime", timer.timePassed);
			}
			timer.BestTime();
			PlayerPrefs.SetInt("beatit", 1);
		}
		PlayerPrefs.SetInt("LevelIndex", levelIndex);
		PlayerPrefs.SetFloat("CurrentTime", timer.timePassed);
	}

	// to WaitForSeconds with timeScale at 0
	IEnumerator WaitForRealSeconds(float time) {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time) {
            yield return null;
        }
    }

    IEnumerator AnimateTitle(string title, float delay = 2f) {
    	preventFreeze = true;
    	nonTitleGroup.SetActive(false);
    	dimensionText.text = title;
    	dimensionText.gameObject.SetActive(true);
    	yield return StartCoroutine(WaitForRealSeconds(delay));
    	dimensionText.gameObject.SetActive(false);
    	nonTitleGroup.SetActive(true);
    	if (levelIndex == 0) {
			Tutorial(0);
    	} else if (levelIndex == 1) {
			Tutorial(1);
		}
		preventFreeze = false;
    }
}
