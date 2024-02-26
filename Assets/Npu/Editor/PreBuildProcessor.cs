using UnityEditor;
using UnityEditor.Build;

public class PreBuildProcessor : IPreprocessBuild
{

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        
    }
}
