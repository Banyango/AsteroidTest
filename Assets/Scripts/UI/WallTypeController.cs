using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Movement;

namespace Asterlike {
	
	public enum WallType {
		WALL,
		BOUNCE,
		WRAP
	}

	public class WallTypeController : MonoBehaviour {

		private List<IWallTypeListening> listeners = new List<IWallTypeListening>();
		private WallType _currentType = WallType.WALL;

		public void RegisterWallListener(IWallTypeListening wall) {
			listeners.Add (wall);

			wall.SetWallCollisionBehaviour (_currentType);
		}

		public void OnWallToggleSelected(string type) {
			for(int i = 0; i < listeners.Count; i++) {
				IWallTypeListening listener = listeners [i];

				_currentType = (WallType) System.Enum.Parse (typeof(WallType), type);

				listener.SetWallCollisionBehaviour (_currentType);
			}

		}

	}

	public class WallLogicHandler {
		public void Handle(WallType wallCollisionBehaviour, ICharacterController controller, CollisionHit2D colliderHit, Transform transform) {
			switch (wallCollisionBehaviour) {
			case WallType.WALL:
				// do nothing.
				break;
			case WallType.BOUNCE:
				// 
				controller.Velocity = -controller.Velocity;
				break;
			case WallType.WRAP:
				var point = Camera.main.WorldToViewportPoint (transform.position);

				var newPosition = transform.position;

				if(point.x >= 0.8f) {
					newPosition.x = -newPosition.x - controller.Collider.size.x * 2;
				} else if(point.x <= -0.2f) {
					newPosition.x = -newPosition.x  + controller.Collider.size.x * 2;
				}

				if(point.y >= 0.8f) {
					newPosition.y = -newPosition.y  - controller.Collider.size.y * 2;

				} else if(point.y <= -0.2f) {
					newPosition.y = -newPosition.y  + controller.Collider.size.y * 2;
				}
					
				controller.TranslateAndIgnoreCollision (newPosition);

				break;
			}
		}
	}
		
}
