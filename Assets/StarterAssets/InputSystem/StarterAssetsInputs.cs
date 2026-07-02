using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		//public Behaviour characterRetargeter;
		//private bool retargetEnabled;
		
		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        private void Update()
        {
			//Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

			//MoveInput(moveInput);

			//if (OVRInput.GetDown(OVRInput.Button.One))
			//	JumpInput(true);
			//else 
			//	JumpInput(false);

			//SprintInput(
			//	OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.7f
			//	);

			//if (OVRInput.GetDown(OVRInput.Button.Three))
			//{
			//	retargetEnabled = !retargetEnabled;
			//	characterRetargeter.enabled = retargetEnabled;
			//}

			//if (moveInput.sqrMagnitude > 0.001f || OVRInput.GetDown(OVRInput.Button.One))
			//{
			//	characterRetargeter.enabled = false;
			//}
			////else
			////{
			////	characterRetargeter.enabled = true;
			////}

















		}

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}