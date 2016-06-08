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
			};

			sampler2D _MainTex;
			sampler2D _CurveTexture;
			float4 _MainTex_ST;
			fixed4 _Color;
			float3 _Forward;
			float3 _Up;
			float3 _Right;
			float _CurveResolution;
			float _ShouldLoop;
			float _TimeSpeed;

			// this function set the vertex position
			v2f vert (appdata v) 
			{
				v2f o;

				// use transform component
				float4 vertex = mul(_Object2World, v.vertex);

				// axis setup (compress vectors into scalars)
				float vertexForward = vertex.x * _Forward.x + vertex.y * _Forward.y + vertex.z * _Forward.z;
				float vertexRight = vertex.x * _Right.x + vertex.y * _Right.y + vertex.z * _Right.z;
				float vertexUp = vertex.x * _Up.x + vertex.y * _Up.y + vertex.z * _Up.z;

				// the actual clamped ratio position on the curve
				float ratio = abs(vertexForward + _Time.x * _TimeSpeed);
				ratio = lerp(clamp(ratio, 0.0, 1.0), fmod(ratio, 1.0), _ShouldLoop);

				// used to distribute point on the plane that is perpendicular to the curve forward
				float angle = atan2(vertexUp, vertexRight);
				float radius = length(float2(vertexRight, vertexUp));

				// get current point through the texture
				float4 p = float4(ratio, 0.0, 0.0, 0.0);
				float3 bezierPoint = mul(_World2Object, tex2Dlod(_CurveTexture, p));

				// get neighbors of the current ratio
				float unit = 1.0 / _CurveResolution;
				float ratioNext = fmod(ratio + unit, 1.0);
				float ratioPrevious = fmod(ratio - unit + 1.0, 1.0);

				// make things loop or not
				ratioNext = lerp(clamp(abs(ratio + unit), 0.0, 1.0), ratioNext, _ShouldLoop);
				ratioPrevious = lerp(clamp(abs(ratio - unit), 0.0, 1.0), ratioPrevious, _ShouldLoop);

				// get next and previous point through the texture
				p.x = ratioNext;
				float3 bezierPointNext = mul(_World2Object, tex2Dlod(_CurveTexture, p));
				p.x = ratioPrevious;
				float3 bezierPointPrevious = mul(_World2Object, tex2Dlod(_CurveTexture, p));

				// find out vectors
				float3 forward = normalize(bezierPointNext - bezierPoint);
				float3 backward = normalize(bezierPointPrevious - bezierPoint);
				float3 up = normalize(cross(forward, backward));
				float3 right = normalize(cross(forward, up));

				// voila
				vertex.xyz = bezierPoint + right * cos(angle) * radius + up * sin(angle) * radius;

				// unity stuff
				o.vertex = mul(UNITY_MATRIX_MVP, vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			// this function give the pixel color
			fixed4 frag (v2f i) : SV_Target 
			{
				// unity stuff
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				return col;
			}
			ENDCG
		}
	}
}
