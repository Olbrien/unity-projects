using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistanteObjectSpawner : MonoBehaviour
    {
        public GameObject persistantObjectPrefab;

        static bool hasSpawned = false;

        void Awake()
        {
            if (hasSpawned) return;
            SpawnPersistentObjects();

            hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(persistantObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }

}
