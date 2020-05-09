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
/// Stores the renderers in the scene and create a RendererProperty for each of them
/// </summary>
[System.Serializable]
public class CachedSceneRenderers
{
    //Values are serailized so they save inside and outside of edit mode.
    [SerializeField]
    [HideInInspector]
    private int numberOfRenderers = 0;
    [SerializeField]
    [HideInInspector]
    private CachedRendererProperties[] cachedRenderers;

    public void GetRenderers()
    {
        //Find all the enabled renderers in the scene.
        Renderer[] renderers = Object.FindObjectsOfType<Renderer>();

        //If the lengths of the arrays are different, reinitialize the entire array.
        if (numberOfRenderers != renderers.Length)
        {
            numberOfRenderers = renderers.Length;
            cachedRenderers = new CachedRendererProperties[numberOfRenderers];
        }
        //Go through each renderer and cache it's materials and it's material properties.
        for (int i = 0; i < numberOfRenderers; i++)
        {
            cachedRenderers[i] = new CachedRendererProperties(renderers[i]);
        }
    }

    public void SetCachedProperties()
    {
        //Go through each renderer and restore the properties that we cached
        for (int i = 0; i < numberOfRenderers; i++)
        {
            cachedRenderers[i].SetProperties();
        }
    }
}