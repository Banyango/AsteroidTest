using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public class StandardWeapon : Weapon {

		public string PrefabName = "StandardBullet";

		#region UnityMethods

		public void Start() {
			GunCoolDownTime = 0.5f;
		}

		#endregion

		#region implemented abstract members of Weapon

		protected override void DoOnCreateBullet(Vector2 spawnPosition, Vector2 direction) {
			var bulletGameObject = (GameObject) GameObject.Instantiate (Resources.Load (PrefabName), spawnPosition, Quaternion.identity);

			// get the bullet and set the velocity / direction
			var standardBullet = bulletGameObject.GetComponent<StandardBullet> ();


			standardBullet.SetDirection (Vector2.Min(direction, (direction * 10).normalized));
		}

		#endregion

	}
	
}
