using System.Collections.Generic;
using UnityEngine;


public class Utility : MonoBehaviour {
}


public class TypedUtility<T> {

	public static void FisherYatesShuffle(List<T> list) {
		var len = list.Count;
		T temp;
		for (var i = len - 1; i >= 1; --i) {
			var j = Random.Range(0, i);

			// swap
			temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
	}
}
