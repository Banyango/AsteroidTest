using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Asterlike {
	public class RespawnTimeController : MonoBehaviour {

		public Player player;

		public void OnRespawnTimeUpdate(InputField field) {

			int result = 0;
			if(int.TryParse (field.text, out result)) {
				player.RespawnTime = result;
			} else {
				field.text = "1";
				player.RespawnTime = 1;
			}

		}
	}
}