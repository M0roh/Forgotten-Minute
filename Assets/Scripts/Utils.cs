using System.Collections;
#if UNITY_EDITOR
#endif
using UnityEngine;
using UnityEngine.AI;


public partial class Utils
    {
    //public static string sceneToLoad;

    //public static bool IsLoading = false;


    //public static void LoadSceneWithLoadScreen(string sceneName)
    //{
    //    if (!SceneManager.GetSceneByName(sceneName).isLoaded)
    //    {
    //        sceneToLoad = sceneName;
    //        SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Single);
    //    }
    //}

    //public static T[] FindAllScriptableObjectInstances<T>() where T : ScriptableObject
    //{
    //    T[] found = Resources.LoadAll<T>("");

    //    return found;
    //}


    public static Vector3 GetRandomDir()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public static bool IsPointOutsideCamera(Vector3 point)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(point);

        if (viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f)
            return true;

        return false;
    }

    public static IEnumerator GetRandomPointOnNavMesh(Vector3 center, float radiusMax, System.Action<Vector3> onComplete, UnityEngine.AI.NavMeshAgent agent = null, float radiusMin = 0f)
    {
        int maxAttempts = 50;

        NavMeshHit hit;

        int areaMask;
        if (agent != null)
        {
            areaMask = agent.areaMask;
        }
        else
            areaMask = 1 << NavMesh.GetAreaFromName("Walkable");

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitCircle * Random.Range(radiusMin, radiusMax);
            randomDirection += center;

            if (NavMesh.SamplePosition(randomDirection, out hit, radiusMax, areaMask))
            {
                onComplete?.Invoke(hit.position);
                yield break;
            }
            yield return null;
        }

        if (NavMesh.SamplePosition(center, out hit, radiusMax, areaMask))
            onComplete?.Invoke(hit.position);

        throw new System.Exception("Failed to find a valid NavMesh point.");
    }
}
