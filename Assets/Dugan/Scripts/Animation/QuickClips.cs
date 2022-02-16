using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/**
	This class is meant to be a replacement for the legacy animation system. 
	It is intened to replicate as much of the original functionality as possible, while 
	making few changes to the original interface.
**/

namespace Dugan.Animation {
	public class QuickClips : MonoBehaviour {

		//Serialized/public fields

		[SerializeField]
		private AnimationClip _clip = null;
		public AnimationClip clip {
			get { return _clip; }
			set {
				//Make it either add or replace the clip.
				_clip = value;
				if (animator != null && clip != null)
					AddClip(_clip, _clip.name);
			}
		}

		[SerializeField]
		private List<AnimationClip> animations = null;
		public bool playAutomatically = true;

		[SerializeField]
		private bool _animatePhysics = false;

		public bool animatePhysics {
			get { return _animatePhysics; }
			set {
				_animatePhysics = value;
				if (animator != null) {
					if (_animatePhysics)
						animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
					else
						animator.updateMode = AnimatorUpdateMode.Normal;
				}
			}
		}

		public bool isPlaying {
			get {
				return IsPlaying();
			}
		}

		// public Bounds localBounds {
		// 	get {
		// 		return animator.ava
		// 	}
		// }

		public AnimatorCullingMode cullingMode = AnimatorCullingMode.AlwaysAnimate;

		//Non serialized private fields
		private Animator animator = null;

		private PlayableGraph playableGraph;

		private AnimationMixerPlayable mixer;

		private AnimationPlayableOutput output;

		private List<AnimationState> animationStates = null;
		private Dictionary<string, AnimationState> namesAndAnimationStates = null;

		private List<SplitClipPlayable> splitClipPlayables = null;
		private Dictionary<string, SplitClipPlayable> namesAndSplitClipPlayables = null;

		private List<BlendStateData> blendStates = null;
		private Dictionary<string, BlendStateData> namesAndBlendStates = null;

		private CrossFadeData crossfade;

		//public UnityEngine.Animation aaa;
		public List<string> playQeue = null;
		private bool bPlayingQeue = false;

		public void Awake() {

			playQeue = new List<string>();

			crossfade = new CrossFadeData();
			crossfade.toState = null;
			crossfade.weights = new Dictionary<string, float>();

			animator = gameObject.AddComponent<Animator>();
			animatePhysics = _animatePhysics;//Update the update mode....
			animator.cullingMode = cullingMode;
			//animator.hideFlags = HideFlags.HideInInspector;//Animator component should not be modifiable by users, so hide it.

			Avatar avatar = AvatarBuilder.BuildGenericAvatar(gameObject, string.Empty);

			avatar.name = "QuickClipAvatar." + gameObject.name + "." + gameObject.GetInstanceID();
			animator.avatar = avatar;

			animationStates = new List<AnimationState>();
			namesAndAnimationStates = new Dictionary<string, AnimationState>();
			splitClipPlayables = new List<SplitClipPlayable>();
			namesAndSplitClipPlayables = new Dictionary<string, SplitClipPlayable>();
			blendStates = new List<BlendStateData>();
			namesAndBlendStates = new Dictionary<string, BlendStateData>();


			playableGraph = PlayableGraph.Create("QuickClipGraph." + gameObject.name + "." + gameObject.GetInstanceID());
			output = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);

			mixer = AnimationMixerPlayable.Create(playableGraph);

			output.SetSourcePlayable(mixer);

			//Create a state for the default clip
			AddClip(clip);

			if (animations != null) {
				for (int i = 0; i < animations.Count; i++) {
					//Create a state for each subsiquent clip elsewise.
					AddClip(animations[i]);
				}
			}

			//Sets the base graph playing. This is needed to get the thing animating at all.
			playableGraph.Play();

			if (clip != null && playAutomatically)
				Play(clip.name);
		}

		public AnimationState this[string name] { 
			get {
				if (namesAndAnimationStates == null || namesAndAnimationStates.Count == 0)
					return null;
					
				AnimationState state = null;
				namesAndAnimationStates.TryGetValue(name, out state);
				return state;
			}
		}

		public void Play(string stateName) {
			AnimationState state = null;
			namesAndAnimationStates.TryGetValue(stateName, out state);
			if (state != null) {

				ClearCrossfade();

				for (int i = 0; i < animationStates.Count; i++) {
					animationStates[i].isPlaying = false;
					mixer.SetInputWeight(animationStates[i].mixerIndex, 0f);
				}

				state.isPlaying = true;
				state.time = 0;
				mixer.SetInputWeight(state.mixerIndex, 1f);

				playQeue.Clear();
			}
		}

