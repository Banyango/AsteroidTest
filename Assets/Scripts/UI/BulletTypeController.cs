using UnityEngine;
using System.Collections;

namespace Asterlike {
	public class BulletTypeController : MonoBehaviour {

		public Player player;

		public void OnBulletTypeToggleSelected(string type) {			
			player.ChangeWeapon (type);
		}
	}
}