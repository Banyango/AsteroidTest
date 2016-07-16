using UnityEngine;
using System.Collections;
using Movement;

namespace Asterlike {

	public interface IInputManager {
		void CheckForInput(IInputResponding iMoving);
	}

	public class KeyboardInputManager : MonoBehaviour, IInputManager {
		
		public KeyCode ThrottleFwdButton;
		public KeyCode ThrottleBackButton;
		public KeyCode TurnLeftButton;
		public KeyCode TurnRightButton;
		public KeyCode ShootButton;

		#region IInputManager implementation

		public void CheckForInput(IInputResponding iMoving) {

			if(Input.GetKey (ThrottleFwdButton)) {
				iMoving.DoOnThrottleUp ();
			} 

			if(Input.GetKey (ThrottleBackButton)) {
				iMoving.DoOnThrottleDown ();
			}

			if(Input.GetKey(TurnLeftButton)) {
				iMoving.DoOnTurn (true);
			} 

			if(Input.GetKey(TurnRightButton)) {
				iMoving.DoOnTurn (false);
			}

			if(Input.GetKey (ShootButton)) {
				iMoving.DoOnShoot ();
			}

		}

		#endregion

	}
}