		public void CrossFade(string stateName, float duration) {
			AnimationState state = null;
			namesAndAnimationStates.TryGetValue(stateName, out state);
			if (state != null && state != crossfade.toState) {

				ClearCrossfade();

				for (int i = 0; i < animationStates.Count; i++) {
					float currentWeight = mixer.GetInputWeight(animationStates[i].mixerIndex);
					crossfade.weights.Add(animationStates[i].name, currentWeight);
				}

				state.time = 0;
				state.isPlaying = true;
				crossfade.toState = state;
				crossfade.duration = duration;

				playQeue.Clear();
			}
		}

		public void Blend(string animation, float targetWeight = 1.0f, float fadeLength = 0.3f) {
			//Set the weight of this animation to the target weight over duration of fade length
			AnimationState state = null;
			namesAndAnimationStates.TryGetValue(animation, out state);
			if (state != null) {
				//Kill existing blend state
				if (namesAndBlendStates.ContainsKey(animation)) {
					blendStates.Remove(namesAndBlendStates[animation]);
					namesAndBlendStates.Remove(animation);
				}
				state.isPlaying = true;
				BlendStateData bs = new BlendStateData(animation, targetWeight, fadeLength, mixer.GetInputWeight(state.mixerIndex));
				namesAndBlendStates.Add(animation, bs);
				blendStates.Add(bs);
			}
		}

		//Adds the animation to a qeue list. Will play through the list so long as none of the animations in the list are looping types.
		public void PlayQeued(string name) {
			if (namesAndAnimationStates.ContainsKey(name)) {
				playQeue.Add(name);
			}
		}

		public void Stop(string stateName = "") {
			AnimationState state = null;
			namesAndAnimationStates.TryGetValue(stateName, out state);
			if (state != null) {
				state.isPlaying = false;
				playQeue.Clear();
			} else {
				for (int i = 0; i < animationStates.Count; i++) {
					animationStates[i].isPlaying = false;
					mixer.SetInputWeight(animationStates[i].mixerIndex, 0f);
				}
				playQeue.Clear();
			}
		}

		public bool IsPlaying(string stateName = "") {
			AnimationState state = null;
			namesAndAnimationStates.TryGetValue(stateName, out state);
			if (state != null) {
				return state.isPlaying;
			} else {
				bool isPlaying = false;
				for (int i = 0; i < animationStates.Count; i++) {
					isPlaying |= animationStates[i].isPlaying;
				}
				return isPlaying;
			}
		}

		public void Rewind(string name) {
			AnimationState state = null;
			namesAndAnimationStates.TryGetValue(name, out state);
			if (state != null) {
				state.normalizedTime = 0.0f;
			} else {
				for (int i = 0; i < animationStates.Count; i++) {
					animationStates[i].normalizedTime = 0.0f;
				}
			}
		}

		public void AddClip(AnimationClip clip, string newName = "") {//Adds a clip
			if (clip == null)
				return;

			if (newName == string.Empty)
				newName = clip.name;

			AddClip(clip, newName, 0, UnityEngine.Mathf.CeilToInt(clip.length * clip.frameRate));
		}

		private void AddClip(AnimationClip clip, string name, int firstFrame, int lastFrame, bool addLoopFrame = false) {//Adds a clip
			if (clip.isLooping) {
				clip.wrapMode = WrapMode.Loop;
			}
			int index = -1;
			if (namesAndAnimationStates.ContainsKey(name)) {
				//Overwriting the existing data
				AnimationState oldState = namesAndAnimationStates[name];
				if (oldState.clip == clip)
					return;//Don't overwrite the old clip since it's the same.

				namesAndAnimationStates.Remove(name);
				namesAndSplitClipPlayables.Remove(name);
				animationStates.Remove(oldState);
				namesAndAnimationStates.Remove(name);
				oldState.playable.playable.Destroy();
				index = oldState.mixerIndex;
				oldState.isValid = false;
			}

			ScriptPlayable<SplitClipPlayable> scriptPlayable = ScriptPlayable<SplitClipPlayable>.Create(playableGraph);
			SplitClipPlayable scp = scriptPlayable.GetBehaviour();
			if (index == -1) {
				index = mixer.AddInput(scriptPlayable, 0, 1);
			} else {
				mixer.ConnectInput(index, scriptPlayable, 0);
			}

			AnimationState state = new AnimationState(clip, name, firstFrame, lastFrame, addLoopFrame, scp, index, mixer);

			animationStates.Add(state);
			namesAndAnimationStates.Add(name, state);

			state.mixerIndex = index;
			state.playable = scp;
			scp.Init(state, scriptPlayable, playableGraph);
			splitClipPlayables.Add(scp);
			namesAndSplitClipPlayables.Add(name, scp);

			mixer.SetInputWeight(index, 0);
		}

