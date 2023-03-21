sampler2D input : register(S0);

//...

float Index
	: register(C0);
float Total
	: register(C1);

float Spacing
	: register(C3);

//...

static float pi = 3.14159265359;

//...

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	float2 xy = uv.xy;
	float4 color = tex2D(input, xy);

	float slice = 1 / Total;

	float spacing = Spacing / 2;

	float minimum = (slice * Index) + spacing;
	float maximum = (slice * (Index + 1)) - spacing;

	float r = 2 * pi;

	float t = atan2(xy.y - 0.5, xy.x - 0.5) + 0;
	t = t + pi;
	if (t > r)
		t = t - r;

	t = t / r;
	
	if (t >= minimum && t <= maximum)
		return color;

	return float4(0, 0, 0, 0);
}