using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike { 
	
	public class AsteroidsLikeMovementModifier : IMovementModifier, IInputResponding {

		public IInputManager InputHandler;

		private Vector2 _velocity;
		private Vector2 _facingDirection;

		private bool _isThrusting;
		private bool _isTurning;
		private float _turnDirection;
		private float _thrustCurveIndex;
		private float _turnCurveIndex;

		[Header("Player Movement")]
		public AnimationCurve ForwardsThrustForce;
		public AnimationCurve TurnSpeed;
		public float Drag;

		#region Unity Methods

		public void Start() {

			_facingDirection = Vector2.up;

			InputHandler = GetComponent<IInputManager> ();

		}

		public void Update() {

			ResetMovementBool ();

			InputHandler.CheckForInput(this);

			ClampCurveIndicies ();

			IncrementCurveIndicies ();

		}
			
		void OnDrawGizmos() {
			Debug.DrawLine(transform.position, transform.position + (Vector3) _facingDirection);
		}
			
		#endregion

		private void IncrementCurveIndicies() {

			if(!_isTurning) {
				_turnCurveIndex = 0;
			} else {
				_turnCurveIndex += Time.deltaTime;
			}

			if(!_isThrusting) {
				_thrustCurveIndex = 0;
			} else {
				_thrustCurveIndex += Time.deltaTime;
			}

		}

		private void ClampCurveIndicies() {
			_thrustCurveIndex = Mathf.Clamp (_thrustCurveIndex, 0 , ForwardsThrustForce.keys[ForwardsThrustForce.length-1].time);
			_turnCurveIndex = Mathf.Clamp (_turnCurveIndex, 0, TurnSpeed.keys[TurnSpeed.length-1].time);
		}

		private void ResetMovementBool() {
			_isThrusting = false;
			_isTurning = false;
		}


		#region IMoving implementation

		public void DoOnThrottleUp() {
			_isThrusting = true;
		}

		public void DoOnThrottleDown() {

		}

		public void DoOnTurn(bool isLeft) {
			_isTurning = true;

			if(isLeft) {
				_turnDirection = -1f;
			} else {
				_turnDirection = 1f;
			}
		}

		public void DoOnShoot() {

		}

		#endregion

		#region implemented abstract members of IMovementModifier

		public override void Do() {

			_velocity = new Vector2(0, 0);

			if(_isTurning) {
				_facingDirection = Quaternion.Euler (new Vector3 (0, 0, TurnSpeed.Evaluate(_turnCurveIndex) * _turnDirection * Time.fixedDeltaTime)) * _facingDirection;
			}

			if(_isThrusting) {

				var direction = _facingDirection;

				direction.Normalize ();

				_velocity += direction * ForwardsThrustForce.Evaluate(_thrustCurveIndex);

			}				

			_velocity *= Drag;

			CharacterController.Velocity += _velocity;

			CharacterController.Velocity *= Drag;
		}

		#endregion
	}

}