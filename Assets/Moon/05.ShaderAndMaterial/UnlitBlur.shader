Shader "Custom/URPSpriteBlur"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0, 0.05)) = 0.01
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode"="Universal2D" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URP 전용 라이브러리 포함 (UnityCG.cginc를 대체함)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                half4 color         : COLOR;
                float2 uv           : TEXCOORD0;
            };

            // URP 텍스처 샘플링 매크로
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _BlurSize;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // UnityObjectToClipPos 를 대체하는 URP 함수
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                half4 col = half4(0, 0, 0, 0);
                float size = _BlurSize;

                // 9-Tap Box Blur
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-size, -size));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -size));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(size, -size));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-size, 0));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, 0));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(size, 0));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-size, size));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, size));
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(size, size));

                col /= 9.0;
                
                // 스프라이트 외곽 투명도(Alpha) 처리
                col.rgb *= col.a; 

                return col * IN.color;
            }
            ENDHLSL
        }
    }
}