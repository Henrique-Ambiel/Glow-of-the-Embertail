Shader "Embertail/Fox Fire Surface URP"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        [HDR] _CoreColor("Fire Core", Color) = (1.8, 1.05, 0.15, 1)
        [HDR] _EdgeColor("Fire Edge", Color) = (1.2, 0.12, 0.01, 1)
        _FlameAmount("Flame Amount", Range(0, 1)) = 1
        _EmissionStrength("Emission Strength", Range(0, 8)) = 2.5
        _NoiseScale("Noise Scale", Range(0.1, 12)) = 3
        _Speed("Fire Speed", Range(0, 5)) = 1
        _RimStrength("Rim Strength", Range(0, 2)) = 0.8
        _RimPower("Rim Power", Range(0.5, 8)) = 2.2
        _MinimumBrightness("Brightness At Zero", Range(0, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                half3 normalWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                half fogFactor : TEXCOORD4;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _CoreColor;
                half4 _EdgeColor;
                float _FlameAmount;
                float _EmissionStrength;
                float _NoiseScale;
                float _Speed;
                float _RimStrength;
                float _RimPower;
                float _MinimumBrightness;
            CBUFFER_END

            float Hash21(float2 position)
            {
                position = frac(position * float2(123.34, 456.21));
                position += dot(position, position + 45.32);
                return frac(position.x * position.y);
            }

            float ValueNoise(float2 position)
            {
                float2 cell = floor(position);
                float2 localPosition = frac(position);
                localPosition = localPosition * localPosition *
                    (3.0 - 2.0 * localPosition);

                float bottomLeft = Hash21(cell);
                float bottomRight = Hash21(cell + float2(1.0, 0.0));
                float topLeft = Hash21(cell + float2(0.0, 1.0));
                float topRight = Hash21(cell + float2(1.0, 1.0));

                return lerp(
                    lerp(bottomLeft, bottomRight, localPosition.x),
                    lerp(topLeft, topRight, localPosition.x),
                    localPosition.y
                );
            }

            float Fbm(float2 position)
            {
                float value = 0.0;
                float amplitude = 0.5;

                [unroll]
                for (int octave = 0; octave < 4; octave++)
                {
                    value += ValueNoise(position) * amplitude;
                    position = position * 2.03 + 17.17;
                    amplitude *= 0.5;
                }

                return value;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(input.normalOS);

                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.positionOS = input.positionOS.xyz;
                output.normalWS = normalInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 baseSample = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv
                ) * _BaseColor;

                half3 normalWS = normalize(input.normalWS);
                half3 viewDirectionWS = GetWorldSpaceNormalizeViewDir(
                    input.positionWS
                );

                float animation = _Time.y * _Speed;
                float3 noisePosition = input.positionOS * _NoiseScale;

                // Duas projecoes reduzem costuras e fazem o fogo percorrer
                // a superficie deformada pelo esqueleto.
                float noiseA = Fbm(float2(
                    noisePosition.x,
                    noisePosition.y - animation
                ));
                float noiseB = Fbm(float2(
                    noisePosition.z + 19.7,
                    noisePosition.y * 1.17 - animation * 1.23
                ));
                float fireNoise = saturate(noiseA * 0.55 + noiseB * 0.45);

                float fresnel = pow(
                    1.0 - saturate(dot(normalWS, viewDirectionWS)),
                    _RimPower
                );

                float flameAmount = saturate(_FlameAmount);
                float coverageThreshold = lerp(0.72, 0.18, flameAmount);
                float coverage = smoothstep(
                    coverageThreshold,
                    coverageThreshold + 0.22,
                    fireNoise + fresnel * _RimStrength
                );

                float heat = saturate(
                    fireNoise * 0.72 + fresnel * _RimStrength
                );
                half3 fireColor = lerp(
                    _EdgeColor.rgb,
                    _CoreColor.rgb,
                    heat
                );

                float flicker = 0.88 + 0.12 * sin(
                    _Time.y * (_Speed * 8.0) + fireNoise * 6.28318
                );

                Light mainLight = GetMainLight(
                    TransformWorldToShadowCoord(input.positionWS)
                );
                half diffuse = saturate(dot(normalWS, mainLight.direction));
                half3 ambient = max(SampleSH(normalWS), half3(0.12, 0.12, 0.12));
                half3 lighting = ambient +
                    mainLight.color *
                    diffuse *
                    mainLight.distanceAttenuation *
                    mainLight.shadowAttenuation;

                float baseBrightness = lerp(
                    _MinimumBrightness,
                    1.0,
                    flameAmount
                );
                half3 litSurface = baseSample.rgb * lighting * baseBrightness;
                half3 emission =
                    fireColor *
                    coverage *
                    _EmissionStrength *
                    flameAmount *
                    flicker;

                half3 finalColor = MixFog(
                    litSurface + emission,
                    input.fogFactor
                );
                return half4(finalColor, baseSample.a);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