		public int GetClipCount() {
			return animationStates.Count;
		}

		public AnimationState GetState(string name) {
			AnimationState state = null;
			namesAndAnimationStates.TryGetValue(name, out state);
			return state;
		}

		private void FixedUpdate() {
			if (_animatePhysics)
				InternalUpdate(UnityEngine.Time.fixedDeltaTime);
		}

		private void Update() {
			if (!_animatePhysics)
				InternalUpdate(UnityEngine.Time.deltaTime);
		}

		private void InternalUpdate(float deltaTime) {
			//Dealing with cross fading
			if (crossfade.toState != null) {
				crossfade.time += deltaTime;//UnityEngine.Time.deltaTime;
				float a = crossfade.time / crossfade.duration;
				for (int i = 0 ; i < animationStates.Count; i++) {
					if (crossfade.weights.ContainsKey(animationStates[i].name)) {
						float startWeight = crossfade.weights[animationStates[i].name];
						float endWeight = (animationStates[i] == crossfade.toState)? 1f : 0f;
						mixer.SetInputWeight(animationStates[i].mixerIndex, UnityEngine.Mathf.Lerp(startWeight, endWeight, a));
					}
				}

				if (crossfade.time >= crossfade.duration) {
					Stop();
					crossfade.toState.isPlaying = true;
					mixer.SetInputWeight(crossfade.toState.mixerIndex, 1.0f);
					ClearCrossfade();
				}
			}

			//Dealing with blending
			for (int i = 0; i < blendStates.Count; i++) {
				AnimationState state = namesAndAnimationStates[blendStates[i].stateName];
				mixer.SetInputWeight(state.mixerIndex, blendStates[i].Update(deltaTime));
				if (blendStates[i].isComplete) {
					namesAndAnimationStates.Remove(blendStates[i].stateName);
					blendStates.RemoveAt(i);
					i--;
				}
			}

			//Dealing with qeue
			if (playQeue.Count > 0) {
				string stateName = playQeue[0];
				AnimationState state = namesAndAnimationStates[stateName];

				if (bPlayingQeue) {
					if (state.totalNormalizedTime >= 1 && !state.clip.isLooping) {
						bPlayingQeue = false;
						playQeue.RemoveAt(0);
					}
				}

				if (playQeue.Count > 0) {
					stateName = playQeue[0];
					state = namesAndAnimationStates[stateName];
				} else {return;}

				if (!bPlayingQeue) {
					ClearCrossfade();

					for (int i = 0; i < animationStates.Count; i++) {
						animationStates[i].isPlaying = false;
						mixer.SetInputWeight(animationStates[i].mixerIndex, 0f);
					}

					state.time = 0;
					mixer.SetInputWeight(state.mixerIndex, 1f);
					state.isPlaying = true;
					bPlayingQeue = true;
				}
			} else { bPlayingQeue = false; } 
		}

		private void ClearCrossfade() {
			crossfade.toState = null;
			crossfade.time = 0;
			crossfade.duration = 0;
			crossfade.weights.Clear();
		}

		private void OnValidate() {
			if (animator != null)
				animator.cullingMode = cullingMode;

			animatePhysics = _animatePhysics;
			clip = _clip;
		}

		private void OnDestroy() {
			if (playableGraph.IsValid())
				playableGraph.Destroy();
		}

		private struct CrossFadeData {
			public AnimationState toState;
			public float time;
			public float duration;
			public Dictionary<string, float> weights;
		}

		private class BlendStateData {
			//Blend class for animation blending
			private float start = 0.0f;
			private float target = 0.0f;
			private float duration = 0.0f;
			public string stateName  {get; internal set; }
			private float t = 0.0f;
			public bool isComplete { get { return t >= duration; } }

			public BlendStateData(string state, float targetWeight, float fadeLength, float startWeight) {
				this.stateName = state;
				target = targetWeight;
				duration = fadeLength;
				start = startWeight;
			}

			public float Update(float deltaTime) {
				t += deltaTime;
				float a = t / duration;
				return UnityEngine.Mathf.Lerp(start, target, a);
			}
		}
	}
}