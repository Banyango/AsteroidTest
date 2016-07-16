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

		public void UnRegisterWallListener(IWallTypeListening wall) {
			listeners.Remove (wall);
		}

		public void OnWallToggleSelected(string type) {
			for(int i = 0; i < listeners.Count; i++) {
				IWallTypeListening listener = listeners [i];

				_currentType = (WallType) System.Enum.Parse (typeof(WallType), type);

				listener.SetWallCollisionBehaviour (_currentType);
			}

		}

	}

		
}
