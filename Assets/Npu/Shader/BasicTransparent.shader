Shader "Npu/BasicTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
	_Index("Index", int) = 0

        [Header(Lighting)]
        _LitFactor ("Lit factor", Range(0, 1)) = 0.5
        _LitAdd ("Lit add", Range(-1, 1)) = 0
        _LitColor ("Lit color", Color) = (1,1,1,0)
        _ShadowFactor ("Shadow factor", Range(0, 1)) = 0.25
        _ShadowColor ("Shadow color", Color) = (0,0,0,1)

        [Header(Special)]
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 2
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase"
        }
        LOD 100
        Cull [_Culling]

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile_instancing
            #include "AutoLight.cginc"

            struct vertdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half3 diff : TEXCOORD1;
                half3 ambient : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half4 _Color;
            half _LitFactor;
            half _LitAdd;
            half4 _LitColor;
            half _ShadowFactor;
            half4 _ShadowColor;
	    int _Index;
            
            uniform half4 _GlobalTint; 
            uniform int _GlobalIndex; 
            

            v2f vert(vertdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.diff = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.ambient = ShadeSH9(half4(worldNormal, 1));
                TRANSFER_SHADOW(o)

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 tint = _Index < 0 || _GlobalIndex == _Index ? _Color : _Color * _GlobalTint;
                half shadow = SHADOW_ATTENUATION(i);
                half3 lighting = i.diff * shadow + i.ambient;
                half4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw) * tint;

                col.rgb = lerp(col, col * (lighting + _LitAdd) * lerp(_LightColor0, _LitColor, _LitColor.a),
                               _LitFactor).rgb;
                col.rgb = lerp(col, _ShadowColor, _ShadowFactor * (1 - i.diff)).rgb;

                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}