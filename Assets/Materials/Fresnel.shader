Shader "Tutorial/012_Fresnel" {
    //show values to edit in inspector
    Properties {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _Metallic ("Metalness", Range(0, 1)) = 0
        _Saturation ("Saturation", Range(0, 5)) = 1
        [HDR] _Emission ("Emission", color) = (0,0,0)

   	 	_AtmosNear("_AtmosNear", Color) = (0.1686275,0.7372549,1,1)
		_AtmosFar("_AtmosFar", Color) = (0.4557808,0.5187039,0.9850746,1)
		_AtmosFalloff("_AtmosFalloff", Float) = 3
    }
    SubShader {
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        CGPROGRAM

        //the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
        //our surface shader function is called surf and we use the standard lighting model, which means PBR lighting
        //fullforwardshadows makes sure unity adds the shadow passes the shader might need
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;

        half _Smoothness;
        half _Metallic;
        half _Saturation;
        half3 _Emission;
        
  		float4 _AtmosNear;
	    float4 _AtmosFar;
	    float _AtmosFalloff;

        //input struct which is automatically filled by unity
        struct Input {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            INTERNAL_DATA
        };
        struct v2f
        {
            float4    pos : SV_POSITION;
            float3    normal : TEXCOORD0;
        };

                                #define PI 3.141592653589793
 
                inline float2 RadialCoords(float3 a_coords)
                {
                    float3 a_coords_n = normalize(a_coords);
                    float lon = atan2(a_coords_n.z, a_coords_n.x);
                    float lat = acos(a_coords_n.y);
                    float2 sphereCoords = float2(lon, lat) * (1.0 / PI);
                    return float2(sphereCoords.x * 0.5 + 0.5, 1 - sphereCoords.y);
                }
        
        //the surface shader function which sets parameters the lighting function then uses
        void surf (Input i, inout SurfaceOutputStandard o) {
            //sample and tint albedo texture
            float2 equiUV = RadialCoords(i.worldNormal);
            float4 color = tex2D(_MainTex, equiUV);
            color.rgb *= _Saturation;

            //just apply the values for metalness and smoothness
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;

            float4 Fresnel0_1_NoInput = float4(0,0,1,1);
	        float4 Fresnel0=(1.0 - dot( normalize( float4( i.viewDir.x, i.viewDir.y,i.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
	        float4 Pow0=pow(Fresnel0,_AtmosFalloff.xxxx);
	        float4 Saturate0=saturate(Pow0);
	        float4 Lerp0=lerp(_AtmosNear,_AtmosFar,Saturate0);
	        float4 Multiply1=Lerp0 * Saturate0;

            o.Albedo = (color.rgb + Multiply1);
        }
        
        ENDCG
    }
    FallBack "Standard"
}