using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
	[RequireComponent(typeof (AudioSource))]
	public class Platformer2DUserControl : MonoBehaviour
    {
		public AudioClip[] m_AudioClips;

        private PlatformerCharacter2D m_Character;
		private AudioSource m_AudioSource;
		private bool m_Jump;

		public int playerIndex;

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
			m_AudioSource = GetComponent<AudioSource>();
		}


        private void Update()
        {
            if (!m_Jump)
            {
				// Read the jump input in Update so button presses aren't missed.
				m_Jump = CrossPlatformInputManager.GetButtonDown("Jump"+playerIndex);

				if (playerIndex == 1)
				{
					m_Jump |= Input.GetKeyDown(KeyCode.W);
				}
				else
				{
					m_Jump |= Input.GetKeyDown(KeyCode.UpArrow);
				}

				// play sound
				if (m_Jump && m_Character.IsGrounded())
				{
					m_AudioSource.clip = m_AudioClips[UnityEngine.Random.Range(0, 2)];
					m_AudioSource.Play();
				}
			}
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal" + playerIndex);

			if (playerIndex == 1)
			{
				if (Input.GetKey(KeyCode.A))
					h = -1;
				if (Input.GetKey(KeyCode.D))
					h = 1;
			}
			else
			{
				if (Input.GetKey(KeyCode.LeftArrow))
					h = -1;
				if (Input.GetKey(KeyCode.RightArrow))
					h = 1;
			}

			// Pass all parameters to the character control script.
			m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
		}
    }
}
