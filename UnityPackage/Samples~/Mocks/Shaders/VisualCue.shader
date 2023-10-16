Shader "InsEye/VisualCue"
{
   Properties
   {
       _Color("Color", COLOR) = (0,1,0,1)
   }

   SubShader
   {
      Tags { "RenderType" = "Transparent" }
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

            UNITY_VERTEX_INPUT_INSTANCE_ID
         };

         struct v2f
         {
            float4 vertex : SV_POSITION;

            UNITY_VERTEX_INPUT_INSTANCE_ID 
            UNITY_VERTEX_OUTPUT_STEREO
         };

         v2f vert (appdata v)
         {
            v2f o;

            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(v2f, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            o.vertex = UnityObjectToClipPos(v.vertex);

            return o;
         }

         fixed4 frag (v2f i) : SV_Target
         {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

            return _Color;
         }
         ENDCG
      }
   }
}