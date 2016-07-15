using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public class StandardWeapon : Weapon {

		#region implemented abstract members of Weapon

		protected override void DoOnCreateBullet(Vector2 spawnPosition, Vector2 direction) {
			var bulletGameObject = (GameObject) GameObject.Instantiate (Resources.Load ("Bullet"), spawnPosition, Quaternion.identity);

			// get the bullet and set the velocity / direction
			var standardBullet = bulletGameObject.GetComponent<StandardBullet> ();

			standardBullet.SetDirection (direction);
		}

		#endregion

	}
	
}
