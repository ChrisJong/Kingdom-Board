Shader "Custom/RadiusDrawer" {
    
	Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_RadiusColor("Radius Color", Color) = (1, 1, 1, 1)
		_RadiusTex("Radius Texture", 2D) = "white" {}
		_RadiusCenter("Radius Center", Vector) = (0, 0, 0, 0)
		_RadiusSize("Radius Size", Range(0, 500)) = 20
		_RadiusBorder("Radius Border", Range(0, 100)) = 1 
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _RadiusTex;

		fixed4 _RadiusColor;
		float3 _RadiusCenter;
		float _RadiusSize;
		float _RadiusBorder;

        struct Input {
            float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
        };

		fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 r = tex2D (_RadiusTex, IN.uv_MainTex) * _RadiusColor;
			float dist = distance(_RadiusCenter, IN.worldPos);

			if(dist > _RadiusSize && dist < (_RadiusSize + _RadiusBorder))
				o.Albedo = r.rgb;
			else {
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			}

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}