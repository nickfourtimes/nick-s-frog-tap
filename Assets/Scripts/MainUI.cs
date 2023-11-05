using UnityEngine;


public class MainUI : MonoBehaviour {

	private const string ANIM_PARAM_MENU_ONSCREEN = "bOnscreen";
	private const string ANIM_PARAM_CREDITS_ONSCREEN = "bShowCredits";

	private static readonly int BoolParamOnscreen = Animator.StringToHash(ANIM_PARAM_MENU_ONSCREEN);
	private static readonly int BoolParamShowCredits = Animator.StringToHash(ANIM_PARAM_CREDITS_ONSCREEN);


	/*********************************************************************************************** VARIABLES & PROPERTIES */
	#region VARIABLES & PROPERTIES

	[SerializeField] private CanvasGroup postSignInButtonGroup;

	[SerializeField] private Animator menuAnimComp;

	[SerializeField] private Animator creditsAnimComp;

	// state
	private Leaderboard _leaderboard;
	private bool _isMenuShowing;
	private bool _isCreditsShowing;

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
		_isMenuShowing = !_isMenuShowing;
		menuAnimComp.SetBool(BoolParamOnscreen, _isMenuShowing);
	}


	public void UiToggleCredits() {
		_isCreditsShowing = !_isCreditsShowing;
		creditsAnimComp.SetBool(BoolParamShowCredits, _isCreditsShowing);
	}

	#endregion


	/*********************************************************************************************** UNITY EVENT FUNCTIONS */
	#region UNITY EVENT FUNCTIONS

	private void Start() {
		if (_leaderboard == null) {
			_leaderboard = FindObjectOfType<Leaderboard>();
		}

		postSignInButtonGroup.interactable = _leaderboard.IsAuthenticated;
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
