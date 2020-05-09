// Copyright(c) 2016 Unity Technologies
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files(the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and / or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Google modifications:
// * This comment.
// * Add a full copy of the license in this file.
// * Rename shader to disambiguate from the Unity internal shader.
// * Replace window space depth generation with eye space depth.
// * Emit depth in single channel; require float precision render target and
//   readback.

// Krystian modifications:
// * This comment.
// * Renamed shader properties to match the HDRP/Lit properties.
// * Added properties to match HDRP/Lit shader.

Shader "GoogleVR/Seurat/HDRPCaptureEyeDepth" {
	Properties{
		[HideInInspector]_BaseColorMap("", 2D) = "white" {}
		[HideInInspector]_AlphaCutoffEnable("Alpha Cutoff Enable", Float) = 0.0
		[HideInInspector]_AlphaCutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[HideInInspector]_Color("", Color) = (1,1,1,1)
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			Pass {

		Cull Off
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 nz : TEXCOORD1;
			UNITY_VERTEX_OUTPUT_STEREO
		};
			  uniform float4 _BaseColorMap_ST;

		v2f vert(appdata_base v) {
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			o.uv = TRANSFORM_TEX(v.texcoord, _BaseColorMap);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.nz.xyz = COMPUTE_VIEW_NORMAL;
			COMPUTE_EYEDEPTH(o.nz.w);
			return o;
		}
		uniform sampler2D _BaseColorMap;
		uniform fixed _AlphaCutoffEnable;
		uniform fixed _AlphaCutoff;

		fixed4 frag(v2f i) : SV_Target{
		half alpha = tex2D(_BaseColorMap, i.uv).a;
			if (_AlphaCutoffEnable > 0)
			clip(alpha - _AlphaCutoff);
			return float4(i.nz.w, i.nz.xy, 1.0);
		}
		 ENDCG
			}
		}
Fallback Off
}
