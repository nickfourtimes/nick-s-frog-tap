using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Canvas))]
public class CanvasHelper : MonoBehaviour {
	private static List<CanvasHelper> helpers = new();

	public static UnityEvent OnResolutionOrOrientationChanged = new();

	private static bool screenChangeVarsInitialized = false;
	private static ScreenOrientation lastOrientation = ScreenOrientation.LandscapeLeft;
	private static Vector2 lastResolution = Vector2.zero;
	private static Rect lastSafeArea = Rect.zero;

	private Canvas _canvas;
	private RectTransform _rectTransform;
	private RectTransform _safeAreaTransform;

	private void Awake() {
		if(!helpers.Contains(this))
			helpers.Add(this);

		_canvas = GetComponent<Canvas>();
		_rectTransform = GetComponent<RectTransform>();

		_safeAreaTransform = transform.Find("SafeArea") as RectTransform;

		if(!screenChangeVarsInitialized) {
			lastOrientation = Screen.orientation;
			lastResolution.x = Screen.width;
			lastResolution.y = Screen.height;
			lastSafeArea = Screen.safeArea;

			screenChangeVarsInitialized = true;
		}

		ApplySafeArea();
	}

	private void Update() {
		if(helpers[0] != this)
			return;

		if(Application.isMobilePlatform && Screen.orientation != lastOrientation)
			OrientationChanged();

		if(Screen.safeArea != lastSafeArea)
			SafeAreaChanged();

		if(Screen.width != lastResolution.x || Screen.height != lastResolution.y)
			ResolutionChanged();
	}

	private void ApplySafeArea() {
		if(_safeAreaTransform == null)
			return;

		var safeArea = Screen.safeArea;

		var anchorMin = safeArea.position;
		var anchorMax = safeArea.position + safeArea.size;
		anchorMin.x /= _canvas.pixelRect.width;
		anchorMin.y /= _canvas.pixelRect.height;
		anchorMax.x /= _canvas.pixelRect.width;
		anchorMax.y /= _canvas.pixelRect.height;

		_safeAreaTransform.anchorMin = anchorMin;
		_safeAreaTransform.anchorMax = anchorMax;
	}

	private void OnDestroy() {
		if(helpers != null && helpers.Contains(this))
			helpers.Remove(this);
	}

	private static void OrientationChanged() {
		//Debug.Log("Orientation changed from " + lastOrientation + " to " + Screen.orientation + " at " + Time.time);

		lastOrientation = Screen.orientation;
		lastResolution.x = Screen.width;
		lastResolution.y = Screen.height;

		OnResolutionOrOrientationChanged.Invoke();
	}

	private static void ResolutionChanged() {
		//Debug.Log("Resolution changed from " + lastResolution + " to (" + Screen.width + ", " + Screen.height + ") at " + Time.time);

		lastResolution.x = Screen.width;
		lastResolution.y = Screen.height;

		OnResolutionOrOrientationChanged.Invoke();
	}

	private static void SafeAreaChanged() {
		// Debug.Log("Safe Area changed from " + lastSafeArea + " to " + Screen.safeArea.size + " at " + Time.time);

		lastSafeArea = Screen.safeArea;

		for (int i = 0; i < helpers.Count; i++) {
			helpers[i].ApplySafeArea();
		}
	}
}
