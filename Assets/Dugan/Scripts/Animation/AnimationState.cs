using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Dugan.Animation {
	public class AnimationState {//Associates a base animation clip with animation state information for an instance.
		public string name { get; internal set; }
		public AnimationClip clip { get; internal set; }
		public float normalizedStartTime { get; internal set; }
		public float normalizedEndTime { get; internal set; }
		public bool bAddLoopFrame { get; internal set; }
		public int mixerIndex { get; internal set; }
		public SplitClipPlayable playable { get; internal set; }
		public bool isValid = true;
		public WrapMode wrapMode = WrapMode.Default;
		private AnimationMixerPlayable mixer;

		public float time {
			get { return (float)playable.playable.GetTime() - startTime; }
			set { playable.playable.SetTime(value + startTime);	}
		}

		public float normalizedTime {
			get { return (time / length) - UnityEngine.Mathf.Floor(time / length); }
			set { playable.playable.SetTime(value * length + startTime); }
		}

		public float totalNormalizedTime {
			get { return time / length; }
			set { float t = value * length + startTime; playable.playable.SetTime(t);}
		}

		public float speed {
			get { return (float)playable.playable.GetSpeed(); }
			set { playable.playable.SetSpeed(value); }
		}

		public float normalizedSpeed {
			get { return speed * length; }
			set { speed = value / length; }
		}

		private bool _enabled = true;
		public bool enabled {
			get { return _enabled;	}
			set {
				_enabled = value;
				if (!_enabled) {
					playable.playable.Pause();
				} else {
					isPlaying = _isPlaying;
				}
			}
		}

		private bool _isPlaying = false;
		public bool isPlaying {
			get { return _isPlaying;	}
			set {
				_isPlaying = value;
				if (_enabled) {
					if (value)
						playable.playable.Play();
					else
						playable.playable.Pause();
				}
			}
		}

		public float length { get { return (normalizedEndTime * clip.length) - (normalizedStartTime * clip.length); } }
		public float startTime { get { return normalizedStartTime * length; } }
		public float endTime { get { return normalizedEndTime * length;	} }

		public int frameCount { get { return UnityEngine.Mathf.RoundToInt(length * clip.frameRate);	} }
		public int startFrame { get { return UnityEngine.Mathf.RoundToInt(normalizedStartTime * (length * clip.frameRate)); } }
		public int endFrame { get { return UnityEngine.Mathf.RoundToInt(normalizedEndTime * (length * clip.frameRate));	} }

		public float weight { get { return mixer.GetInputWeight(mixerIndex); } set { mixer.SetInputWeight(mixerIndex, value); } }

		public AnimationState(AnimationClip clip, string name, int startFrame, int endFrame, bool addLoopFrame, SplitClipPlayable playable, int mixerIndex, AnimationMixerPlayable mixer) {
			float frameCount = clip.frameRate * clip.length;
			this.clip = clip;
			this.normalizedStartTime = 0;//(float)startFrame / frameCount;
			this.normalizedEndTime = 1;//(float)endFrame / frameCount;
			this.bAddLoopFrame = addLoopFrame;
			this.playable = playable;
			this.mixerIndex = mixerIndex;
			this.name = name;

			this.mixer = mixer;
		}
	}
}
