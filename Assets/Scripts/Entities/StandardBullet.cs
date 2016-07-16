using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public class StandardBullet : IMovementModifier, IColliding, IWallTypeListening {

		public float Speed;

		protected Vector2 _velocity;

		private Transform _sprite;

		private WallType _wallCollisionBehaviour;

		#region Unity Methods

		void Start () {
			_sprite = transform.FindChild ("Sprite");

			GameObject.FindObjectOfType<WallTypeController> ().RegisterWallListener (this);
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

		#region IColliding implementation

		public void DoOnCollision(CollisionHit2D colliderHit) {
			if(colliderHit.collider.tag.Equals ("MainCamera")) {
				HandleCollisionWithWall (colliderHit);	
			}
		}

		#endregion

		private void HandleCollisionWithWall(CollisionHit2D colliderHit) {

			const float ScreenBoundsTop = .9f;
			const float ScreenBoundsBottom = .1f;

			switch (_wallCollisionBehaviour) {
			case WallType.WALL:
				// do nothing.
				break;
			case WallType.BOUNCE:				 				
				_velocity = Vector2.Reflect(_velocity, colliderHit.normal);   
				break;
			case WallType.WRAP:				

				var hitPoint = colliderHit.point;

				var position = Camera.main.WorldToViewportPoint (hitPoint);

				var newPosition = Camera.main.WorldToViewportPoint (transform.position);

				if(position.x >= ScreenBoundsTop) {
					newPosition.x = ScreenBoundsBottom;
				} else if(position.x <= ScreenBoundsBottom) {
					newPosition.x = ScreenBoundsTop;
				}

				if(position.y >= ScreenBoundsTop) {
					newPosition.y = ScreenBoundsBottom;
				} else if(position.y <= ScreenBoundsBottom) {
					newPosition.y = ScreenBoundsTop;
				}

				CharacterController.TranslateAndIgnoreCollision (Camera.main.ViewportToWorldPoint(newPosition));

				break;
			}
		}

		#region IWallTypeListener implementation

		public void SetWallCollisionBehaviour(WallType type) {
			_wallCollisionBehaviour = type;
		}

		#endregion
	}
}
