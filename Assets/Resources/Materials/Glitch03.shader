﻿Shader "Unlit/Glitch03"
{
   
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseScale("Noise Scale",float) = 0.5
        [MaterialToggle]_IsGlitch("Is Glitch", Float) = 1
        _Alpha("Alpha", float) = 0
        _BGSlider("BGSlider", Range(0.,1.)) = 0
    }
    SubShader
    {
        // No culling or depth
         Tags { "RenderType"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
             
            sampler2D _MainTex;
            float _NoiseScale;
            float4 _MainTex_TexelSize;
            float _IsGlitch;
            float _Alpha;
            float _BGSlider;
 
           float sat( float t ) {
                return clamp( t, 0.0, 1.0 );
            }
            
            float2 sat( float2 t ) {
                return clamp( t, 0.0, 1.0 );
            }
            
            //remaps inteval [a;b] to [0;1]
            float remap  ( float t, float a, float b ) {
                return sat( (t - a) / (b - a) );
            }
            
            //note: /\ t=[0;0.5;1], y=[0;1;0]
            float linterp( float t ) {
                return sat( 1.0 - abs( 2.0*t - 1.0 ) );
            }
            
            float3 spectrum_offset( float t ) {
                float3 ret;
                float lo = step(t,0.5);
                float hi = 1.0-lo;
                float w = linterp( remap( t, 1.0/6.0, 5.0/6.0 ) );
                float neg_w = 1.0-w;
                ret = float3(lo,1.0,hi) * float3(neg_w, w, neg_w);
                float x = pow( ret.x, 1.0/2.2 );
                float y = pow( ret.y, 1.0/2.2 );
                float z = pow( ret.z, 1.0/2.2 );
                
                return float3(x,y,z);
            }
            
            //note: [0;1]
            float rand( float2 n ) {
              return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
            }
            
            //note: [-1;1]
            float srand( float2 n ) {
                return rand(n) * 2.0 - 1.0;
            }
            
            float mytrunc( float x, float num_levels )
            {
                return floor(x*num_levels) / num_levels;
            }
            float2 mytrunc( float2 x, float num_levels )
            {
                return floor(x*num_levels) / num_levels;
            }
             
            fixed4 frag (v2f i) : SV_Target {
   
                fixed4 finalCol = tex2D(_MainTex, i.uv);
                float time = fmod(_Time.y*10.0, 32.0)/11.0;
                
                float GLITCH = 0.1 + _NoiseScale / _MainTex_TexelSize.z;
                float2 uv = i.uv;
	
	            float gnm = sat( GLITCH );
                float rnd0 = rand( mytrunc( float2(time, time), 6.0 ) );
                float r0 = sat((1.0-gnm)*0.7 + rnd0);
                float rnd1 = rand( float2(mytrunc( uv.x, 10.0*r0 ), time) ); //horz
                //float r1 = 1.0f - sat( (1.0f-gnm)*0.5f + rnd1 );
                float r1 = 0.5 - 0.5 * gnm + rnd1;
                r1 = 1.0 - max( 0.0, ((r1<1.0) ? r1 : 0.9999999) ); //note: weird ass bug on old drivers
                float rnd2 = rand( float2(mytrunc( uv.y, 40.0*r1 ), time) ); //vert
                float r2 = sat( rnd2 );
            
                float rnd3 = rand( float2(mytrunc( uv.y, 10.0*r0 ), time) );
                float r3 = (1.0-sat(rnd3+0.8)) - 0.1;
            
                float pxrnd = rand( uv + time );
            
                float ofs = 0.05 * r2 * GLITCH * ( rnd0 > 0.5 ? 1.0 : -1.0 );
                ofs += 0.5 * pxrnd * ofs;
            
                uv.y += 0.1 * r3 * GLITCH;
            
                const int NUM_SAMPLES = 20;
                const float RCP_NUM_SAMPLES_F = 1.0 / float(NUM_SAMPLES);
                
                float4 sum = float4(0.0,0.0,0.0,0.0);
                float3 wsum = float3(0,0,0);
                for( int i=0; i<NUM_SAMPLES; ++i )
                {
                    float t = float(i) * RCP_NUM_SAMPLES_F;
                    uv.x = sat( uv.x + ofs * t );
                    float4 samplecol = tex2D( _MainTex, uv );
                    float3 s = spectrum_offset( t );
                    samplecol.rgb = samplecol.rgb * s;
                    sum += samplecol;
                    wsum += s;
                }
                sum.rgb /= wsum;
                sum.a *= RCP_NUM_SAMPLES_F;
                if(_IsGlitch == 0) sum = finalCol;
                
                 if(_BGSlider > uv.x && _BGSlider > 0)
                {
//                    col = float4(1,1,1,1);
//                    float a = tex2D(_MainTex, i.uv).a;
                    sum.rgb = float3(1-sum.r,1-sum.g,1-sum.b);
                    sum.a = 1;
                }
                
                
                sum.a *= _Alpha;
                return sum;
            }
            ENDCG
        }
    }
}
