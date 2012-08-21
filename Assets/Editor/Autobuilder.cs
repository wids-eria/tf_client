using UnityEditor;
using System.Collections;

public class Autobuilder {

	static void PerformBuild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSXUniversal);
		string [] scenes = {@"Assets/Build Structure/Levels/Login.unity", @"Assets/Build Structure/Levels/Game Level.unity"};
		BuildPipeline.BuildPlayer(scenes, "TrailsForwardAutoBuild.app", BuildTarget.StandaloneOSXIntel, BuildOptions.None);
	}
}
