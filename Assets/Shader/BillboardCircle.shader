Shader "Custom/BillboardCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0, 1)) = 0.5
        _Center ("Center", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "IGNOREPROJECTOR" = "true" "RenderType"="Transparent" "Queue"="Transparent+500" } //"LIGHTMODE"="FORWARDBASE" "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="TransparentCutout" "DisableBatching"="LodFading"
        LOD 100

        Pass
        {
        
            //ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
            };
            
            float4 _Center;
            float _Radius;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)


            v2f vert (appdata i)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                
                //copy them so we can change them (demonstration purpos only)
                float4x4 m = UNITY_MATRIX_M;
                float4x4 v = UNITY_MATRIX_V;
                float4x4 p = UNITY_MATRIX_P;
                
                //break out the axis
                float3 right = normalize(v._m00_m01_m02);
                float3 up = normalize(v._m10_m11_m12);
                float3 forward = normalize(v._m20_m21_m22);
                //get the rotation parts of the matrix
                float4x4 rotationMatrix = float4x4(right, 0,
    	            up, 0,
    	            forward, 0,
    	            0, 0, 0, 1);
                
                //the inverse of a rotation matrix happens to always be the transpose
                float4x4 rotationMatrixInverse = transpose(rotationMatrix);
                
                //apply the rotationMatrixInverse, model, view and projection matrix
                float4 pos = i.vertex;
                pos = mul(rotationMatrixInverse, pos);
                pos = mul(m, pos);
                pos = mul(v, pos);
                pos = mul(p, pos);
                o.vertex = pos;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = _Center.xy;
                float2 position = i.uv - center;
                float distance = length(position);
                
                if (distance > _Radius)
                    discard; // Discard fragments outside the circle
                // sample the texture
                fixed4 col = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_SETUP_INSTANCE_ID(i); // necessary only if any instanced properties are going to be accessed in the fragment Shader.
                return col;
            }
            ENDCG
        }
    }
}
