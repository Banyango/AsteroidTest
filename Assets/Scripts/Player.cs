using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public interface IInputResponding {
		void DoOnThrottleUp();
		void DoOnTurn(bool isLeft);
		void DoOnShoot();
		void DoOnThrottleDown();
	}

	public class Player : IMovementModifier {
		
		public const float MIN_VELOCITY_FOR_ROTATION = 0.1f;

		public IInputManager InputHandler;

		private SpriteRenderer _sprite;

		[Header("Player Sprite")]
		public NumericSpring RotateSpring;

		#region UnityMethods

		public void Start() {
			
			_sprite = GetComponentOnSpecificChild<SpriteRenderer> ("Sprite");

			RotateSpring.value = 0;

		}	

		public void Update() {
			RotateSprite ();
		}
			
		#endregion

		private T GetComponentOnSpecificChild<T>(string childName) where T : Component {

			var child = transform.FindChild (childName);

			if(child == null) {
				Debug.Log ("Error: Expecting Child " + childName + " not found on object " + transform.name);

				return null;
			}				

			return child.GetComponent<T>();
		}

		private void RotateSprite() {

			var normalizedVelocity = CharacterController.Velocity.normalized;

			if(normalizedVelocity.magnitude > MIN_VELOCITY_FOR_ROTATION) {

				var angle = Mathf.Atan2 (normalizedVelocity.y, normalizedVelocity.x) * Mathf.Rad2Deg;

				if(angle < 0f) {
					angle += 2 * Mathf.PI * Mathf.Rad2Deg; 
				}
								
				if(angle - RotateSpring.value > (Mathf.PI * Mathf.Rad2Deg)) { 
					RotateSpring.value = 2 * Mathf.PI * Mathf.Rad2Deg;
				} else if(angle - RotateSpring.value < -(Mathf.PI * Mathf.Rad2Deg)) {
					RotateSpring.value = 0;
				}

				RotateSpring.SetTarget (angle);

				RotateSpring.Update (Time.deltaTime);

				_sprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, RotateSpring.value));
			}

		}
			
		#region implemented abstract members of IMovementModifier

		public override void Do() {
			
		}

		#endregion
	}
}

