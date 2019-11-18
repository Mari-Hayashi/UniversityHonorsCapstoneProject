using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class BuildGenerator
{
    [PostProcessBuild]
    public static void PostBuildUpdates(BuildTarget buildTarget, string pathToBuildProject)
    {
        switch (buildTarget)
        {
            case BuildTarget.iOS:
                {
                    //PList Keys
                    //Get plist
                    PlistDocument plist = new PlistDocument();
                    plist.ReadFromFile($"{pathToBuildProject}/Info.plist");

                    //iTunes Sharing Key
                    plist.root.SetBoolean("UIFileSharingEnabled", true);
                    //nonExemptEncryptionKey
                    plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);

                    //Write plist
                    plist.WriteToFile($"{pathToBuildProject}/Info.plist");

                    
                    /* We do not ned to strip the bitcode, but here is how it could be done:
                    //Strip Bitcode
                    //Open Project
                    string projectPath = $"{pathToBuildProject}/Unity-iPhone.xcodeproj/project.pbxproj";

                    PBXProject pbxProject = new PBXProject();
                    pbxProject.ReadFromFile(projectPath);

                    string target = pbxProject.TargetGuidByName("Unity-iPhone");
                    pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "false");

                    //Save Project
                    pbxProject.WriteToFile(projectPath);
                    */
                    
                }
                break;

            default:
                //Nothing to add
                break;
        }
    }
}
