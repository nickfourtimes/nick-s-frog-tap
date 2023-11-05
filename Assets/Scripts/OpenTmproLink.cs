using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class OpenTmproLink : MonoBehaviour, IPointerClickHandler {

	private TextMeshProUGUI _pTextMeshPro;


	public void OnPointerClick(PointerEventData eventData) {
		var linkIndex = TMP_TextUtilities.FindIntersectingLink(_pTextMeshPro, Input.mousePosition, null);

		if (linkIndex == -1) return; // was a link clicked?

		var linkInfo = _pTextMeshPro.textInfo.linkInfo[linkIndex];

		// open the link id as a url, which is the metadata we added in the text field
		Application.OpenURL(linkInfo.GetLinkID());
	}


	private void Start() {
		_pTextMeshPro = GetComponent<TextMeshProUGUI>();
	}
}
