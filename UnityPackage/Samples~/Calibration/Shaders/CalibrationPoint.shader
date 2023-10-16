Shader "InsEye/CalibrationPoint"
{
   Properties
   {
       _Color("Color", COLOR) = (0,1,0,1)
   }

   SubShader
   {
      Tags { "RenderType" = "Transparent" "IgnoreProjector"="True" }
      Blend SrcAlpha OneMinusSrcAlpha
      ZTest Off
      Zwrite Off

      Pass
      {
         CGPROGRAM

         #pragma vertex vert
         #pragma fragment frag
         
         float4 _Color;

         #include "UnityCG.cginc"

         struct appdata
         {
            float4 vertex : POSITION;
            float2 texcoord: TEXCOORD0;
         };
         struct v2f
         {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
         };

         v2f vert (appdata v)
         {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord - 0.5; // creates circle
            return o;
         }

         fixed4 frag (v2f i) : SV_Target
         {
            float dist = length(i.uv);
            float pwidth = length(float2(ddx(dist), ddy(dist)));
            float alpha = smoothstep(0.5, 0.5 - pwidth * 1.5, dist);
            return fixed4(_Color.rgb, _Color.a * alpha);
         }
         ENDCG
      }
   }
}