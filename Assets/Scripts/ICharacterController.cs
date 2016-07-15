using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Movement {

	public enum MoveDirection {
		Left, Right, Up, Down
	}

	public interface IMovementModifierListener {
		void DoAction (string eventType, object[] info);
	}

	public interface IColliding {
		void DoOnCollision(Collider2D colliderHit);
	}

	public abstract class IMovementModifier : MonoBehaviour 
	{
		private List<IMovementModifierListener> listeners = new List<IMovementModifierListener>();

		[HideInInspector]
		public ICharacterController CharacterController;

		[Header("IMovement Modifier")]
		public bool IsEnabled = true;

		public abstract void Do ();

		public virtual void DoAfter () {

		}

		public void AddListener(IMovementModifierListener modifier) {
			listeners.Add (modifier);
		}

		public void NotifyListeners(string eventType, object[] info) {
			foreach (var item in listeners) {
				item.DoAction (eventType, info);
			}
		}
	}

	public enum CollisionDirections2D 
	{
		Right,Left,Top,Bottom
	}

	[Serializable]
	public class CollisionState2D
	{
		public bool Right;
		public bool Left;
		public bool Top;
		public bool Bottom;

		public List<Collider2D> CollidersHit = new List<Collider2D>();

		public bool HasCollision() {
			return Bottom || Top || Right || Left;
		}

		public void Reset() {
			Right = false;
			Left = false;
			Top = false;
			Bottom = false;
			CollidersHit.Clear ();
		}

		public void Set(CollisionState2D collisionState) {
			this.Right = collisionState.Right;
			this.Left = collisionState.Left;
			this.Top = collisionState.Top;
			this.Bottom = collisionState.Bottom;
			this.CollidersHit = collisionState.CollidersHit;
		}
	}

	public struct RayCastOrigins {
		public Vector2 topLeft;
		public Vector2 topRight;
		public Vector2 bottomRight;
		public Vector2 bottomLeft;

		public void Copy(RayCastOrigins old) {
			topLeft.x = old.topLeft.x;
			topLeft.y = old.topLeft.y;

			topRight.x = old.topRight.x;
			topRight.y = old.topRight.y;

			bottomRight.x = old.bottomRight.x;
			bottomRight.y = old.bottomRight.y;

			bottomLeft.x = old.bottomLeft.x;
			bottomLeft.y = old.bottomLeft.y;
		}

		public Vector2 FindCorner(int index) {
			switch (index) {
				case 0:
					return topLeft;
				case 1:
					return topRight;
				case 2:
					return bottomLeft;
				case 3:
					return bottomRight;
			}

			return new Vector2 ();
		}
	}

	[RequireComponent( typeof(BoxCollider2D))]
	public class ICharacterController : MonoBehaviour {

		private CollisionState2D _collisionState = new CollisionState2D();
		private CollisionState2D _previousCollisionState = new CollisionState2D();
		private RayCastOrigins _raycastOrigins;
		private RayCastOrigins _raycastFuture;
		private BoxCollider2D _collider2D;
		private RaycastHit2D _raycastHit;
		private Vector2 _lastPosition;
		private List<IMovementModifier> _movements;
		private List<IColliding> _collisionHandlers;

		private RayCastOrigins _oldRayCastOrigins;

		protected Vector2 deltaMovement;

		public static RaycastHit2D[] Hits = new RaycastHit2D[10];

		public LayerMask PlatformMask;
		public int HorizontalRays;
		public int VerticalRays;
		public Vector2 Velocity;
		public string InitialState;
		public bool UseTunnelChecking;
		public bool IsEnabled = true;

		public ICharacterController RunAfter;

		public float SkinWidth;

		public CollisionState2D CollisionState {
			get {
				return _collisionState;
			}
		}			

		public void Awake() {
		    _collider2D = GetComponent<BoxCollider2D> ();

			_movements = new List<IMovementModifier> ();
			_collisionHandlers = GetComponentsInChildren<IColliding> ().ToList ();

			if (string.IsNullOrEmpty (InitialState)) {
				_movements.AddRange (GetComponentsInChildren<IMovementModifier> ().ToList ());
			} else {
				_movements.AddRange (transform.Find (InitialState).GetComponents<IMovementModifier> ().ToList ());
			}

			foreach (var item in _movements) {
				item.CharacterController = this;
			}	

			_lastPosition = transform.position;
		}

		public virtual void PreMove() {
			foreach(IMovementModifier modifier in _movements) {
				if (modifier.IsEnabled && modifier.enabled) {
					modifier.Do ();
				}
			}

			deltaMovement = Velocity * Time.fixedDeltaTime;
		}

		public void FixedUpdate() {	
			if (IsEnabled) {
				PreMove ();
				Move ();
				PostMove ();
				HandleCollisions ();
			}
		}			
			
		private void Move() {
		
			if (!Mathf.Approximately (deltaMovement.x, 0f) || !Mathf.Approximately (deltaMovement.y, 0f)) {

				_previousCollisionState.Set (_collisionState);

				_collisionState.Reset ();

				_oldRayCastOrigins.Copy(_raycastOrigins);

				PrimeRaycastOrigins ();

				if(!Mathf.Approximately (deltaMovement.x, 0f)) {
					MoveHorizontally ();
				}

				if (!Mathf.Approximately (deltaMovement.y, 0f)) {
					MoveVertically ();
				}						

				transform.Translate (deltaMovement, Space.World);

				if (UseTunnelChecking) {
					TunnelCheck ();
				}

			}
								
			if (Time.fixedDeltaTime > 0) {
				_lastPosition = transform.position;
				Velocity = deltaMovement / Time.fixedDeltaTime;
			}

			if (Mathf.Abs(Velocity.x) < 0.001f)
				Velocity.x = 0f;

			if (Mathf.Abs(Velocity.y) < 0.001f)
				Velocity.y = 0f;

		}


		public void HandleCollisions() {
			for(int i = 0; i < _collisionState.CollidersHit.Count; i++) {
				var colliderHit = _collisionState.CollidersHit [i];

				for (int j = 0; j < _collisionHandlers.Count; j++) {
					IColliding collisionHandler = _collisionHandlers[j];

					collisionHandler.DoOnCollision (colliderHit);
				}	

				var otherCollider = colliderHit.GetComponent<IColliding> ();

				if(otherCollider != null) {
					otherCollider.DoOnCollision (_collider2D);
				}
			}

			_collisionState.CollidersHit.Clear ();
		}

		public void ChangeState(string[] states) {
			_movements.Clear ();

			foreach (var state in states) {
				_movements.AddRange (transform.Find (state).GetComponents<IMovementModifier> ().ToList ());
			}

			foreach (var item in _movements) {
				item.CharacterController = this;
			}	
		}

		public T GetModifier<T> () {
			return (T)Convert.ChangeType (GetModifier (typeof(T)), typeof(T)); 
		}

		public Component GetModifier (Type type) {
			foreach (var mov in _movements) {
				if (type.IsInstanceOfType (mov)) {
					return mov;
				}
			}

			return null;
		}

		public virtual void PostMove() {
			foreach(IMovementModifier modifier in _movements) {
				if (modifier.IsEnabled && modifier.enabled) {
					modifier.DoAfter ();
				}
			}
		}

		private void DrawRay( Vector3 start, Vector3 dir, Color color )
		{
			Debug.DrawRay( start, dir, color );
		}

		private void PrimeRaycastOrigins()
		{
			var scaledColliderSize = new Vector2( _collider2D.size.x * Mathf.Abs( transform.localScale.x ), _collider2D.size.y * Mathf.Abs( transform.localScale.y ) ) / 2;
			var scaledCenter = new Vector2( _collider2D.offset.x * transform.localScale.x, _collider2D.offset.y * transform.localScale.y );

			_raycastOrigins.topRight = transform.position + new Vector3( scaledCenter.x + scaledColliderSize.x, scaledCenter.y + scaledColliderSize.y );
			_raycastOrigins.topRight.x -= SkinWidth;
			_raycastOrigins.topRight.y -= SkinWidth;

			_raycastOrigins.topLeft = transform.position + new Vector3( scaledCenter.x - scaledColliderSize.x, scaledCenter.y + scaledColliderSize.y );
			_raycastOrigins.topLeft.x += SkinWidth;
			_raycastOrigins.topLeft.y -= SkinWidth;

			_raycastOrigins.bottomRight = transform.position + new Vector3( scaledCenter.x + scaledColliderSize.x, scaledCenter.y -scaledColliderSize.y );
			_raycastOrigins.bottomRight.x -= SkinWidth;
			_raycastOrigins.bottomRight.y += SkinWidth;

			_raycastOrigins.bottomLeft = transform.position + new Vector3( scaledCenter.x - scaledColliderSize.x, scaledCenter.y -scaledColliderSize.y );
			_raycastOrigins.bottomLeft.x += SkinWidth;
			_raycastOrigins.bottomLeft.y += SkinWidth;

			Debug.DrawLine (_raycastOrigins.topLeft, _raycastOrigins.topRight);
			Debug.DrawLine (_raycastOrigins.topRight, _raycastOrigins.bottomRight);
			Debug.DrawLine (_raycastOrigins.bottomRight, _raycastOrigins.bottomLeft);
			Debug.DrawLine (_raycastOrigins.bottomLeft, _raycastOrigins.topLeft);

		}

		private void MoveHorizontally () {

			bool isGoingRight = deltaMovement.x > 0;

			var rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidth;
			var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
			var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

			var colliderUseableHeight = _collider2D.size.y * Mathf.Abs (transform.localScale.y) - ( 2f * SkinWidth );
			var verticalDistanceBetweenRays = colliderUseableHeight / ( HorizontalRays - 1 );

			for (int i = 0; i < HorizontalRays; i++) {
				var ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * verticalDistanceBetweenRays );

				DrawRay( ray, rayDirection * rayDistance, Color.red );

				_raycastHit = Physics2D.Raycast ( ray, rayDirection, rayDistance, PlatformMask);

				if (_raycastHit) {

					deltaMovement.x = _raycastHit.point.x - ray.x;

					rayDistance = Mathf.Abs (deltaMovement.x);

					if( isGoingRight) {
						deltaMovement.x -= SkinWidth;
						_collisionState.Right = true;
					} else {
						deltaMovement.x += SkinWidth;
						_collisionState.Left = true;
					}

					if(!_collisionState.CollidersHit.Contains (_raycastHit.collider)) {
						_collisionState.CollidersHit.Add (_raycastHit.collider);
					}
				}
			}				
		}

		private void MoveVertically () {

			bool isGoingUp = deltaMovement.y > 0;

			var rayDistance = Mathf.Abs (deltaMovement.y) + SkinWidth;
			var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
			var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

			initialRayOrigin.x += deltaMovement.x;

			var colliderUseableWidth = _collider2D.size.x * Mathf.Abs (transform.localScale.x) - ( 2f * SkinWidth );
			var horizontalDistanceBetweenRays = colliderUseableWidth / ( VerticalRays - 1 );

			for (int i = 0; i < VerticalRays; i++) {
				var ray = new Vector2( initialRayOrigin.x + i * horizontalDistanceBetweenRays, initialRayOrigin.y );

				DrawRay( ray, rayDirection.normalized * rayDistance, Color.yellow );

				_raycastHit = Physics2D.Raycast (ray, rayDirection, rayDistance, PlatformMask);

				if (_raycastHit) {

					deltaMovement.y = _raycastHit.point.y - ray.y;

					rayDistance = Mathf.Abs (deltaMovement.y);
						
					if(isGoingUp) {
						deltaMovement.y -= SkinWidth;
						_collisionState.Top = true;
					} else {
						deltaMovement.y += SkinWidth;
						_collisionState.Bottom = true;
					}

					if(!_collisionState.CollidersHit.Contains (_raycastHit.collider)) {
						_collisionState.CollidersHit.Add (_raycastHit.collider);
					}
				}
			}
		}			
	
		public void TunnelCheck () {
				

			var delta = ((Vector2) transform.position) - _lastPosition;

			Debug.DrawLine (_lastPosition, transform.position);

			int hits = Physics2D.BoxCastNonAlloc (_lastPosition, new Vector2(_collider2D.size.x - 2 * SkinWidth, _collider2D.size.y - 2 * SkinWidth), 0, delta, Hits, Vector2.Distance(_lastPosition, transform.position), PlatformMask); 

//			Debug.Log (transform.name + " Tunnel Check Hits" + hits);
			if (hits > 0) {

				float fraction = Hits [0].fraction;

				if (hits > 1) {
					for (int i = 0; i < hits; i++) {
						if (Hits [i].fraction < fraction) {
							fraction = Hits [i].fraction;
						}

						if(!_collisionState.CollidersHit.Contains (Hits[i].collider)) {
							_collisionState.CollidersHit.Add (Hits[i].collider);
						}
					}
				}

				transform.position = _lastPosition + (delta * fraction);

				AdjustTransformForSkinWidth (delta);
				AdjustTransformForSkinHeight (delta);

				deltaMovement = new Vector2 (0,0);

			}


		}

		public Vector2 GetDeltaMovement() {
			return deltaMovement;
		}

		public bool TranslateWithBoxCast(Vector2 delta) {
		
			bool hitOccurred = false;

			int hits = Physics2D.BoxCastNonAlloc (transform.position, new Vector2(_collider2D.size.x - 2 * SkinWidth, _collider2D.size.y - 2 * SkinWidth), 0, delta, Hits, Vector2.Distance(transform.position, ((Vector2)transform.position) + delta), PlatformMask); 


			if (hits > 0) {
				for (int i = 0; i < hits; i++) {			
					
					transform.position = _lastPosition + (delta * Hits [i].fraction);

					if(!_collisionState.CollidersHit.Contains (Hits[i].collider)) {
						_collisionState.CollidersHit.Add (Hits[i].collider);
					}
				}

				AdjustTransformForSkinWidth (delta);
				AdjustTransformForSkinHeight (delta);

				hitOccurred = true;
			} else {
				transform.Translate (delta, Space.World);
			}				

			deltaMovement = new Vector2 ();

			return hitOccurred;
		}

		public void TranslateAndIgnoreCollision(Vector2 newPosition) {			
			transform.position = newPosition;
			deltaMovement = new Vector2 ();
			UpdateLastPosition ();
		}			

		public void UpdateLastPosition (Vector2 newPosition) {
			_lastPosition = newPosition;
		}

		public void UpdateLastPosition() {
			_lastPosition = transform.position;
		}
	
		public void AdjustTransformForSkinWidth (Vector2 delta)
		{
			var skinAdjust = new Vector2 (transform.position.x, transform.position.y);

			if (delta.x > 0) {
				skinAdjust.x = transform.position.x - SkinWidth;
			} else if (delta.x < 0) {
				skinAdjust.x = transform.position.x + SkinWidth;
			}

			transform.position = skinAdjust;
		}

		public void AdjustTransformForSkinHeight (Vector2 delta) {
			var skinAdjust = new Vector2 (transform.position.x, transform.position.y);

			if (delta.y > 0) {
				skinAdjust.y = transform.position.y - SkinWidth;
			} else if (delta.y < 0) {
				skinAdjust.y = transform.position.y + SkinWidth;
			}

			transform.position = skinAdjust;
		}
	}

		
}
