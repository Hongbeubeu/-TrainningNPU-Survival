Shader "Npu/Blur" 
{
    Properties
    {
        [PerRendererData] _MainTex("Base (RGB)", 2D) = "white" { }
        //_Color ("Color", Color) = (1, 1, 1, 1)
        _Width ("Width", Range(0.0, 10)) = 1
        //_Strength ("Strength", Range(1, 100)) = 1
    }

    SubShader
    {
        ZTest Always 
        Cull Off 
        ZWrite Off 
        Fog { Mode Off }
        Blend Off // SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half4 _Color;
            half _Width;
            half _Strength;
            float4 _MainTex_TexelSize;
           
            struct v2f 
            {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
            };

            float4 _MainTex_ST;
            float4 _MainTex_ST_TexelSize;

            v2f vert(appdata_base v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float step_w = _MainTex_TexelSize.x * _Width;
                float step_h = _MainTex_TexelSize.y * _Width;

                float2 offset3x3[9] = 
                {
                    float2(-step_w, -step_h),      float2(0.0, -step_h),     float2(step_w, -step_h),
                    float2(-step_w, 0.0),          float2(0.0, 0.0),         float2(step_w, 0.0),
                    float2(-step_w, step_h),       float2(0.0, step_h),      float2(step_w, step_h),
                };

                float kernel3x3[9] = 
                {

                    0.0625, 0.125, 0.0625,
                    0.125 , 0.25, 0.125,
                    0.0625, 0.125, 0.0625

                };

                float4 sum = float4(0.0, 0.0, 0.0, 0.0);

                for (int j = 0; j < 9; j++) 
                {
                    float4 tmp = tex2D(_MainTex, i.uv + offset3x3[j]);
                    sum += tmp * kernel3x3[j];
                }

                //sum.a = atan(_Strength * sum.a) * 2 / 3.141592;

                return sum;
            }

        ENDCG //Shader End
        }

    }

}