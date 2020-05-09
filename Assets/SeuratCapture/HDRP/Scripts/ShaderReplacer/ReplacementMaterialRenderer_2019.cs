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

using UnityEngine;
#if UNITY_RENDER_PIPELINE_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif
using UnityEngine.Rendering;

//Preforms the material replacement render when using Unity 2019.1 or higher.
partial struct MaterialReplacer
{
#if UNITY_RENDER_PIPELINE_HDRP && UNITY_2019_1_OR_NEWER
    private DrawingSettings drawingSettings;

    public void ReplacementMaterialRenderer(ref Material targetMaterial )
    {
        drawingSettings = new DrawingSettings();
        drawingSettings.SetShaderPassName(0, new ShaderTagId("DepthOnly"));
        drawingSettings.SetShaderPassName(1, new ShaderTagId("SRPDefaultUnlit"));
        drawingSettings.SetShaderPassName(2, new ShaderTagId("Vertex"));
        drawingSettings.perObjectData = PerObjectData.None;
        drawingSettings.overrideMaterial = targetMaterial;
        drawingSettings.overrideMaterialPassIndex = 0;
    }

    public void Draw(ref ScriptableRenderContext context, ref HDCamera camera)
    {
        camera.camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters);
        CullingResults cullingResults = context.Cull(ref cullingParameters);

        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);

        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

#endif

}