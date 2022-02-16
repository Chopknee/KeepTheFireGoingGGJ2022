using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.Input {
    public class Raycaster : MonoBehaviour {

        private static Raycaster instance = null;
        private List<Camera> cameras = null;

		public bool bForceCameraRefresh = false;

		public static QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        public static Raycaster Instance() {
            return instance;
        }

        private void Awake() {
            if (instance == null)
                instance = this;
            cameras = new List<Camera>();
			bForceCameraRefresh = true;
        }

		public List<Vector3> hits = new List<Vector3>();

        public void ManualUpdate() {
			bool bCamerasNeedRefreshed = false;
			//First determine if the cameras array needs to be updated
			for (int i = 0; i < cameras.Count; i++) {
				if (cameras[i] == null)
					bCamerasNeedRefreshed = true;
			}

			bCamerasNeedRefreshed |= bForceCameraRefresh || cameras.Count != Camera.allCamerasCount;
			bForceCameraRefresh = false;

			if (bCamerasNeedRefreshed) {
				Camera[] currentCameras = Camera.allCameras;
				cameras.Clear();
				for (int i = 0; i < currentCameras.Length; i++) {
					if (currentCameras[i].depth < 0)//Ignoring camera depth values less than 0
						continue; 

					if (cameras.Count == 0) {
						cameras.Add(currentCameras[i]);
						continue;
					}

					for (int ii = 0; ii < cameras.Count; ii++) {
						if (cameras[ii].depth < currentCameras[i].depth) {
							cameras.Insert(ii, currentCameras[i]);
							break;
						}
					}
				}
				// string cams = string.Empty;
				// for (int i = 0; i < cameras.Count; i++) {
				// 	cams += cameras[i].depth + ",";
				// }
				// Debug.Log(cams);
			}
			hits.Clear();

            //Raycasting on the camera stack
            for (int pointerIndex = 0; pointerIndex < Dugan.Input.PointerManager.GetPointerCacheCount(); pointerIndex++) {

                Dugan.Input.Pointers.Pointer pointer = Dugan.Input.PointerManager.GetPointerByIndex(pointerIndex);

                //Skip over dead or idle pointers
                if (!pointer.active)
                    continue;

                for (int cameraIndex = 0; cameraIndex < cameras.Count; cameraIndex++) {
                    Camera cam = cameras[cameraIndex];

                    if (Physics.Raycast(cam.ScreenPointToRay(pointer.position), out RaycastHit hit, cam.farClipPlane, cam.cullingMask, queryTriggerInteraction)) {
						hits.Add(hit.point);
                        //The initial hit
						//Debug.Log("Hit collider " + hit.collider.name);
						if (pointer.pointerTarget == null) {//If the old pointer target is null, assign the current hit.
							pointer.pointerTarget = hit.transform.GetComponent<PointerTarget>();
							if (pointer.pointerTarget != null)
								pointer.pointerTarget.UpdateTarget(pointer);
						} else {
							if (pointer.pointerTarget.transform != hit.transform) {
								pointer.pointerTarget = hit.transform.GetComponent<PointerTarget>();
								if (pointer.pointerTarget != null)
									pointer.pointerTarget.UpdateTarget(pointer);
							} else {
								pointer.pointerTarget.UpdateTarget(pointer);
							}
						}
                        break;
                    }
                }
            }
        }

		// private void OnDrawGizmos() {
		// 	for (int i = 0; i < hits.Count; i++) {
		// 		Gizmos.DrawCube(hits[i], Vector3.one * 250);
		// 	}
		// }

        private void Init() {
            instance = this;
        }
    }
}