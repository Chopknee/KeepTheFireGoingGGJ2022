using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
	public class Scene : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private HeadsUpDisplay headsUpDisplay = null;

		private ParticleSystem rainParticles = null;

		public FirePit firePit { get; internal set; }

		public Logs logs {get; internal set; }

		private GameObject pool = null;
		private Deer[] deers = null;
		private Squirrel[] squirrels = null;
		private Wolf[] wolfs = null;

		private Light playerTorch = null;

		public static Scene instance = null;

		public float gameTime = 0.0f;

		public bool bIsRaining {get; internal set;}

		private Dugan.UI.Button btnFlashlight = null;

		private Dugan.UI.Button btnUmbrella = null;

		public static bool bPaused { get { return Popups.Menu.Popup.bOpened; } }

		private float rainMapY = 0.0f;

		public AudioClip rainClip = null;
		public AudioClip rainUmbrellaClip = null;
		private AudioSource rainSource = null;

		private bool bGameOver = false;

		public bool bIgnoreGameover = false;

		private void Awake() {
			instance = this;

			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();
			camera.gameObject.AddComponent<Game.Camera>();

			GameObject hudPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/HeadsUpDisplay");
			headsUpDisplay = Instantiate(hudPrefab).AddComponent<HeadsUpDisplay>();
			headsUpDisplay.transform.position = new Vector3(0.0f, 0.0f, 1000.0f);

			Transform level = transform.Find("Level");

			playerTorch = level.Find("Player/Light").GetComponent<Light>();
			playerTorch.transform.Find("cone").gameObject.AddComponent<TorchCollider>();

			firePit = level.Find("FirePit").gameObject.AddComponent<FirePit>();

			logs = level.Find("Logs").gameObject.AddComponent<Logs>();

			rainParticles = level.Find("RainParticles").GetComponent<ParticleSystem>();
			rainSource = rainParticles.GetComponent<AudioSource>();
			rainSource.clip = rainClip;

			Transform allEyes = level.Find("AllEyes");

			for (int i = 0; i < allEyes.childCount; i++) {
				allEyes.GetChild(i).gameObject.AddComponent<Eyes>();
			}

			pool = new GameObject("Pool");
			pool.transform.SetParent(transform);
			pool.transform.localPosition = new Vector3(0, -100, 0);
			GameObject deerPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/Animals/Deer_Buck");
			deers = new Deer[5];

			for (int i = 0; i < deers.Length; i++) {
				deers[i] = Instantiate(deerPrefab).AddComponent<Deer>();
				deers[i].transform.SetParent(pool.transform);
				deers[i].transform.localPosition = Vector3.zero;
			}

			GameObject squirrelPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/Animals/Squirrel");
			squirrels = new Squirrel[15];
			for (int i = 0; i < squirrels.Length; i++) {
				squirrels[i] = Instantiate(squirrelPrefab).AddComponent<Squirrel>();
				squirrels[i].transform.SetParent(pool.transform);
				squirrels[i].transform.localPosition = Vector3.zero;
			}

			GameObject wolfPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/Animals/Wolf");
			wolfs = new Wolf[1];
			wolfs[0] = Instantiate(wolfPrefab).AddComponent<Wolf>();
			wolfs[0].transform.SetParent(pool.transform);
			wolfs[0].transform.localPosition = Vector3.zero;

			btnFlashlight = level.Find("Flashlight").gameObject.AddComponent<Dugan.UI.Button>();
			btnFlashlight.OnPointerUp += OnClickBtnFlashlight;
			btnFlashlight.gameObject.SetActive(true);

			btnUmbrella = level.Find("Umbrella").gameObject.AddComponent<Dugan.UI.Button>();
			btnUmbrella.OnPointerUp += OnClickBtnUmbrella;
			firePit.umbrella.SetActive(false);

			rainMapY = Random.Range(0.0f, 5.0f);

			//Start the fire with an initial amount of logs.
			for (int i = 0; i < 4; i++) {
				firePit.AddLog();
			}

			Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));
			lastPlayerTorchDirection = ray.direction;

			Transition.instance.OnClosed += OnTransitionClosed;
			Transition.instance.SetDirection(-1);
		}

		private void OnTransitionClosed() {
			Transition.instance.OnClosed -= OnTransitionClosed;
		}

		private Vector3 lastPlayerTorchDirection = Vector3.zero;

		private void Update() {
			if (Popups.Menu.Popup.bOpened)
				return;

			//Is raining or not.
			float chanceOfRain = (Dugan.Mathf.Simplex.Noise(gameTime / 50.0f, rainMapY) + 1) * 0.5f;
			bIsRaining = chanceOfRain > 0.5f;

			//Update the rain particles
			if (bIsRaining && !rainParticles.isPlaying) {
				rainParticles.Play();
				rainSource.Play();
			} else if (!bIsRaining && rainParticles.isPlaying) {
				rainParticles.Stop();
				rainSource.Stop();
			}
			
			//Move player torch
			Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));
			lastPlayerTorchDirection = Vector3.Lerp(lastPlayerTorchDirection, ray.direction, Time.deltaTime * 10.0f);

			playerTorch.transform.forward = lastPlayerTorchDirection;

			if (!bGameOver && !bIgnoreGameover) {
				//Check for game over
				if (firePit.health <= 0.0f) {
					bGameOver = true;
					headsUpDisplay.GameOver(false);
				}

				if (gameTime >= 300) {
					bGameOver = true;
					headsUpDisplay.GameOver(true);
				}
			}

			gameTime += Time.deltaTime;

		}

		private void OnClickBtnFlashlight(Dugan.Input.PointerTarget pointerTarget, string args) {
			//Set the flashlight as the active tool
			btnUmbrella.gameObject.SetActive(true);
			btnFlashlight.gameObject.SetActive(false);
			playerTorch.gameObject.SetActive(true);
			firePit.umbrella.SetActive(false);
			rainSource.clip = rainClip;
		}

		private void OnClickBtnUmbrella(Dugan.Input.PointerTarget pointerTarget, string args) {
			//Set the umbrella as the active tool
			btnUmbrella.gameObject.SetActive(false);
			btnFlashlight.gameObject.SetActive(true);
			playerTorch.gameObject.SetActive(false);
			firePit.umbrella.SetActive(true);
			rainSource.clip = rainUmbrellaClip;
		}
	}
}