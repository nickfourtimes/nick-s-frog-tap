using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;


public class Purchasing : MonoBehaviour, IDetailedStoreListener {

	private const string IAP_ID_100_TAPS = "IAP_ID_100_TAPS";
	private const string IAP_ID_500_TAPS = "IAP_ID_500_TAPS";
	private const string IAP_ID_2000_TAPS = "IAP_ID_2000_TAPS";
	private const string IAP_ID_10000_TAPS = "IAP_ID_10000_TAPS";

	private const string SAVE_STR_ACCEPTED_WARNING = "SAVE_STR_ACCEPTED_WARNING";

	private const string ANIM_PARAM_SHOW = "bShowCredits";
	private static readonly int ShowCreditsID = Animator.StringToHash(ANIM_PARAM_SHOW);


	/*********************************************************************************************** VARIABLES & PROPERTIES */
	#region VARIABLES & PROPERTIES

	public bool IsInitialised { get; private set; }

	[SerializeField]
	private Animator warningAnimComp;

	private IStoreController _controller;
	private IExtensionProvider _extensions;

	private Leaderboard _leaderboard;

	#endregion


	/*********************************************************************************************** UI METHODS */
	#region UI METHODS

	public void UiPressPurchasingButton100() {
		DoPurchase(100, IAP_ID_100_TAPS);
	}


	public void UiPressPurchasingButton500() {
		DoPurchase(500, IAP_ID_500_TAPS);
	}


	public void UiPressPurchasingButton2000() {
		DoPurchase(2000, IAP_ID_2000_TAPS);
	}


	public void UiPressPurchasingButton10000() {
		DoPurchase(10000, IAP_ID_10000_TAPS);
	}


	public void UiPressAcceptPurchaseWarning() {
		AcceptWarning(true);
	}


	public void UiPressRefusePurchaseWarning() {
		AcceptWarning(false);
	}

	#endregion


	/*********************************************************************************************** METHODS */
	#region METHODS

	private void DoPurchase(int taps, string iapID) {
		// if they haven't accepted the warning, then show the warning
		if (!PlayerPrefs.HasKey(SAVE_STR_ACCEPTED_WARNING) || PlayerPrefs.GetInt(SAVE_STR_ACCEPTED_WARNING) == 0) {
			warningAnimComp.SetBool(ShowCreditsID, true);
			return;
		}

		// if ACCEPT_WARNING, then proceed to purchase
		if (Application.isEditor) {
			_leaderboard.BoughtTaps(taps);
		} else {
			if (Utility.IsAndroid()) {
				_controller.InitiatePurchase(iapID);
			} else {
				CompletePurchaseInternal(taps);
			}
		}
	}


	private void AcceptWarning(bool accepted) {
		warningAnimComp.SetBool(ShowCreditsID, false);
		PlayerPrefs.SetInt(SAVE_STR_ACCEPTED_WARNING, accepted ? 1 : 0);
	}


	private void CompletePurchaseInternal(int numTaps) {
		_leaderboard.BoughtTaps(numTaps);
	}

	#endregion


	/*********************************************************************************************** STORE LISTENER METHODS */
	#region STORE LISTENER METHODS

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
		_controller = controller;
		_extensions = extensions;
		IsInitialised = true;
	}


	public void OnInitializeFailed(InitializationFailureReason error) {
		Debug.LogError($"Initialise failed ({error})");
	}


	public void OnInitializeFailed(InitializationFailureReason error, string message) {
		Debug.LogError($"Initialise failed ({error}): {message}");
	}


	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
		Debug.LogError($"Got purchase failed: {failureReason}.");
	}


	public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {
		throw new System.NotImplementedException();
	}


	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
		switch (purchaseEvent.purchasedProduct.definition.id) {
			case IAP_ID_100_TAPS:
				CompletePurchaseInternal(100);
				break;
			case IAP_ID_500_TAPS:
				CompletePurchaseInternal(500);
				break;
			case IAP_ID_2000_TAPS:
				CompletePurchaseInternal(2000);
				break;
			case IAP_ID_10000_TAPS:
				CompletePurchaseInternal(10000);
				break;
			default:
				Debug.LogError($"Received inconceivable purchase ID ({purchaseEvent.purchasedProduct.definition.id}).");
				break;
		}

		return PurchaseProcessingResult.Complete;
	}

	#endregion


	/*********************************************************************************************** UNITY EVENT FUNCTIONS */
	#region UNITY EVENT FUNCTIONS

	private void Start() {
		// create our IAPs
		if (Utility.IsAndroid()) {
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			builder.AddProduct(IAP_ID_100_TAPS, ProductType.Consumable);
			builder.AddProduct(IAP_ID_500_TAPS, ProductType.Consumable);
			builder.AddProduct(IAP_ID_2000_TAPS, ProductType.Consumable);
			builder.AddProduct(IAP_ID_10000_TAPS, ProductType.Consumable);
			UnityPurchasing.Initialize(this, builder);
		}

		_leaderboard = GetComponent<Leaderboard>();
	}

	#endregion
}
