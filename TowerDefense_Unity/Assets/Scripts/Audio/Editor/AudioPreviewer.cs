using UnityEngine;

namespace Game.Audio.Editor
{
	/// <summary>
	/// Can be used to preview Audio Assets by temporarily creating a audio source in the scene.
	/// <para>NOTE: User has to call Remove() manually when preview is not available.</para>
	/// </summary>
	public class AudioPreviewer
	{
		private AudioSource _AudioPreviewer;

		public void Create()
		{
			if (_AudioPreviewer)
				return;

			GameObject gameObject = new GameObject {
				hideFlags = HideFlags.HideAndDontSave
			};

			_AudioPreviewer = gameObject.AddComponent<AudioSource>();
		}

		public void Remove()
		{
			if (!_AudioPreviewer)
				return;

			Object.DestroyImmediate(_AudioPreviewer.gameObject);
			_AudioPreviewer = null;
		}

		public void Play(AudioAsset audioAsset)
		{
			if (audioAsset.ClipCount == 0)
				return;

			AudioClip audioClip = audioAsset.GetClip();
			if (audioClip == null)
			{
				Debug.LogWarning("Audio asset returned null. Make sure all audioclip fields are filled.");
			}


			_AudioPreviewer.clip = audioAsset.GetClip();
			_AudioPreviewer.Play();
		}
	}
}
