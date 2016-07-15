using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike { 
	
	public class WASDMovementModifier : IMovementModifier, IInputResponding {

		public IInputManager InputHandler;

		public const float MIN_THRUST_VELOCITY = 0.1f;

		private Vector2 _velocity;

		private Vector2 _thrustDirection;

		private float _thrustCurveIndex;

		[Header("Player Movement")]
		public AnimationCurve ForwardsThrustForce;

		public float Drag;

		#region Unity Methods

		public void Start() {

			InputHandler = GetComponent<IInputManager> ();

		}

		public void Update() {

			ResetMovementVector ();

			InputHandler.CheckForInput(this);

			ClampCurveIndicies ();

			IncrementCurveIndicies ();

		}
					
		#endregion

		private void IncrementCurveIndicies() {


			if(!IsThrustingInAnyDirection ()) {
				_thrustCurveIndex = 0;
			} else {
				_thrustCurveIndex += Time.deltaTime;
			}

		}

		private void ClampCurveIndicies() {
			_thrustCurveIndex = Mathf.Clamp (_thrustCurveIndex, 0 , ForwardsThrustForce.keys[ForwardsThrustForce.length-1].time);
		}

		private bool IsThrustingInAnyDirection() {
			return Mathf.Abs(_thrustDirection.x) > MIN_THRUST_VELOCITY || Mathf.Abs(_thrustDirection.y) > MIN_THRUST_VELOCITY;
		}

		private void ResetMovementVector() {
			_thrustDirection = new Vector2 ();
		}


		#region IMoving implementation

		public void DoOnThrottleUp() {
			_thrustDirection += Vector2.up;
		}

		public void DoOnThrottleDown() {
			_thrustDirection += Vector2.down;
		}

		public void DoOnTurn(bool isLeft) {
			if(isLeft) {
				_thrustDirection += Vector2.left;
			} else {
				_thrustDirection += Vector2.right;
			}
		}

		public void DoOnShoot() {

		}

		#endregion

		#region implemented abstract members of IMovementModifier

		public override void Do() {

			_velocity = new Vector2(0, 0);

			if(IsThrustingInAnyDirection()) {
				_velocity += _thrustDirection * ForwardsThrustForce.Evaluate(_thrustCurveIndex);
			}				

			_velocity *= Drag;

			CharacterController.Velocity += _velocity;

			CharacterController.Velocity *= Drag;
		}

		#endregion
	}

}