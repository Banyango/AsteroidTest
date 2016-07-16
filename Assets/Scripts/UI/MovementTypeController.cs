using UnityEngine;
using System.Collections;

namespace Asterlike {
	public class MovementTypeController : MonoBehaviour {

		public Player player;

		public void DoOnMovementTypeUpdate( string type ) {
			switch (type) {
			case "WASD":
				player.ChangeToWASDMovement();
				break;
			case "ASTERLIKE":
				player.ChangeToAsterlikeMovement ();
				break;
			}
		}
	}
}