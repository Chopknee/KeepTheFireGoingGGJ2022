using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.Input {
    public class InputSystem : MonoBehaviour {

        private Dugan.Input.PointerManager pointersRef = null;
        private Dugan.Input.Raycaster raycasterRef = null;

        private static InputSystem instance = null;

        private void EarlyUpdate() {
            pointersRef.ManualUpdate();
            raycasterRef.ManualUpdate();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {

            if (instance != null)
                return;

            GameObject inputManager = new GameObject("Dugan.InputSystem");
            instance = inputManager.AddComponent<Dugan.Input.InputSystem>();

            instance.pointersRef = inputManager.AddComponent<Dugan.Input.PointerManager>();
            instance.raycasterRef = inputManager.AddComponent<Dugan.Input.Raycaster>();

            DontDestroyOnLoad(inputManager);

			PreScriptExecution.EarlyUpdate += instance.EarlyUpdate;
        }

    }
}