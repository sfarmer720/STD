using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VectorCompare : IComparer<Vector3>{

	//compare vector 3s
	public int Compare(Vector3 a, Vector3 b){

		//check y argument
		if (a.y < b.y) {
			return -1;
		}

		if (Mathf.Approximately (a.y, b.y)) {

			//check x argument
			if (a.x < b.x) {
				return -1;
			}

			if (Mathf.Approximately (a.x, b.x)) {

				//check z arguement
				if (a.z > b.z) {
					return 1;
				}

				if (Mathf.Approximately (a.z, b.z)) {
					return 0;
				}

				//b less in z
				return -1;

			}

			//b greater in x
			return 1;

		}

		//b greater in y
		return 1;
	}

					


}
