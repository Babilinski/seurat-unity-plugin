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

/// <summary>
/// Stores a list of material properties and a MaterialPropertyBlock that contains
/// properties that are shared between the google's 'normal' renderer and the HDRP/Lit shader
/// </summary>
[System.Serializable]
public class MaterialPropertyValues
{
    //The material block that will cache all of the material properties values.
    private MaterialPropertyBlock propertyBlock;

    //The property values we are interested, as they will effect how we render the normals.
    private static readonly int BaseColorMap = Shader.PropertyToID("_BaseColorMap");
    private static readonly int AlphaCutoffEnable = Shader.PropertyToID("_AlphaCutoffEnable");
    private static readonly int AlphaCutoff = Shader.PropertyToID("_AlphaCutoff");

    //Cache the material values.
    public MaterialPropertyValues(Material material)
    {
        propertyBlock = new MaterialPropertyBlock();

        Material mat = material;
        if (mat.HasProperty(BaseColorMap))
        {
           var texture = mat.GetTexture(BaseColorMap);
            if (texture != null)
                propertyBlock.SetTexture(BaseColorMap, texture);
        }

        if (mat.HasProperty(AlphaCutoffEnable))
        {
            var cutOffEnabled = mat.GetFloat(AlphaCutoffEnable);
            propertyBlock.SetFloat(AlphaCutoffEnable, cutOffEnabled);
        }

        if (mat.HasProperty(AlphaCutoff))
        {
            var cutOff = mat.GetFloat(AlphaCutoff);
            propertyBlock.SetFloat(AlphaCutoff, cutOff);
        }
    }

    public void SetValues(Renderer renderer, int index)
    {
        renderer.SetPropertyBlock(propertyBlock, index);
    }
}