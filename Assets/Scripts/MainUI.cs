using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class MainUI : MonoBehaviour {

	private const string ANIM_PARAM_MENU_ONSCREEN = "bOnscreen";
	private const string ANIM_PARAM_CREDITS_ONSCREEN = "bShowCredits";

	private static readonly int BoolParamOnscreen = Animator.StringToHash(ANIM_PARAM_MENU_ONSCREEN);
	private static readonly int BoolParamShowCredits = Animator.StringToHash(ANIM_PARAM_CREDITS_ONSCREEN);


	/*********************************************************************************************** VARIABLES & PROPERTIES */
	#region VARIABLES & PROPERTIES

	[Header("Buttons & UI elements")]

	[SerializeField] private CanvasGroup postSignInButtonGroup;
	[SerializeField] private Button btnLeaderboards;
	[SerializeField] private Button btnAchievements;

	[Header("SFX")]

	[SerializeField] private float uiTapSoundVolume = 0.5f;
	[SerializeField] private List<AudioClip> uiTapSounds = new();
	private List<AudioClip> _uiTapSoundsBucket = new();

	[Header("Animators")]

	[SerializeField] private Animator menuAnimComp;
	[SerializeField] private Animator creditsAnimComp;

	// state
	private Leaderboard _leaderboard;
	private bool _isMenuShowing;
	private bool _isCreditsShowing;
	private AudioSource srcAudio;

	#endregion


	/*********************************************************************************************** CALLBACK METHODS */
	#region CALLBACK METHODS

	private void OnAuthentication() {
		postSignInButtonGroup.interactable = true;
	}


	private void OnDeauthentication() {
		postSignInButtonGroup.interactable = false;
	}

	#endregion


	/*********************************************************************************************** UI METHODS */
	#region UI METHODS

	public void UiToggleMenu() {
		PlayButtonSound();
		_isMenuShowing = !_isMenuShowing;
		menuAnimComp.SetBool(BoolParamOnscreen, _isMenuShowing);
	}


	public void UiToggleCredits() {
		PlayButtonSound();
		_isCreditsShowing = !_isCreditsShowing;
		creditsAnimComp.SetBool(BoolParamShowCredits, _isCreditsShowing);
	}

	#endregion


	/*********************************************************************************************** METHODS */
	#region METHODS

	public void PlayButtonSound() {
		if (_uiTapSoundsBucket.Count <= 0) {
			_uiTapSoundsBucket = uiTapSounds.ToList();
		}

		TypedUtility<AudioClip>.FisherYatesShuffle(_uiTapSoundsBucket);

		srcAudio.PlayOneShot(_uiTapSoundsBucket[0], uiTapSoundVolume);
		_uiTapSoundsBucket.RemoveAt(0);
	}

	#endregion


	/*********************************************************************************************** UNITY EVENT FUNCTIONS */
	#region UNITY EVENT FUNCTIONS

	private void Start() {
		if (_leaderboard == null) {
			_leaderboard = FindObjectOfType<Leaderboard>();
		}

		// make certain buttons inactive
		postSignInButtonGroup.interactable = _leaderboard.IsAuthenticated;
		if (!Utility.IsAndroid()) {
			btnLeaderboards.interactable = false;
			btnAchievements.interactable = false;
		}

		srcAudio = GetComponent<AudioSource>();
	}


	private void OnEnable() {
		if (_leaderboard == null) {
			_leaderboard = FindObjectOfType<Leaderboard>();
		}

		_leaderboard.OnAuthenticate += OnAuthentication;
		_leaderboard.OnDeauthenticate += OnDeauthentication;
	}


	private void OnDisable() {
		if (_leaderboard == null) {
			return;
		}

		_leaderboard.OnAuthenticate -= OnAuthentication;
		_leaderboard.OnDeauthenticate -= OnDeauthentication;
	}

	#endregion
}
