Shader "Embertail/Tail Flame URP"
{
    Properties
    {
        [HDR] _CoreColor("Cor do centro", Color) = (1.0, 1.6, 0.15, 1.0)
        [HDR] _EdgeColor("Cor da borda", Color) = (1.0, 0.12, 0.01, 1.0)
        _FlameAmount("Quantidade de chama", Range(0, 1)) = 1
        _Speed("Velocidade", Range(0, 5)) = 1.5
        _NoiseScale("Escala do ruido", Range(1, 12)) = 5
        _Intensity("Intensidade", Range(0, 8)) = 2.2
        _EdgeSoftness("Suavidade da borda", Range(0.01, 0.2)) = 0.06
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _CoreColor;
                half4 _EdgeColor;
                float _FlameAmount;
                float _Speed;
                float _NoiseScale;
                float _Intensity;
                float _EdgeSoftness;
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
                localPosition = localPosition * localPosition * (3.0 - 2.0 * localPosition);

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
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float life = saturate(_FlameAmount);
                float2 uv = input.uv;

                float2 animatedUv = float2(
                    (uv.x - 0.5) * _NoiseScale,
                    uv.y * (_NoiseScale * 1.25) - _Time.y * _Speed
                );

                float noise = Fbm(animatedUv);

                // Base arredondada e ponta estreita. A oscilacao aumenta no topo.
                float taper = pow(saturate(1.0 - uv.y), 0.72);
                float halfWidth = lerp(0.26, 0.43, life) * taper + 0.012;
                float sway = (noise - 0.5) * 0.12 * smoothstep(0.15, 1.0, uv.y);
                float distanceFromCenter = abs((uv.x - 0.5) + sway);
                float sideMask = 1.0 - smoothstep(
                    max(halfWidth - _EdgeSoftness, 0.0),
                    halfWidth + _EdgeSoftness,
                    distanceFromCenter
                );

                // A energia define a altura; o ruido movimenta a ponta.
                float maximumHeight = lerp(0.08, 1.02, life);
                float irregularHeight = maximumHeight + (noise - 0.5) * 0.20 * life;
                float heightMask = 1.0 - smoothstep(
                    irregularHeight - _EdgeSoftness,
                    irregularHeight + _EdgeSoftness,
                    uv.y
                );

                float breakup = smoothstep(
                    0.27,
                    0.62,
                    noise + (1.0 - uv.y) * 0.24 + life * 0.12
                );

                // Estas mascaras garantem que nenhuma borda do Quad apareca.
                float horizontalEdgeFade =
                    smoothstep(0.01, 0.09, uv.x) *
                    (1.0 - smoothstep(0.91, 0.99, uv.x));
                float baseFade = smoothstep(0.015, 0.10, uv.y);
                float visibleLife = smoothstep(0.0, 0.06, life);
                float alpha =
                    sideMask *
                    heightMask *
                    breakup *
                    horizontalEdgeFade *
                    baseFade *
                    visibleLife;

                float core = 1.0 - smoothstep(
                    0.0,
                    max(halfWidth, 0.001),
                    distanceFromCenter
                );
                half4 flameColor = lerp(
                    _EdgeColor,
                    _CoreColor,
                    saturate(core * 0.85)
                );

                clip(alpha - 0.002);
                return half4(flameColor.rgb * _Intensity, alpha * flameColor.a);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
