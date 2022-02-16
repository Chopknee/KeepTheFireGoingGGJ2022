using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Dugan.Animation {
	public class SplitClipPlayable : PlayableBehaviour {
		public Playable playable;
		private Playable mixer;
		private AnimationState animationState;
		private PlayableGraph graph;

		private Playable clipA;
		//private Playable clipB;

		public void Init(AnimationState animationState, Playable owner, PlayableGraph graph) {
			playable = owner;
			owner.SetInputCount(1);
			mixer = AnimationMixerPlayable.Create(graph, 2);
			graph.Connect(mixer, 0, owner, 0);
			owner.SetInputWeight(0, 1);

			graph.Connect(AnimationClipPlayable.Create(graph, animationState.clip), 0, mixer, 0);
			mixer.SetInputWeight(0, 1.0f);
			clipA = mixer.GetInput(0);
			clipA.SetTime(animationState.startTime);
			
			// graph.Connect(AnimationClipPlayable.Create(graph, animationState.clip), 0, mixer, 1);
			// mixer.SetInputWeight(0, 1.0f);
			// clipB = mixer.GetInput(1);
			// clipB.SetTime(animationState.startTime);

			this.animationState = animationState;
			this.graph = graph;
		}

		public override void PrepareFrame(Playable playable, FrameData info) {
			WrapMode mode = (animationState.wrapMode == WrapMode.Default)? animationState.clip.wrapMode : animationState.wrapMode;
			switch (mode) {
				// case WrapMode.Loop:
				// 	if (animationState.totalNormalizedTime >= 1.0f) {
				// 		animationState.normalizedTime = 0.0f;//This is where the special loop time stuff comes in handy.
				// 	}
				// 	break;

				case WrapMode.ClampForever:
					if (animationState.totalNormalizedTime >= 1.0f) {
						playable.Pause();
						animationState.totalNormalizedTime = 1.0f;
					}
					break;

				case WrapMode.Once:
					if (animationState.totalNormalizedTime >= 1.0f) {
						playable.Pause();
						animationState.totalNormalizedTime = 0.0f;
					}
					break;
				
				case WrapMode.Default:
					if (animationState.totalNormalizedTime >= 1.0f) {
						playable.Pause();
						animationState.totalNormalizedTime = 0.0f;
					}
					break;

				case WrapMode.PingPong://Hmmm....
					break;
			}
			double time = playable.GetTime();
			mixer.SetTime(time);
			clipA.SetTime(time);
			//clipB.SetTime(time);

			double speed = playable.GetSpeed();
			mixer.SetSpeed(speed);
			clipA.SetSpeed(speed);
			//clipB.SetSpeed(speed);

			if (animationState.normalizedTime < 0.0f)
					animationState.normalizedTime = 0.0f;
		}
	}
}
