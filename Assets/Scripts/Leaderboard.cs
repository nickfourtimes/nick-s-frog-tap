using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;


/// <summary>
/// Formerly would handle connections &amp; queries to the Google Play leaderboards for the game.
/// This required leaderboard IDs generated by the Google Play Games Unity plugin and stored
/// in a file named GPGSIds. That file is not included in this repository, and would need to
/// be recreated for another version of this game.
/// References to GPGSIds are still included below, and will break Android platform compilation.
/// </summary>
public class Leaderboard : MonoBehaviour {

	/*********************************************************************************************** VARIABLES & PROPERTIES */
	#region VARIABLES & PROPERTIES

	[SerializeField] private AudioClip sndChaChing;

	private ILeaderboard _ldrToday;
	private ILeaderboard _ldrWeekly;
	private ILeaderboard _ldrAllTime;

	public Action OnAuthenticate;
	public Action OnDeauthenticate;

	public bool IsAuthenticated { get; private set; }

	// misc state
	private bool _didFirstAuth;
	private Frog _frog;
	private AudioSource _srcAudio;

	#endregion


	/*********************************************************************************************** UI METHODS */
	#region UI METHODS

	public void UiPressSignIn() {
		#if UNITY_ANDROID
		if (IsAuthenticated) {
			if (!Application.isEditor) {
				// PlayGamesPlatform.Instance.SignOut();
				DeAuthenticated();
			}

		} else {
			Social.localUser.Authenticate((success, error) => {
				if (success) {
					Authenticated();
				} else {
					Debug.LogError($"GPG sign-in error: {error}");
				}
			});
		}

		#else

		// probably have some check here, but i'm going to leave it on by default.
		// i.e., on other social platforms, do some proper login handshake...
		var canAuthenticate = true;
		if (canAuthenticate) {
			Authenticated();
		}

		#endif
	}


	public void UiPressAchievements() {
		#if UNITY_ANDROID
		if (!Application.isEditor && IsAuthenticated) {
			// try to read all achievement descriptions just for testing
			Social.LoadAchievementDescriptions((descriptions) => {
				foreach (var d in descriptions) {
					Debug.Log($"Achievement: '{d.title}' (ID {d.id})");
				}
			});

			Social.ShowAchievementsUI();
		}
		#endif
	}


	public void UiPressLeaderboardButton() {
		#if UNITY_ANDROID
		if (!Application.isEditor && IsAuthenticated) {
			CheckUserScores();
			Social.ShowLeaderboardUI();
		}
		#endif
	}

	#endregion


	/*********************************************************************************************** METHODS */
	#region METHODS

	private void Initialise() {
		#if UNITY_ANDROID
		// PlayGamesPlatform.InitializeInstance(PlayGamesClientConfiguration.DefaultConfiguration);
		// PlayGamesPlatform.Activate();

		// set up references to all the leaderboards
		ldrToday = Social.CreateLeaderboard();
		ldrToday.id = GPGSIds.leaderboard_top_frog;
		ldrToday.timeScope = TimeScope.Today;
		ldrWeekly = Social.CreateLeaderboard();
		ldrWeekly.id = GPGSIds.leaderboard_top_frog;
		ldrWeekly.timeScope = TimeScope.Week;
		ldrAllTime = Social.CreateLeaderboard();
		ldrAllTime.id = GPGSIds.leaderboard_top_frog;
		ldrAllTime.timeScope = TimeScope.AllTime;
		#endif
	}


	public void BoughtTaps(int numTaps) {
		_frog.ScoreTaps(numTaps, false);
		_srcAudio.PlayOneShot(sndChaChing);

		#if UNITY_ANDROID
		Social.ReportScore(_frog.Taps, GPGSIds.leaderboard_top_frog, success => {
			if (success) {
				Debug.Log("Reported score correctly!");
			}
		});
		#endif
	}


	private void Authenticated() {
		IsAuthenticated = true;
		OnAuthenticate?.Invoke();
		Debug.Log("Successfully authenticated.");

		if (_didFirstAuth) return;

		_didFirstAuth = true;
		StartCoroutine(I_CheckAuthentication());
		CheckUserScores();
	}


	private void DeAuthenticated() {
		IsAuthenticated = false;
		OnDeauthenticate?.Invoke();
		Debug.Log("User lost authentication.");
	}


	/// <summary> Periodically check authentication status </summary>
	private IEnumerator I_CheckAuthentication() {
		#if UNITY_ANDROID
		while (true) {
			yield return new WaitForSeconds(1f);

			if (_didFirstAuth && IsAuthenticated) {
				if (!Social.Active.localUser.authenticated) {
					DeAuthenticated();
				}
			}
		}
		#endif

		yield break;
	}


	private void CheckUserScores() {
		#if UNITY_ANDROID
		_ldrToday.LoadScores((success) => {
			if (success) {
				if (_ldrToday.scores.Length > 0) {
					if (_ldrToday.localUserScore.rank == 1) {
						Debug.Log("Congrats! You are top frog (daily)!");
						Social.Active.ReportProgress(GPGSIds.achievement_frog_of_the_day, 100, (success) => {
							// noop
						});
					}
				}
			}
		});

		_ldrWeekly.LoadScores((success) => {
			if (success) {
				if (_ldrWeekly.scores.Length > 0) {
					if (_ldrWeekly.localUserScore.rank == 1) {
						Debug.Log("Congrats! You are top frog (weekly)!");
						Social.Active.ReportProgress(GPGSIds.achievement_frog_of_the_week, 100, (success) => {
							// noop
						});
					}
				}
			}
		});

		_ldrAllTime.LoadScores((success) => {
			if (success) {
				if (_ldrAllTime.scores.Length > 0) {
					Debug.Log($"Current user all-time rank is: {_ldrAllTime.localUserScore.rank}.");
					if (_ldrAllTime.localUserScore.rank == 1) {
						Debug.Log("Congrats! You are top frog (all-time)!");
						Social.Active.ReportProgress(GPGSIds.achievement_alltime_frog, 100, (success) => {
							// noop
						});
					}
				}
			}
		});
		#endif
	}

	#endregion


	/*********************************************************************************************** UNITY EVENT FUNCTIONS */
	#region UNITY EVENT FUNCTIONS

	private void Start() {
		Initialise();

		_frog = FindObjectOfType<Frog>();

		_srcAudio = GetComponent<AudioSource>();
	}

	#endregion
}
