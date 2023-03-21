sampler2D input : register(s0);

float R : register(C0);
float G : register(C1);
float B : register(C2);

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	float4 color = tex2D(input, uv.xy); 
	color.r = R == 0 ? 0 : color.r;
	color.g = G == 0 ? 0 : color.g;
	color.b = B == 0 ? 0 : color.b;
	return color;
}