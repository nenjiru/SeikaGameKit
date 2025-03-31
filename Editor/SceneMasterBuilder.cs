#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class SceneMasterBuilder : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        SeikaGameKit.SceneManagement.SceneMaster.Instance?.ConvertSceneGuidsToNames();
    }
}
#endif