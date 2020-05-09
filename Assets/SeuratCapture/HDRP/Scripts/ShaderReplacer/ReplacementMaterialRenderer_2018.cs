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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_RENDER_PIPELINE_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif
using UnityEngine.Rendering;


//Preforms the material replacement render when using Unity 2018.4.
partial struct MaterialReplacer
{
#if UNITY_RENDER_PIPELINE_HDRP && UNITY_2018_4
     private DrawRendererSettings drawingSettings;

    public MaterialReplacer(ref Material targetMaterial)
    {
        drawingSettings = new DrawRendererSettings();
        drawingSettings.SetShaderPassName(0, new ShaderPassName("DepthOnly"));
        drawingSettings.SetShaderPassName(1, new ShaderPassName("SRPDefaultUnlit"));
        drawingSettings.SetShaderPassName(2, new ShaderPassName("Vertex"));
        drawingSettings.SetOverrideMaterial(targetMaterial, 0); 
    }

    public void Draw(ref ScriptableRenderContext context, ref HDCamera camera)
    {
        var filteringSettings = GetFilteringSettings();
        var cullingResults = GetCullingResults(ref context, ref camera.camera);
        context.DrawRenderers(cullingResults.visibleRenderers, ref drawingSettings, filteringSettings);
    }

    private FilterRenderersSettings GetFilteringSettings()
    {
        FilterRenderersSettings filteringSettings = new FilterRenderersSettings(true)
        {
            renderQueueRange = RenderQueueRange.all
        };
        return filteringSettings;
    }

    private CullResults GetCullingResults(ref ScriptableRenderContext context, ref Camera camera)
    {
        CullResults results = new CullResults();
        CullResults.GetCullingParameters(camera, out ScriptableCullingParameters cullingParameters);
        CullResults.Cull(ref cullingParameters, context, ref results);
        return results;
    }
#endif
}
