using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;


public class Frog : MonoBehaviour {

	private const long MAX_TAPS = 999999999;

	#if UNITY_EDITOR
	private const string EDITOR_TAPS = "EDITOR_TAPS";
	#endif

	private const string SCORE_NOT_AVAILABLE = "N/A";


	/*********************************************************************************************** VARIABLES & PROPERTIES */
	#region VARIABLES & PROPERTIES

	public long Taps { get; private set; }

	[Header("UI references")]

	[SerializeField] private Button button;
	[SerializeField] private TextMeshProUGUI scoreReadout;

	[Header("Tap params")]

	[SerializeField] private float cooldownTime = 0.5f;
	[SerializeField] private List<AudioClip> squeakSounds = new();
	private List<AudioClip> _squeakBucket = new();

	// state
	private Leaderboard _leaderboard;
	private AudioSource _srcAudio;

	#endregion


	/*********************************************************************************************** UI METHODS */
	#region UI METHODS

	public void UiPressFrog() {
		_srcAudio.PlayOneShot(GetNextSqueak());
		StartCoroutine(I_Cooldown());

		if (_leaderboard.IsAuthenticated) {
			ScoreTaps(1, true);
		}
	}

	#endregion


	/*********************************************************************************************** CALLBACKS */
	#region CALLBACKS

	private void OnAuthenticate() {
		// get all leaderboard scores and see if we can find our own
		var ldrAllTime = Social.CreateLeaderboard();
		ldrAllTime.id = "leaderboardID";
		ldrAllTime.timeScope = TimeScope.AllTime;
		ldrAllTime.LoadScores((success) => {
			if (success) {
				Debug.Log($"Loaded user scores and current score is {ldrAllTime.localUserScore.value}.");
				Taps = ldrAllTime.localUserScore.value;
			}
			scoreReadout.text = Taps.ToString("N0");
		});
	}


	private void OnDeauthenticate() {
		Taps = 0;
		scoreReadout.text = SCORE_NOT_AVAILABLE;
	}

	#endregion


	/*********************************************************************************************** METHODS */
	#region METHODS

	private IEnumerator I_Cooldown() {
		button.interactable = false;
		yield return new WaitForSeconds(cooldownTime);
		button.interactable = true;
	}


	private AudioClip GetNextSqueak() {
		if (_squeakBucket.Count <= 0) {
			_squeakBucket = squeakSounds.ToList();
		}

		TypedUtility<AudioClip>.FisherYatesShuffle(_squeakBucket);

		var nextClip = _squeakBucket[0];
		_srcAudio.clip = nextClip;
		_squeakBucket.RemoveAt(0);

		return nextClip;
	}


	public void ScoreTaps(long additionalTaps, bool autoUpdateLeaderboard) {
		SetTaps(Taps + additionalTaps, autoUpdateLeaderboard);
	}


	public void SetTaps(long taps, bool autoUpdateLeaderboard) {
		// check for first cheevo
		if (taps >= 100 && Taps < 100) {
			Social.Active.ReportProgress("achievementID", 100, (success) => {
				Debug.Log("Big Big Hundo");
			});
		}

		Taps = taps;

		if (Taps >= MAX_TAPS) {
			Taps = MAX_TAPS;
			Social.Active.ReportProgress("achievementID", 100, (success) => {
				Debug.Log("Well, congratulations I guess.");
			});
		}

		// save current points
		#if UNITY_EDITOR
		PlayerPrefs.SetInt(EDITOR_TAPS, (int)Taps);
		#endif

		// check for occasional uploads to leaderboards
		if (autoUpdateLeaderboard && !Application.isEditor) {
			if (Taps % 50 == 0) {
				Social.Active.ReportScore(Taps, "achievementID", (success) => {
					// noop
				});
			}
		}

		// update the ui
		scoreReadout.text = Taps.ToString("N0");
	}

	#endregion


	/*********************************************************************************************** UNITY EVENT FUNCTIONS */
	#region UNITY EVENT FUNCTIONS

	private void Start() {
		_srcAudio = GetComponent<AudioSource>();
		if (_leaderboard == null) {
			_leaderboard = FindObjectOfType<Leaderboard>();
		}

		// get the player's current score
		if (Application.isEditor) {
			#if UNITY_EDITOR
			SetTaps(PlayerPrefs.HasKey(EDITOR_TAPS) ? (long)PlayerPrefs.GetInt(EDITOR_TAPS) : 0, false);
			#endif

		} else {
			scoreReadout.text = SCORE_NOT_AVAILABLE;
		}
	}


	private void OnEnable() {
		if (_leaderboard == null) {
			_leaderboard = FindObjectOfType<Leaderboard>();
		}

		_leaderboard.OnAuthenticate += OnAuthenticate;
		_leaderboard.OnDeauthenticate += OnDeauthenticate;
	}


	private void Update() {
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.T)) {
			SetTaps(0, false);
		}
		#endif
	}


	private void OnDisable() {
		if (_leaderboard == null) {
			return;
		}

		_leaderboard.OnAuthenticate -= OnAuthenticate;
		_leaderboard.OnDeauthenticate -= OnDeauthenticate;
	}

	#endregion
}
