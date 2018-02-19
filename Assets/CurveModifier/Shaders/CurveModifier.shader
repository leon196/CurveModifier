// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CurveModifier" 
{
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float curveRatio : TEXCOORD1;
				float3 color : COLOR;
			};

			sampler2D _MainTex;
			sampler2D _CurveTexture;
			sampler2D _CurveVectorUpTexture;
			float4 _MainTex_ST;
			fixed4 _Color;
			float3 _Forward;
			float3 _Up;
			float3 _Right;
			float _CurveResolution;
			float _ShouldInverse;
			float _ShouldLoop;
			float _CycleOffset;
			float _CycleTime;
			float _CurveScale;
			float _PlanarScale;

			// this function set the vertex position
			v2f vert (appdata v) 
			{
				v2f o;

				// use transform component
				float4 vertex = mul(unity_WorldToObject, v.vertex);

				// axis setup (compress vectors into scalars)
				float vertexForward = vertex.x * _Forward.x + vertex.y * _Forward.y + vertex.z * _Forward.z;
				float vertexRight = vertex.x * _Right.x + vertex.y * _Right.y + vertex.z * _Right.z;
				float vertexUp = vertex.x * _Up.x + vertex.y * _Up.y + vertex.z * _Up.z;

				// the actual clamped ratio position on the curve
				float ratio = fmod(abs(vertexForward * _CurveScale + _CycleTime + _CycleOffset), 1.0);
				ratio = lerp(ratio, 1.0 - ratio, _ShouldInverse);

				// used to distribute point on the plane that is perpendicular to the curve forward
				float angle = atan2(vertexUp, vertexRight);
				float radius = length(float2(vertexRight * _PlanarScale, vertexUp * _PlanarScale));

				// get current point through the texture
				float4 p = float4(ratio, 0.0, 0.0, 0.0);
				float3 bezierPoint = mul(unity_WorldToObject, tex2Dlod(_CurveTexture, p));

				// get neighbors of the current ratio
				float unit = 1.0 / _CurveResolution;
				float next = fmod(ratio + unit, 1.0);
				float prev = fmod(ratio - unit + 1.0, 1.0);
				float ratioNext = lerp(next, prev, _ShouldInverse);
				float ratioPrevious = lerp(prev, next, _ShouldInverse);

				// get next and previous point through the texture
				p.x = ratioNext;
				float3 bezierPointNext = mul(unity_WorldToObject, tex2Dlod(_CurveTexture, p));
				p.x = ratioPrevious;
				float3 bezierPointPrevious = mul(unity_WorldToObject, tex2Dlod(_CurveTexture, p));

				// find out vectors
				float3 forward = normalize(bezierPointNext - bezierPoint);
				// float3 backward = normalize(bezierPointPrevious - bezierPoint);
				float3 up = normalize(cross(normalize(bezierPoint), normalize(bezierPointNext)));
				// float3 up = normalize(cross(forward, backward));
				float3 right = normalize(cross(forward, up));

				// voila
				vertex.xyz = bezierPoint + right * cos(angle) * radius + up * sin(angle) * radius;

				// unity stuff
				o.curveRatio = lerp(ratio, -1.0, step(1.0, ratio + unit));
				o.curveRatio = lerp(o.curveRatio, -1.0, step(ratio - unit, 0.0));
				o.color = up;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			// this function give the pixel color
			fixed4 frag (v2f i) : SV_Target 
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				float unit = 1.0 / _CurveResolution;
				if (i.curveRatio < 0.0) {
					clip(_ShouldLoop - 0.5);
				}
				// return col;
				return col;// * fixed4(i.color * 0.5 + 0.5, 1.0);
			}
			ENDCG
		}
	}
}
