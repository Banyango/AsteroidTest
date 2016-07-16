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

			vector = vector + ((Vector2)transform.up) * Mathf.Sin (Time.time * Frequency) * Scale;

			CharacterController.Velocity = vector;
		}

		#endregion
	}
}
