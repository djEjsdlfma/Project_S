Shader "Glitch/Overlay"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        [MainColor]   _Color ("Tint", Color) = (1,1,1,1)
        _GlitchAmount ("Glitch Amount", Range(0,1)) = 0.6
        _ColorOffset ("Color Offset", Range(0,0.1)) = 0.03
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.3
        _BlockCount ("Block Count", Range(4,64)) = 24
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "GlitchOverlay"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _GlitchAmount;
                float _ColorOffset;
                float _ScanlineIntensity;
                float _BlockCount;
            CBUFFER_END

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = _Time.y;

                // 이 가로 띠가 지금 "글리치 중"인지 판정 (Amount 높을수록 더 많은 띠가 활성)
                float block = floor(uv.y * _BlockCount);
                float noise = rand(float2(block, floor(t * 12.0)));
                float bandActive = step(1.0 - _GlitchAmount * 0.6, noise);

                // 변위 (활성 띠만)
                float displace = bandActive * (rand(float2(block, t)) - 0.5) * 0.15;
                uv.x += displace;

                // RGB 채널 분리
                float offset = _ColorOffset;
                half4 cr = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(offset, 0));
                half4 cg = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half4 cb = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(offset, 0));
                half3 rgb = half3(cr.r, cg.g, cb.b) * _Color.rgb;

                // === 핵심: 글리치 활성 부분만 보이게 알파 결정, 나머지는 투명 ===
                float alpha = cg.a * _Color.a * bandActive;

                // 가끔 밝은 줄 번쩍 (Amount 0이면 안 뜸 → 완전 투명)
                float lineNoise = rand(float2(block, floor(t * 30.0)));
                float flash = step(0.985, lineNoise) * step(0.001, _GlitchAmount);
                rgb += flash * 0.5;
                alpha = max(alpha, flash * cg.a);

                // 스캔라인 (보이는 부분 위에만)
                float scan = sin(uv.y * 800.0) * 0.5 + 0.5;
                rgb -= scan * _ScanlineIntensity * 0.1;

                return half4(rgb, saturate(alpha));
            }
            ENDHLSL
        }
    }
}
