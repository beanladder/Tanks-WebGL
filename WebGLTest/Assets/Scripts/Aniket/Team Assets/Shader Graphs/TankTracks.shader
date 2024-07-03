Shader "Custom/TankTracks"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Speed", Float) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Lambert vertex:vert

            struct Input
            {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            float _Speed;

            void vert(inout appdata_full v)
            {
                // Offset UV coordinates based on time and speed
                float offset = _Time.y * _Speed;
                v.texcoord.y += offset;
            }

            void surf(Input IN, inout SurfaceOutput o)
            {
                // Sample texture using modified UV coordinates
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
