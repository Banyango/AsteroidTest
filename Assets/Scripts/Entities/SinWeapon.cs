using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public class SinWeapon : Weapon {

		public string PrefabName = "SinWaveBullet";

		#region UnityMethods

		public void Start() {
			GunCoolDownTime = 0.5f;
		}

		#endregion

		#region implemented abstract members of Weapon

		protected override void DoOnCreateBullet(Vector2 spawnPosition, Vector2 direction) {
			var bulletGameObject = (GameObject) GameObject.Instantiate (Resources.Load (PrefabName), spawnPosition, Quaternion.identity);

			var sinBullet = bulletGameObject.GetComponent<SinWaveBullet> ();

			sinBullet.SetDirection (Vector2.Min(direction, (direction * 10).normalized));
		}

		#endregion

	}
	
}
