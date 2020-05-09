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

//Component that adds a custom render event to the camera data. A work around for the "render with shader" function that doesn't work in HDRP.
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OverrideMaterialRenderer : MonoBehaviour
{
#if UNITY_RENDER_PIPELINE_HDRP

    [Tooltip("Enables the shader replacer for the attached camera. Is disabled by disabling the component ")]
    public bool PreviewRender;

    //***Values have to be serialized so they are saved in and out of edit mode***//

    // Condition to make sure we don't subscribe to the render event more than once.
    [SerializeField]
    [HideInInspector]
    private bool hasData;

    //Keeps track of the renderers and their material properties.
    [SerializeField]
    [HideInInspector]
    private CachedSceneRenderers cachedSceneRenderers;

    //Reference the additional camera data from the HD renderer.
    [SerializeField]
    [HideInInspector]
    private HDAdditionalCameraData hdAdditionalCameraData;

    //The shader path for the normals shader with HDRP support.
    private string shaderPath = "GoogleVR/Seurat/HDRPCaptureEyeDepth";

    //The material we are replacing with.
    private Material targetMaterial; 

    private void OnEnable()
    {
        if (PreviewRender)
        {
            EnableOverride();
        }
    }

    public void EnableOverride()
    {
        if (hasData)
            return;

        InitializeCameraData();
        hdAdditionalCameraData.customRender += CustomMaterialRenderer;
        hasData = true;
    }

    private void OnValidate()
    {
        InitializeCameraData();
    }

    private void InitializeCameraData()
    {
        //Check if we have the HD camera data component cached.
        if (hdAdditionalCameraData == null)
        {
            //If we don't find it on our game object.
            HDAdditionalCameraData hdCameraData = GetComponent<HDAdditionalCameraData>();
            //if it doesn't exist.
            if (hdCameraData == null)
            {
                //Add it to our object.
                hdCameraData = gameObject.AddComponent<HDAdditionalCameraData>();
            }
            //Assign the new value
            hdAdditionalCameraData = hdCameraData;
        }
    }

    public void OnDisable()
    {
        if (!hasData)
            return;

        InitializeCameraData();
        hdAdditionalCameraData.customRender -= CustomMaterialRenderer;
        hasData = false;
    }


    public void CustomMaterialRenderer(ScriptableRenderContext context, HDCamera hdCamera)
    {
        //Initialize the values for the scene renderers and override material.
        Initialize(ref cachedSceneRenderers, ref targetMaterial);

        //Pool a command buffer instead of creating one each frame.
        CommandBuffer cameraBuffer = CommandBufferPool.Get("Render Camera");

        //This function sets up view, projection and clipping planes global shader variables.
        context.SetupCameraProperties(hdCamera.camera);

        //Identifier when measuring CPU and GPU time spent on this command buffer.
        cameraBuffer.BeginSample("Render Camera");

        //Clear the values on the render target if any
        cameraBuffer.ClearRenderTarget(true, true, Color.clear);

        //Execute and clear the command buffer.
        ExecuteCommandBuffer(ref context, ref cameraBuffer);

        //Create material replacement settings.
        MaterialReplacer replacementMaterialRenderer = new MaterialReplacer();
        replacementMaterialRenderer.ReplacementMaterialRenderer(ref targetMaterial);

        //Restore the previous material properties on the replaced material.
        cachedSceneRenderers.SetCachedProperties();

        //Draw all of the objects with the replaced material.
        replacementMaterialRenderer.Draw(ref context,ref hdCamera);

        //Draw the skybox.
        context.DrawSkybox(hdCamera.camera);

        //End the sample.
        cameraBuffer.EndSample("Render Camera");

        //Execute and clear the command buffer.
        ExecuteCommandBuffer(ref context, ref cameraBuffer);

        //Submit the custom render to be rendered
        context.Submit();
    }

    private void Initialize(ref CachedSceneRenderers sceneRenderers, ref Material material)
    {
        //Creates the override material if one has not been created.
        if (material == null)
        {
            //Find the shader  
            Shader depthShader = Shader.Find(shaderPath);
            if (depthShader == null)
            {
                Debug.LogError("Could not find shader ");
                depthShader = Shader.Find("Hidden/InternalErrorShader");
            }
            //Create the material
            material = new Material(depthShader)
            {
                //The material should never be saved.
                hideFlags = HideFlags.HideAndDontSave
            };
        }
        //Creates a reference to the scene renderers if it doesn't exist This assumes that renderers will not be enabled/disabled between captures.
        if (sceneRenderers == null)
        {
            sceneRenderers = new CachedSceneRenderers();
            sceneRenderers.GetRenderers();
        }
    }

    private void ExecuteCommandBuffer(ref ScriptableRenderContext context,ref CommandBuffer cameraBuffer)
    {
        context.ExecuteCommandBuffer(cameraBuffer);
        cameraBuffer.Clear();
    }
#endif

}
