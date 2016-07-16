using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public class SinWaveBullet : StandardBullet {

		public float Frequency;
		public float Scale;

		#region StandardBullet Override methods

		public override void Do() {
			
			var vector = _velocity;

			var axis = Vector2.up;

			var angleBetweenUpAndVelocity = Mathf.Abs (Vector2.Angle (_velocity, Vector2.up));

			if(angleBetweenUpAndVelocity < 35 || angleBetweenUpAndVelocity > 125) {
				axis = Vector2.right;
			}

			vector = vector + axis * Mathf.Sin (Time.time * Frequency) * Scale;

			CharacterController.Velocity = vector;
		}

		#endregion
	}
}
