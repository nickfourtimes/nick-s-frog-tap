using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FrogMusic : MonoBehaviour {

	/*********************************************************************************************** VARIABLES & PROPERTIES */
	#region VARIABLES & PROPERTIES

	[SerializeField] private List<AudioClip> musicClips = new();
	private List<AudioClip> _musicBucket = new();

	private AudioSource _srcAudio;

	private Coroutine _crtNextSong;

	#endregion


	/*********************************************************************************************** METHODS */
	#region METHODS

	private void PlayNextSong() {
		if (_crtNextSong != null) {
			StopCoroutine(_crtNextSong);
		}

		if (_musicBucket.Count <= 0) {
			_musicBucket = musicClips.ToList();
		}

		TypedUtility<AudioClip>.FisherYatesShuffle(_musicBucket);

		var nextClip = _musicBucket[0];
		_srcAudio.clip = nextClip;
		_musicBucket.RemoveAt(0);

		_srcAudio.Play();
		_crtNextSong = StartCoroutine(I_WaitForNextSong(nextClip.length + 0.2f));
	}


	private IEnumerator I_WaitForNextSong(float len) {
		yield return new WaitForSeconds(len);
		PlayNextSong();
	}

	#endregion


	/*********************************************************************************************** UNITY EVENT FUNCTIONS */
	#region UNITY EVENT FUNCTIONS

	private void Start() {
		_srcAudio = GetComponent<AudioSource>();
		PlayNextSong();
	}


	private void Update() {
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.M)) {
			PlayNextSong();
		}
		#endif
	}

	#endregion
}
