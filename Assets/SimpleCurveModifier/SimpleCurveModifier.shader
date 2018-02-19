Shader "Unlit/SimpleCurveModifier"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Speed ("Speed", Float) = 1
		_Length ("Length", Float) = 1
		_Radius ("Radius", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct attributes
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct varying
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			uniform sampler2D _MainTex, _CurveTexture;
			uniform float4 _MainTex_ST;
			uniform float _Speed, _Length, _Radius;
			
			varying vert (attributes v)
			{
				varying o;

				o.position = mul(UNITY_MATRIX_M, v.position);
				
				float current = fmod(o.position.y * _Length + _Time.y * _Speed, 1.);
				float next = fmod(current + .01, 1.);

				float3 curvePosition = tex2Dlod(_CurveTexture, float4(current,0,0,0)).xyz;
				float3 curvePositionNext = tex2Dlod(_CurveTexture, float4(next,0,0,0)).xyz;

				float3 forward = normalize(curvePositionNext - curvePosition);
				// float3 up = normalize(cross(float3(0,1,0), forward));
				float3 up = normalize(cross(normalize(curvePositionNext), normalize(curvePosition)));
				float3 right = normalize(cross(forward, up));

				float angle = atan2(o.position.z, o.position.x);
				float radius = length(o.position.xz) * _Radius;
				o.position.xyz = curvePosition + (right * cos(angle) + up * sin(angle)) * radius;

				o.position = mul(UNITY_MATRIX_VP, o.position);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (varying i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				return color;
			}
			ENDCG
		}
	}
}
