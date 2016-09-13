Shader "Custom/Image Effects/TransitionImageEffect"
{
	Properties
	{
		//Screen texture
		_MainTex ("Texture", 2D) = "white" {}

		//Transition values
		_Color("Transition Color", Color) = (0,0,0,1)
		_TransitionTexture("Transition Texture", 2D) = "white" {}
		_Cutoff ("Cut-off", float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				//Standard draw vertex
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv;

				//Fix platform issues
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y;
				#endif

				return o;
			}
			
			sampler2D _MainTex;

			float4 _Color;
			sampler2D _TransitionTexture;
			float _Cutoff;

			fixed4 frag (v2f i) : SV_Target
			{
				//Transition texture
				float4 transit = tex2D(_TransitionTexture, i.uv);

				//If value is below cutoff, show the transition color
				if (transit.b < _Cutoff)
					return _Color;

				//Otherwise show the screen texture
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
