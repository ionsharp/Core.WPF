sampler2D input : register(S0);


//...

float Progress
	: register(C0);
float Angle
	: register(C2);
float OffsetX
	: register(C3);
float OffsetY 
	: register(C4);
float Opacity
	: register(C1);
bool Reflect
	: register(C5);
bool Reverse
	: register(C6);
float Scale
	: register(C7);
float Thickness
	: register(C8);

//...

float4 Color1 : register(C9);
float Offset1 : register(C10);

float4 Color2 : register(C11);
float Offset2 : register(C12);

float4 Color3 : register(C13);
float Offset3 : register(C14);

float4 Color4 : register(C15);
float Offset4 : register(C16);

float4 Color5 : register(C17);
float Offset5 : register(C18);

float4 Color6 : register(C19);
float Offset6 : register(C20);

float4 Color7 : register(C21);
float Offset7 : register(C22);

float4 Color8 : register(C23);
float Offset8 : register(C24);

float4 Color9 : register(C25);
float Offset9 : register(C26);

float4 Color10 : register(C27);
float Offset10 : register(C28);

float4 Color11 : register(C29);
float Offset11 : register(C30);

float4 Color12 : register(C31);
float Offset12 : register(C32);

float4 Color13 : register(C33);
float Offset13 : register(C34);

float4 Color14 : register(C35);
float Offset14 : register(C36);

float4 Color15 : register(C37);
float Offset15 : register(C38);

float4 Color16 : register(C39);
float Offset16 : register(C40);

//...

float Length : register(C41);

//...

static float pi = 3.14159265359;

//...

static float GetDistance(float x1, float y1, float x2, float y2)
{
	return sqrt(pow(abs(x1 - x2), 2) + pow(abs(y1 - y2), 2));
}

static float FLerp(float norm, float min, float max)
{
	return (max - min) * norm + min;
}

//...

float4 Blend(float4 inputColor, float4 blendColor)
{
	blendColor.r = blendColor.r == 0 ? 0.001 : blendColor.r;
	blendColor.g = blendColor.g == 0 ? 0.001 : blendColor.g;
	blendColor.b = blendColor.b == 0 ? 0.001 : blendColor.b;

	float4 result;
	result.a = inputColor.a;

	result.a = 1.0 - (1.0 - blendColor.a) * (1.0 - inputColor.a);
	result.r = blendColor.r * blendColor.a / result.a + inputColor.r * inputColor.a * (1.0 - blendColor.a) / result.a;
	result.g = blendColor.g * blendColor.a / result.a + inputColor.g * inputColor.a * (1.0 - blendColor.a) / result.a;
	result.b = blendColor.b * blendColor.a / result.a + inputColor.b * inputColor.a * (1.0 - blendColor.a) / result.a;
	return result;
}

//...

float4 Blend(float t)
{
	t = t * (1 - Scale);

	float4 color1 = Color1;
	float4 color2 = Color2;

	float offset1 = Offset1;
	float offset2 = Offset2;

	if (Length == 0)
	{
		color1 = float4(0, 0, 0, 0);
		color2 = color1;

		offset1 = 0;
		offset2 = 1;
	}
	else if (Length == 1)
	{
		color1 = Color1;
		color2 = Color1;

		offset1 = 0;
		offset2 = 1;
	}
	else if (Length == 2)
	{
		color1 = Color1;
		color2 = Color2;

		offset1 = 0;
		offset2 = 1;
	}
	else
	{
		if (t >= Offset1 && t < Offset2)
		{
			color1 = Color1; color2 = Color2;
			offset1 = Offset1; offset2 = Offset2;
		}
		else if (t >= Offset2 && t < Offset3)
		{
			color1 = Color2; color2 = Color3;
			offset1 = Offset2; offset2 = Offset3;
		}
		else if (t >= Offset3 && t < Offset4)
		{
			color1 = Color3; color2 = Color4;
			offset1 = Offset3; offset2 = Offset4;
		}
		else if (t >= Offset4 && t < Offset5)
		{
			color1 = Color4; color2 = Color5;
			offset1 = Offset4; offset2 = Offset5;
		}
		else if (t >= Offset5 && t < Offset6)
		{
			color1 = Color5; color2 = Color6;
			offset1 = Offset5; offset2 = Offset6;
		}
		else if (t >= Offset6 && t < Offset7)
		{
			color1 = Color6; color2 = Color7;
			offset1 = Offset6; offset2 = Offset7;
		}
		else if (t >= Offset7 && t < Offset8)
		{
			color1 = Color7; color2 = Color8;
			offset1 = Offset7; offset2 = Offset8;
		}
		else if (t >= Offset8 && t < Offset9)
		{
			color1 = Color8; color2 = Color9;
			offset1 = Offset8; offset2 = Offset9;
		}
		else if (t >= Offset9 && t < Offset10)
		{
			color1 = Color9; color2 = Color10;
			offset1 = Offset9; offset2 = Offset10;
		}
		else if (t >= Offset10 && t < Offset11)
		{
			color1 = Color10; color2 = Color11;
			offset1 = Offset10; offset2 = Offset11;
		}
		else if (t >= Offset11 && t < Offset12)
		{
			color1 = Color11; color2 = Color12;
			offset1 = Offset11; offset2 = Offset12;
		}
		else if (t >= Offset12 && t < Offset13)
		{
			color1 = Color12; color2 = Color13;
			offset1 = Offset12; offset2 = Offset13;
		}
		else if (t >= Offset13 && t < Offset14)
		{
			color1 = Color13; color2 = Color14;
			offset1 = Offset13; offset2 = Offset14;
		}
		else if (t >= Offset14 && t < Offset15)
		{
			color1 = Color14; color2 = Color15;
			offset1 = Offset14; offset2 = Offset15;
		}
		else
		{
			color1 = Color15; color2 = Color16;
			offset1 = Offset15; offset2 = Offset16;
		}
	}

	float q = offset2 - offset1;
	float p = q == 0 ? 0 : (t - offset1) / q;

	color2.a = p;
	return Blend(color1, color2);
}

//...

#define GetRadian(input) (pi / 180) * input;

//...

float GetProgress(float2 xy)
{
	float r = 2 * pi;

	float t = atan2(xy.y - 0.5, xy.x - 0.5) + GetRadian(Angle);
	t = t + pi;
	if (t > r)
		t = t - r;

	t = t / r;
	return t;
}

//...

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	float4 color1 = tex2D(input, uv.xy);

	float distance = GetDistance(0.5, 0.5, uv.xy.x, uv.xy.y);

	if (distance >= 0.5 - (Thickness * 0.5) && distance <= 0.5)
	{
		float progress = GetProgress(uv.xy);
		if (progress <= Progress)
		{
			float4 color2 = Blend(progress);
			color2.a = Opacity;
			return color2;
		}
	}
	return float4(0, 0, 0, 0);
}