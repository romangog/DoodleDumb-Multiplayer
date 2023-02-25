Shader "Unlit/DynamicRainbow"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
			_HeightMin("HeightMin", float) = 0
		_HeightMax("HeightMax", float) = 1
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_ST;
			sampler2D _MainTex;
			float _HeightMin;
			float _HeightMax;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}


			float3 rgb2hsv(float3 rgb)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(rgb.bg, K.wz), float4(rgb.gb, K.xy), step(rgb.b, rgb.g));
				float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			float3 hsv2rgb(float3 hsv)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
				return hsv.z * lerp(K.xxx, saturate(p - K.xxx), hsv.y);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float sin1 = frac((_Time) * 1.5 + i.uv.y / 10.0);
				float sin2 = frac((_Time + 0.5 )*1.5 + i.uv.y/10.0);
				float3 active = hsv2rgb(float3(sin1, 1, 1));
				float3 background = hsv2rgb(float3(sin2, 1, 1));
				UNITY_APPLY_FOG(i.fogCoord, col);

				fixed4 activeFix = fixed4(active.rgb, 1);
				fixed4 backgroundFix = fixed4(background.rgb, 1);

				if (i.uv.y > _HeightMax || i.uv.y < _HeightMin)
					return col;

				return fixed4(
					lerp(activeFix.r, backgroundFix.r, col.r),
					lerp(activeFix.g, backgroundFix.g, col.g),
					lerp(activeFix.b, backgroundFix.b, col.b),
					lerp(activeFix.a, backgroundFix.a, col.a)
				);

				if (col.r > 0.9)
					return activeFix;
				else return backgroundFix;
			}
		ENDCG
		}
	}
}
