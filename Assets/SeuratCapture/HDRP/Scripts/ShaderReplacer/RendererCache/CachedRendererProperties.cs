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
/// Stores a renderer and a list of it's' material properties 
/// </summary>
[System.Serializable]
public class CachedRendererProperties
{
    //The renderer we are targeting.
    public Renderer renderer;
    //The properties saved for each material
    public MaterialPropertyValues[] materialProperties;
    //The length of materials for fast retrieval
    public int materialsLength;

    public CachedRendererProperties(Renderer renderer)
    {
        //Store the renderer so that it can be used when restoring material values.
        this.renderer = renderer;
        //Get the shared materials, we use property blocks to edit per-instance data.
        Material[] sharedMaterials = renderer.sharedMaterials;
        //Cache the length for faster iteration.
        materialsLength = sharedMaterials.Length;
        //Create a container of cached properties for each material.
        materialProperties = new MaterialPropertyValues[materialsLength];
        for (int i = 0; i < materialsLength; i++)
        {
            //Cache the properties from each material
            materialProperties[i] = new MaterialPropertyValues(sharedMaterials[i]);
        }
    }

    public void SetProperties()
    {
        //Go through each material and assign the cached properties through property blocks.
        for (int i = 0; i < materialsLength; i++)
        {
            materialProperties[i].SetValues(renderer, i);
        }
    }

}