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

	public interface IWallTypeListening {
		void SetWallCollisionBehaviour(WallType type);
	}
		
	public class Player : IMovementModifier, IInputResponding, IColliding, IWallTypeListening {
		
		public const float MIN_VELOCITY_FOR_ROTATION = 0.1f;

		public IInputManager InputHandler;

		private SpriteRenderer _sprite;
		private Weapon _weapon;
		private WallType _wallCollisionBehaviour;
		private WallLogicHandler _wallLogicHandler = new WallLogicHandler();

		private Transform _bulletSpawnPoint;
		private bool _isDead = false;

		[Header("Player Sprite")]
		public NumericSpring RotateSpring;

		[Header("Attributes")]
		public float RespawnTime = 1f;
		public Transform RespawnPoint;

		#region UnityMethods

		public void Start() {
			
			_weapon = GetComponent<Weapon> ();

			_sprite = GetComponentOnSpecificChild<SpriteRenderer> ("Sprite");
			_bulletSpawnPoint = GetComponentOnSpecificChild<Transform> ("Sprite/BulletSpawningPoint");

			InputHandler = GetComponent<IInputManager> ();

			RotateSpring.value = 0;

			GameObject.FindObjectOfType<WallTypeController> ().RegisterWallListener (this);

		}	

		public void Update() {

			RotateSprite ();

			InputHandler.CheckForInput (this);

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

		private void HandleCollisionWithBullet(CollisionHit2D bulletCollider) {

			if(!_isDead) {
				DestroyShip ();
			}

		}

		private void HandleCollisionWithWall(CollisionHit2D colliderHit) {
			_wallLogicHandler.Handle (_wallCollisionBehaviour, CharacterController, colliderHit, transform);			
		}

		public void SetWallCollisionBehaviour(WallType type) {
			_wallCollisionBehaviour = type;				
		}

		public void DestroyShip() {

			_isDead = true;

			_sprite.enabled = false;

			CharacterController.IsEnabled = false;

			CharacterController.Velocity = new Vector2 ();

			StartCoroutine (WaitThenRespawn ());

		}

		public void RespawnShip() {

			_isDead = false;

			_sprite.enabled = true;

			CharacterController.IsEnabled = true;

			CharacterController.TranslateAndIgnoreCollision (RespawnPoint.position);

		}

		private IEnumerator WaitThenRespawn() {
			yield return new WaitForSeconds (RespawnTime);
			_isDead = false;
			RespawnShip ();
		}

		public void ChangeWeapon(string type) {
			switch (type) {
			case "SIN": 
				ChangeWeapon<SinWeapon> ();
				break;
			case "STANDARD":
				ChangeWeapon<StandardWeapon> ();
				break;
			default:
				Debug.LogError ("Weapon type string not recognized " + type);
				break;
			}
		}

		public void ChangeWeapon<T>() where T : Weapon {

			DestroyImmediate (_weapon);

			_weapon = gameObject.AddComponent<T> ();

		}

		#region IColliding implementation 

		public void DoOnCollision(CollisionHit2D colliderHit) {
			if(colliderHit.collider.tag.Equals ("Bullet")) {
				HandleCollisionWithBullet (colliderHit);
			}

			if(colliderHit.collider.tag.Equals ("MainCamera")) {
				HandleCollisionWithWall (colliderHit);
			}
		}

		#endregion
			
		#region implemented abstract members of IMovementModifier


		public override void Do() {
				
		}

		#endregion

		#region IInputResponding implementation

		public void DoOnThrottleUp() {
			
		}

		public void DoOnTurn(bool isLeft) {
			
		}

		public void DoOnShoot() {
			_weapon.ShootBullet (_bulletSpawnPoint.position, CharacterController.Velocity);	
		}

		public void DoOnThrottleDown() {
			
		}

		#endregion
	}
}

