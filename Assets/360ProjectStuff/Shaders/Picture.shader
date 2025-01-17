﻿// (c) 2018 Guidev
// This code is licensed under MIT license (see LICENSE.txt for details)

Shader "Huzaifa/360 Picture Shaders/360 Picture Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Enum(CompareFucntion)] _StencilComp("Stencil Comp", Int) = 3
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull front
		Pass
		{
			Stencil{
				Ref 1
				Comp [_StencilComp]
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal: NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal: TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = normalize(mul(v.normal, unity_WorldToObject));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 texCol = tex2D(_MainTex, i.uv);
				return texCol;
			}
			ENDCG
		}
	}
}
