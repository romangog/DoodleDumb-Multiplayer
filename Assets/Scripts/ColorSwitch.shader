Shader "Unlit/ColorSwitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Active("Active Color", Color) = (1,1,1,1)
        _Background("Background Color", Color) = (1,1,1,1)
        _HeightMin("HeightMin", float) = 0
        _HeightMax("HeightMax", float) = 1
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Active;
            fixed4 _Background;
            float _HeightMin;
            float _HeightMax;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                if (i.uv.y > _HeightMax || i.uv.y < _HeightMin)
                    return col;
                return fixed4(
                    lerp(_Active.r, _Background.r, col.r),
                    lerp(_Active.g, _Background.g, col.g),
                    lerp(_Active.b, _Background.b, col.b),
                    lerp(_Active.a, _Background.a, col.a)
                );
                if (col.r > 0.0)
                    return _Background;
                else return _Active;
                return col;
            }
            ENDCG
        }
    }
}
