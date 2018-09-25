//sampler2D baseMap;
sampler TextureSampler : register(s0);

//struct PS_INPUT 
//{
//   float2 Texcoord : TEXCOORD0;

//};

//float4 ps_main( PS_INPUT Input ) : COLOR0
//{
//   float4 color = tex2D( baseMap, Input.Texcoord );
//   float4 left  = tex2D( baseMap, Input.Texcoord - float2( 1, 0 ) );
//   float4 right = tex2D( baseMap, Input.Texcoord + float2( 1, 0 ) );
//   float4 up    = tex2D( baseMap, Input.Texcoord - float2( 0, 1 ) );
//   float4 down  = tex2D( baseMap, Input.Texcoord + float2( 0, 1 ) );

//   if( left * right * up * down == 0 )
//    return float4(1.0f, 1.0f, 1.0f, 1.0f);
//   else return color;
//}

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the texture color.
    float4 tex = tex2D(TextureSampler, texCoord);
    
    // Convert it to greyscale. The constants 0.3, 0.59, and 0.11 are because
    // the human eye is more sensitive to green light, and less to blue.
    float greyscale = dot(tex.rgb, float3(0.3, 0.59, 0.11));
    
    // The input color alpha controls saturation level.
    tex.rgb = lerp(greyscale, tex.rgb, color.a * 4);
    
    return tex;
}

technique Outline
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}