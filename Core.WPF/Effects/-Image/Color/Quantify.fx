sampler2D input : register(S0);

float SizeX : register(C0);
float SizeY : register(C1);
float SizeZ : register(C2);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv.xy);

    float3 size = float3(SizeX, SizeY, SizeZ);

    color.r = color.r;
    color.r *= size;
    color.r = round(color.r);
    color.r /= size;

    color.g = color.g;
    color.g *= size;
    color.g = round(color.g);
    color.g /= size;

    color.b = color.b;
    color.b *= size;
    color.b = round(color.b);
    color.b /= size;

    return color;
}