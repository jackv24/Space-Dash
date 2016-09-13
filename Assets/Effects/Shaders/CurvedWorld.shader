// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Curved World"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM

		#pragma surface surf Standard vertex:vert

		sampler2D _MainTex;
		float4 _Color;

		float _Curvature;
		float3 _TargetPos;

		struct Input
		{
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		//Manipulate vertices
		void vert(inout appdata_full v)
		{
			//Get vertex world coordinates
			float4 worldV = mul(unity_ObjectToWorld, v.vertex);

			//Get target coordinates relative to vertex world
			worldV.xyz -= _TargetPos.xyz;

			//Transform vertex based on x distance from target
			worldV = float4(0.0f, (worldV.x * worldV.x) * -_Curvature, 0.0f, 0.0f);

			//Add this offset to vertex
			v.vertex += mul(unity_WorldToObject, worldV);
		}

		//Draw surface shader
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Albedo *= _Color.rgb;
		}

		ENDCG
	}

	Fallback "Diffuse"
}