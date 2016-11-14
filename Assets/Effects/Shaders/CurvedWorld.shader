// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Curved World"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionMap("Emission", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
		_EmissionMultiplier("Emission Multiplier", Float) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM

		#pragma surface surf Lambert vertex:vert addshadow

		sampler2D _MainTex;
		float4 _Color;

		sampler2D _BumpMap;

		sampler2D _EmissionMap;
		float4 _EmissionColor;
		float _EmissionMultiplier;

		float _Curvature;
		float3 _TargetPos;

		struct Input
		{
			float2 uv_MainTex;
			float4 color : COLOR;

			float2 uv_BumpMap;
			float2 uv_EmissionMap;
		};

		//Manipulate vertices
		void vert(inout appdata_full v)
		{
			//Get vertex world coordinates
			float4 worldV = mul(unity_ObjectToWorld, v.vertex);

			//Get target coordinates relative to vertex world
			worldV.xyz -= _TargetPos.xyz;

			//Transform vertex based on x distance from target
			worldV = float4(0.0f, ((worldV.z * worldV.z) + (worldV.x * worldV.x)) * -_Curvature, 0.0f, 0.0f);

			//Add this offset to vertex
			v.vertex += mul(unity_WorldToObject, worldV);
		}

		//Draw surface shader
		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Albedo *= _Color.rgb;

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			o.Emission = tex2D(_EmissionMap, IN.uv_EmissionMap);
			o.Emission *= _EmissionColor.rgb * _EmissionMultiplier;
			
		}

		ENDCG
	}

	Fallback "Diffuse"
}