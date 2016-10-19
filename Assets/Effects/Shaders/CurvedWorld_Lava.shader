// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Curved World (Lava)"
{
	Properties
	{
		_TopTex("Rocks (RGB)", 2D) = "white" {}
		_MainTex ("Lava (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_EmissionColor("Emission Color", Color) = (0,0,0)
		_EmissionMultiplier("Emission Multiplier", Float) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM

		#pragma surface surf Standard vertex:vert addshadow
		
		sampler2D _TopTex;
		sampler2D _MainTex;
		float4 _Color;

		float4 _EmissionColor;
		float _EmissionMultiplier;

		float _Curvature;
		float3 _TargetPos;

		struct Input
		{
			float2 uv_TopTex;
			float2 uv_MainTex;
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
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 topTex = tex2D(_TopTex, IN.uv_TopTex);

			fixed4 mainOutput = mainTex.rgba * (1.0 - topTex.a);
			fixed4 blendOutput = topTex.rgba * topTex.a;

			o.Albedo = mainOutput.rgb + blendOutput.rgb;
			o.Alpha = mainOutput.a + blendOutput.a;
			o.Albedo *= _Color.rgb;

			float4 emission = tex2D(_MainTex, IN.uv_MainTex);
			emission.rgb *= 1 - blendOutput.a;

			o.Emission = emission.rgba;
			o.Emission *= _EmissionColor.rgb * _EmissionMultiplier;
		}

		ENDCG
	}

	Fallback "Diffuse"
}