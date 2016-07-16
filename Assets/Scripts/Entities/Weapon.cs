using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public abstract class Weapon : MonoBehaviour {

		private bool _isEnabled = true;

		[Header("Attributes")]
		public float GunCoolDownTime;

		private IEnumerator WaitThenEnableWeapon() {
			_isEnabled = false;

			yield return new WaitForSeconds (GunCoolDownTime);

			_isEnabled = true;
		}

		public void ShootBullet(Vector2 spawnPosition, Vector2 direction) {
			if(_isEnabled) {
				DoOnCreateBullet (spawnPosition, direction);

				StartCoroutine (WaitThenEnableWeapon ());
			}
		}

		protected abstract void DoOnCreateBullet(Vector2 spawnPosition, Vector2 direction);

	}
	
}
