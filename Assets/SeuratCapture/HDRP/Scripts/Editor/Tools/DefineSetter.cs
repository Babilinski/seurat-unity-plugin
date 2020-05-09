/*
Copyright 2019 Krystian Babilinski All Rights Reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

/// <summary>
/// Creates a defines symbol for the HD render pipeline if it is detected in the graphics settings.
/// </summary>
[InitializeOnLoad]
sealed class DefineSetter
{
    private const string Define = "UNITY_RENDER_PIPELINE_HDRP";

    static DefineSetter()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

        if (group != BuildTargetGroup.Standalone)
            return;

        if (!IsHdrpValid())
        {
            RemoveDefinesSymbol(BuildTargetGroup.Standalone);
        }
        else
        {
            AddDefinesSymbol(BuildTargetGroup.Standalone);
        }
    }

    private static void AddDefinesSymbol(BuildTargetGroup target)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

        List<string> list = defines.Split(';', ' ').Where(x => !string.IsNullOrEmpty(x)).ToList();

        if (list.Contains(Define))
            return;

        list.Add(Define);
        defines = list.Aggregate((a, b) => a + ";" + b);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
    }

    private static void RemoveDefinesSymbol(BuildTargetGroup target)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

        List<string> list = defines.Split(';', ' ').Where(x => !string.IsNullOrEmpty(x)).ToList();

        if (!list.Contains(Define))
            return;

        list.Remove(Define);

        defines = list.Aggregate((a, b) => a + ";" + b);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
    }

    private static bool IsHdrpValid()
    {
        var renderAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        return renderAsset != null && renderAsset.GetType().Name.Equals("HDRenderPipelineAsset");
    }

    private static bool IsObsolete(BuildTargetGroup group)
    {
        var attrs = typeof(BuildTargetGroup).GetField(group.ToString())
            .GetCustomAttributes(typeof(ObsoleteAttribute), false);

        return attrs != null && attrs.Length > 0;
    }
}