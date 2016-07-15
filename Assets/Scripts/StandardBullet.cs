using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {
	public class StandardBullet : IMovementModifier {

		public float Speed;

		private Vector2 _velocity;
		private Transform _sprite;

		#region Unity Methods

		void Start () {
			_sprite = transform.FindChild ("Sprite");
		}

		public void Update() {
			RotateSprite ();
		}

		#endregion

		public void SetDirection(Vector2 direction) {
			_velocity = Speed * direction.normalized;
		}

		private void RotateSprite() {

			var normalizedVelocity = CharacterController.Velocity.normalized;

			var angle = Mathf.Atan2 (normalizedVelocity.y, normalizedVelocity.x) * Mathf.Rad2Deg;

			_sprite.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

		}


		#region implemented abstract members of IMovementModifier

		public override void Do() {
			CharacterController.Velocity = _velocity;			
		}

		#endregion
	}
}
