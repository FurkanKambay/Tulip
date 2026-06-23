using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Furkan.Common
{
    [Serializable, PublicAPI]
    public class SceneInfo
    {
        public Scene Scene => SceneManager.GetSceneByBuildIndex(BuildIndex);
        public AsyncOperation AsyncOperation { get; private set; }

        [ShowInInspector] public int BuildIndex { get; }
        [ShowInInspector] public bool AllowAutoActivation => AsyncOperation?.allowSceneActivation ?? false;
        [ShowInInspector] public float Progress => AsyncOperation?.progress ?? -1f;
        [ShowInInspector] public bool IsPreloaded => AsyncOperation?.progress >= 0.9f;
        [ShowInInspector] public bool IsActivated => AsyncOperation?.isDone ?? false;

        public SceneInfo(int buildIndex) =>
            BuildIndex = buildIndex;

        public IEnumerator LoadAsync(LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            yield return SceneManager.LoadSceneAsync(BuildIndex, loadSceneMode);
            yield return null;
        }

        public IEnumerator PreloadAsync(LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            Assert.IsNull(AsyncOperation);
            AsyncOperation = SceneManager.LoadSceneAsync(BuildIndex, loadSceneMode);
            Assert.IsNotNull(AsyncOperation);

            AsyncOperation.allowSceneActivation = false;
            yield return null;
        }

        public IEnumerator ReloadAsync()
        {
            if (Scene.isLoaded)
            {
                AsyncOperation = null;
                yield return SceneManager.UnloadSceneAsync(Scene);
            }

            yield return PreloadAsync();
        }

        public IEnumerator ActivateScene()
        {
            Assert.IsNotNull(AsyncOperation);

            if (IsActivated)
                yield break;

            AllowActivation();
            yield return AsyncOperation;

            // to be safe
            while (!Scene.isLoaded)
                yield return null;

            SceneManager.SetActiveScene(Scene);
        }

        public void AllowActivation()
        {
            if (AsyncOperation != null)
                AsyncOperation.allowSceneActivation = true;
        }

        public IEnumerator WaitUntilPreloaded()
        {
            while (AsyncOperation.progress < 0.9f)
                yield return null;
        }

        public static IEnumerator UnloadAsync(Scene scene)
        {
            yield return SceneManager.UnloadSceneAsync(scene);
            yield return null;
        }
    }
}
