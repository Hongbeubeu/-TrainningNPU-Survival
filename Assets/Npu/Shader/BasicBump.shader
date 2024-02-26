Shader "Npu/BasicBumped"
{
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}
        _BumpTex ("Bump Texture", 2D) = "black" {}
        _Color ("Color Tint", Color) = (1.0,1.0,1.0,1.0)
        _Index ("Index", int) = 0

        [Header(Light)]
        _LightIntensity ("Light Intensity", Range(0, 1.5)) = 1
        _DiffuseStrength ("Diffuse Factor", Range(0, 1)) = 0.35

        [Header(Shadow)]
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1.0)
        _ShadowStrength ("Shadow Factor", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers flash
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _BumpTex;
            uniform half4 _MainTex_ST;
            uniform half4 _Color;
            uniform half _DiffuseStrength;
            uniform half4 _ShadowColor;
            uniform half _ShadowStrength;
            uniform half _LightIntensity;
            uniform int _Index;

            uniform half4 _GlobalTint;
            uniform int _GlobalIndex;

            struct vertexInput
            {
                half4 vertex : POSITION;
                half4 tangent : TANGENT;
                half3 normal : NORMAL;
                half2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct vertexOutput
            {
                half4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                half3 worldPos : TEXCOORD1;
                half nl : COLOR2;
                half3 vertexNormal : COLOR;
                SHADOW_COORDS(2)
                half3 tspace0 : TEXCOORD5; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD6; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD7; // tangent.z, bitangent.z, normal.z
            };


            vertexOutput vert(vertexInput v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                vertexOutput o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                o.nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));

                o.vertexNormal = v.normal;

                half4 tangent = v.tangent;
                
                half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(worldNormal, wTangent) * tangentSign;
                
                o.tspace0 = half3(wTangent.x, wBitangent.x, worldNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, worldNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, worldNormal.z);

                TRANSFER_SHADOW(o);

                return o;
            }

            half4 frag(vertexOutput i) : COLOR
            {
                // sample the normal map, and decode from the Unity encoding
                half3 tnormal = UnpackNormal(tex2D(_BumpTex, i.uv));
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);

                half4 diffuse = i.nl;
                diffuse.rgb += ShadeSH9(half4(worldNormal, 1));

                half4 tint = _Index < 0 || _GlobalIndex == _Index ? _Color : _Color * _GlobalTint;
                half4 color = tex2D(_MainTex, i.uv) * tint * _LightColor0 * _LightIntensity;
                color = lerp(color, color * diffuse, _DiffuseStrength);

                half shadow = SHADOW_ATTENUATION(i);
                shadow = saturate(1 - lerp(shadow, 1, 1 - i.nl));
                color.rgb = lerp(color.rgb, _ShadowColor, _ShadowStrength * shadow);

                return color;
            }
            ENDCG

        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

    }
    //Fallback "Specular"
}