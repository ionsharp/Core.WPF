sampler2D input			: register(S0);

//...

float Model				: register(C0);
float XComponent		: register(C1);
float YComponent		: register(C2);

float Mode				: register(C3);
float Shape				: register(C4);
float Depth				: register(C5);

float X					: register(C6);
float Y					: register(C7);
float Z					: register(C8);
float W					: register(C9);

float Companding		: register(C10);

float Compression_A		: register(C45);
float Compression_B		: register(C46);
float Compression_C		: register(C47);
float Compression_D		: register(C48);
float Compression_E		: register(C49);

float WhiteX			: register(C12) = 0.31271; //D65 (2°)
float WhiteY			: register(C13) = 0.32902; //D65 (2°)

float3 LMS_XYZ_x		: register(C14);
float3 LMS_XYZ_y		: register(C15);
float3 LMS_XYZ_z		: register(C16);

float3 RGB_XYZ_x		: register(C17);
float3 RGB_XYZ_y		: register(C18);
float3 RGB_XYZ_z		: register(C19);

float3 XYZ_LMS_x		: register(C20);
float3 XYZ_LMS_y		: register(C21);
float3 XYZ_LMS_z		: register(C22);

float3 XYZ_RGB_x		: register(C23);
float3 XYZ_RGB_y		: register(C24);
float3 XYZ_RGB_z		: register(C25);

//...

float3 LABk_LMSk_x		: register(C26);
float3 LABk_LMSk_y		: register(C27);
float3 LABk_LMSk_z		: register(C28);

float3 LMSk_LABk_x		: register(C29);
float3 LMSk_LABk_y		: register(C30);
float3 LMSk_LABk_z		: register(C31);

float3 LMSk_XYZk_x		: register(C32);
float3 LMSk_XYZk_y		: register(C33);
float3 LMSk_XYZk_z		: register(C34);

float3 XYZk_LMSk_x		: register(C35);
float3 XYZk_LMSk_y		: register(C36);
float3 XYZk_LMSk_z		: register(C37);

//...

float3  xyYC_exy		: register(C38);

//...

float HighlightAmount	: register(C39);
float HighlightRange	: register(C40);

float MidtoneAmount		: register(C41);
float MidtoneRange		: register(C42);

float ShadowAmount		: register(C43);
float ShadowRange		: register(C44);

//[Constants]

static float pi = 3.1415926535897932384626433832795028841971693993751058209749445923078164062;
static float pi2 = pi * 2;
static float pi3 = pi * 3;

static float Alpha = 1.09929682680944;
static float Beta = 0.018053968510807;
static float BetaInverse = Beta * 4.5;
static float Epsilon = 216.0 / 24389.0;
static float Kappa = 24389.0 / 27.0;

static float MaxValue = 3.40282347E+38;

//...

static float Total3 = 52;

static float Total4 = 4;

//...

static float3 Maximum3[52] =
{
	//RCA
	float3(255, 255, 255),
	//RGB
	float3(1, 1, 1),
	//RGV
	float3(255, 255, 255),
	//RYB
	float3(255, 255, 255),
	//CMY
	float3(1, 1, 1),
	//HCV
	float3(360, 100, 100),
	//HCY
	float3(360, 100, 255),
	//HPLuv
	float3(360, 100, 100),
	//HSB
	float3(360, 100, 100),
	//HSL
	float3(360, 100, 100),
	//HSLuv
	float3(360, 100, 100),
	//HSM
	float3(360, 100, 255),
	//HSP
	float3(360, 100, 255),
	//HWB
	float3(360, 100, 100),
	//IPT
	float3(1, 1, 1),
	//JCh
	float3(100, 100, 360),
	//JMh
	float3(100, 100, 360),
	//Jsh
	float3(100, 100, 360),
	//JPEG
	float3(255, 255, 255),
	//Lab
	float3(100, 100, 100),
	//Labh
	float3(100, 128, 128),
	//Labi
	//float3(8, 12, 6),
	//Labj
	float3(1, 1, 1),
	//Labk
	float3(1, 1, 1),
	//Labksb
	float3(360, 100, 100),
	//Labksl
	float3(360, 100, 100),
	//Labkwb
	float3(360, 100, 100),
	//LCHab
	float3(100, 100, 360),
	//LCHabh
	float3(100, 100, 360),
	//LCHabj
	float3(1, 1, 360),
	//LCHrg
	float3(100, 100, 360),
	//LCHuv
	float3(100, 100, 360),
	//LCHxy
	float3(100, 100, 360),
	//LMS
	float3(1, 1, 1),
	//Luv
	float3(100, 224, 122),
	//QCh
	float3(100, 100, 360),
	//QMh
	float3(100, 100, 360),
	//Qsh
	float3(100, 100, 360),
	//rgG
	float3(1, 1, 1),
	//TSL
	float3(1, 1, 1),
	//UCS
	float3(1, 1, 1),
	//UVW
	float3(224, 122, 100),
	//xvYCC
	float3(255, 255, 255),
	//xyY
	float3(1, 1, 1),
	//xyYC
	float3(76, 100, 100),
	//XYZ
	float3(1, 1, 1),
	//YCbCr
	float3(235, 240, 240),
	//YCoCg
	float3(1, 0.5, 0.5),
	//YDbDr
	float3(1, 1.333, 1.333),
	//YES
	float3(1, 1, 1),
	//YIQ
	float3(1, 0.5957, 0.5226),
	//YPbPr
	float3(1, 0.5, 0.5),
	//YUV
	float3(1, 0.5, 0.5)
};

static float3 Minimum3[52] =
{
	//RCA
	float3(0, 0, 0),
	//RGB
	float3(0, 0, 0),
	//RGV
	float3(0, 0, 0),
	//RYB
	float3(0, 0, 0),
	//CMY
	float3(0, 0, 0),
	//HCV
	float3(0, 0, 0),
	//HCY
	float3(0, 0, 0),
	//HPLuv
	float3(0, 0, 0),
	//HSB
	float3(0, 0, 0),
	//HSL
	float3(0, 0, 0),
	//HSLuv
	float3(0, 0, 0),
	//HSM
	float3(0, 0, 0),
	//HSP
	float3(0, 0, 0),
	//HWB
	float3(0, 0, 0),
	//IPT
	float3(0, 0, 0),
	//JCh
	float3(0, 0, 0),
	//JMh
	float3(0, 0, 0),
	//Jsh
	float3(0, 0, 0),
	//JPEG
	float3(0, 0, 0),
	//Lab
	float3(0, 0, 0),
	//Labh
	float3(0, -128, -128),
	//Labi
	//float3(-10, -6, -10),
	//Labj
	float3(0, -1, -1),
	//Labk
	float3(0, 0, 0),
	//Labksb
	float3(0, 0, 0),
	//Labksl
	float3(0, 0, 0),
	//Labkwb
	float3(0, 0, 0),
	//LCHab
	float3(0, 0, 0),
	//LCHabh
	float3(0, 0, 0),
	//LCHabj
	float3(0, 0, 0),
	//LCHrg
	float3(0, 0, 0),
	//LCHuv
	float3(0, 0, 0),
	//LCHxy
	float3(0, 0, 0),
	//LMS
	float3(0, 0, 0),
	//Luv
	float3(0, -134, -140),
	//QCh
	float3(0, 0, 0),
	//QMh
	float3(0, 0, 0),
	//Qsh
	float3(0, 0, 0),
	//rgG
	float3(0, 0, 0),
	//TSL
	float3(0, 0, 0),
	//UCS
	float3(0, 0, 0),
	//UVW
	float3(-134, -140, 0),
	//xvYCC
	float3(0, 0, 0),
	//xyY
	float3(0, 0, 0),
	//xyYC
	float3(10, 0, 0),
	//XYZ
	float3(0, 0, 0),
	//YCbCr
	float3(16, 16, 16),
	//YCoCg
	float3(0, -0.5, -0.5),
	//YDbDr
	float3(0, -1.333, -1.333),
	//YES
	float3(0, 0, 0),
	//YIQ
	float3(0, -0.5957, -0.5226),
	//YPbPr
	float3(0, -0.5, -0.5),
	//YUV
	float3(0, -0.5, -0.5)
};

static float4 Maximum4[4] =
{
	//CMYK
	float4(100, 100, 100, 100),
	//CMYW
	float4(100, 100, 100, 100),
	//RGBK
	float4(255, 255, 255, 255),
	//RGBW
	float4(255, 255, 255, 255),
};

static float4 Minimum4[4] =
{
	//CMYK
	float4(0, 0, 0, 0),
	//CMYW
	float4(0, 0, 0, 0),
	//RGBK
	float4(0, 0, 0, 0),
	//RGBW
	float4(0, 0, 0, 0),
};

//[Structs]

struct CAM02
{
	float J, Q;
	float C, M, s;
	float h;
};

struct CAM02Conditions
{
	float xw, yw, zw, aw;
	float la, yb;
	int surround;
	float n, z, f, c, nbb, nc, ncb, fl, d;
};

//[Matrices]
static float3x3 Aab_RGB = float3x3
(
	float3(0.32787,  0.32145,  0.20527),
	float3(0.32787, -0.63507, -0.18603),
	float3(0.32787, -0.15681, -4.49038)
);

static float3x3 CAT02_XYZ = float3x3
(
	float3( 1.096124, -0.278869, 0.182745),
	float3( 0.454369,  0.473533, 0.072098),
	float3(-0.009628, -0.005698, 1.015326)
);

static float3x3 XYZ_CAT02 = float3x3
(
	float3( 0.7328, 0.4296, -0.1624),
	float3(-0.7036, 1.6975,  0.0061),
	float3( 0.0030, 0.0136,  0.9834)
);

static float3x3 HPE_XYZ = float3x3
(
	float3(1.910197, -1.112124,  0.201908),
	float3(0.370950,  0.629054, -0.000008),
	float3(0,		  0,		 1)
);

//[Methods]

static bool AB(float input, float a, float b)
{
	return input >= a && input <= b;
}

static bool Ab(float input, float a, float b)
{
	return input >= a && input < b;
}

static bool aB(float input, float a, float b)
{
	return input > a && input <= b;
}

//...

static float Cbrt(float input) { return pow(input, 1 / 3); }

static float ConvertRange(float value, float minimum, float maximum) { return (value * (maximum - minimum)) + minimum; }

static float3 ConvertRange(float3 value, float3 minimum, float3 maximum)
{
	value[0] = ConvertRange(value[0], minimum[0], maximum[0]);
	value[1] = ConvertRange(value[1], minimum[1], maximum[1]);
	value[2] = ConvertRange(value[2], minimum[2], maximum[2]);
	return value;
}

static float4 ConvertRange(float4 value, float4 minimum, float4 maximum)
{
	value[0] = ConvertRange(value[0], minimum[0], maximum[0]);
	value[1] = ConvertRange(value[1], minimum[1], maximum[1]);
	value[2] = ConvertRange(value[2], minimum[2], maximum[2]);
	value[3] = ConvertRange(value[3], minimum[3], maximum[3]);
	return value;
}

static float3 FLerp(float3 xyz, float3 max, float3 min, float3 scale)
{
	return float3
	(
		xyz[0] + ((max[0] + abs(min[0])) - (xyz[0] + abs(min[0]))) * scale[0],
		xyz[1] + ((max[1] + abs(min[1])) - (xyz[1] + abs(min[1]))) * scale[1],
		xyz[2] + ((max[2] + abs(min[2])) - (xyz[2] + abs(min[2]))) * scale[2]
	);
}

//* Used by [HSLuv] and [HPLuv]

static float GetDistance(float2 input)
{
	return sqrt(pow(input.x, 2) + pow(input.y, 2));
}

static float GetIntersection(float2 a, float2 b)
{
	return (a.y - b.y) / (b.x - a.x);
}

static float2 GetBounds2(int t, float L, float m1, float m2, float m3, float s2)
{
	float x = (284517 * m1 - 948390 * m3) * s2;
	float y = (838422 * m3 + 769860 * m2  + 731718 * m1) * L * s2 - 769860 * t * L;
	float z = (632260 * m3 - 126452 * m2) * s2 + 126452 * t;
	return float2(x / z, y / z);
}

static float2x2 GetBounds1(int c, float L)
{
	float3x3 M = float3x3
	(
		float3( 3.240969941904521, -1.537383177570093, -0.498610760293),
		float3(-0.96924363628087,   1.87596750150772,   0.041555057407175),
		float3( 0.055630079696993, -0.20397695888897,   1.056971514242878)
	);

	float s1 = pow(L + 16, 3) / 1560896;
	float s2 = s1 > Epsilon ? s1 : L / Kappa;

	float m1 = M[c][0]; float m2 = M[c][1]; float m3 = M[c][2];
	return float2x2(GetBounds2(0, L, m1, m2, m3, s2), GetBounds2(1, L, m1, m2, m3, s2));
}

//...

//> [HPLuv]
static float GetChroma1a(int i, float L, float result)
{
	float2x2 bounds = GetBounds1(0, L);

	float m1 = bounds[i][0];
	float b1 = bounds[i][1];

	float x = GetIntersection(float2(m1, b1), float2(-1 / m1, 0));
	float length = GetDistance(float2(x, b1 + x * m1));

	return min(result, length);
}

static float GetChroma1(float L)
{
	float result = MaxValue;
	result = GetChroma1a(0, L, result);
	result = GetChroma1a(1, L, result);
	return result;
}

//> [HSLuv]
static float GetChroma2a(float2 xy, float hrad, float result)
{
	float length = xy.y / (sin(hrad) - xy.x * cos(hrad));
	if (length >= 0)
		result = min(result, length);

	return result;
}

static float GetChroma2(float L, float H)
{
	float hrad = H / 360 * pi2;

	float2x2 x = GetBounds1(0, L);
	float2x2 y = GetBounds1(1, L);
	float2x2 z = GetBounds1(2, L);

	float result = MaxValue;

	result = GetChroma2a(x[0], hrad, result);
	result = GetChroma2a(x[1], hrad, result);

	result = GetChroma2a(y[0], hrad, result);
	result = GetChroma2a(y[1], hrad, result);

	result = GetChroma2a(z[0], hrad, result);
	result = GetChroma2a(z[1], hrad, result);
	return result;
}

//...

static float GetDegree(float radian) { return radian * (180 / pi); }

static float GetDistance(float x1, float y1, float x2, float y2) { return sqrt(pow(abs(x1 - x2), 2) + pow(abs(y1 - y2), 2)); }

static float GetDistance(float x, float y) { return GetDistance(0.5, 0.5, x, y); }

static float3x3 GetMatrix(float3 r, float3 g, float3 b) { return float3x3(float3(r.x, r.y, r.z), float3(g.x, g.y, g.z), float3(b.x, b.y, b.z)); }

static float GetMaximum(float3 input) { return max(input[0], max(input[1], input[2])); }

static float GetMinimum(float3 input) { return min(input[0], min(input[1], input[2])); }

static float GetRadian(float degree) { return (pi / 180) * degree; }

static float GetSum(float3 input) { return input[0] + input[1] + input[2]; }

static float3 Multiply(float3x3 a, float3 b)
{
	float x = b[0] * a[0][0] + b[1] * a[1][0] + b[2] * a[2][0];
	float y = b[0] * a[0][1] + b[1] * a[1][1] + b[2] * a[2][1];
	float z = b[0] * a[0][2] + b[1] * a[1][2] + b[2] * a[2][2];
	return float3(x, y, z);
}

static float NormalizeDegree(float degree)
{
	float result = degree % 360.0;
	return result >= 0 ? result : (result + 360.0);
}

//...

static float PerceptualQuantizer(float x)
{
	float xx = pow(x * 1e-4, 0.1593017578125);
	float result = pow((0.8359375 + 18.8515625 * xx) / (1 + 18.6875 * xx), 134.034375);
	return result;
}

static float PerceptualQuantizerInverse(float X)
{
	float XX = pow(X, 7.460772656268214e-03);
	float result = 1e4 * pow((0.8359375 - XX) / (18.6875 * XX - 18.8515625), 6.277394636015326);
	return result;
}

//...

static float Pow2(float input) { return pow(input, 2); }

static float Pow3(float input) { return pow(input, 3); }

//...

//(+|+) [XYZ] > [xyY]
float3 xyY_XYZ(float3 input)
{
	float x = input[0]; float y = input[1]; float Y = input[2];
	if (y == 0)
	{
		return float3(0.0000000000001, 0.0000000000001, 0.0000000000001);
	}
	return float3(x * Y / y, Y, (1 - x - y) * Y / y);
}
float3 XYZ_xyY(float3 input)
{
	float sum, X, Y, Z;
	X = input[0]; Y = input[1]; Z = input[2];

	sum = X + Y + Z;
	if (sum == 0)
	{
		return float3(0, 0, Y);
	}
	return float3(X / sum, Y / sum, Y);
}

//(+|+) [XYZ] > [xyY] > [xy]
static float3 xy_XYZ(float x, float y)
{
	//xy > xyY
	float3 result = float3(x, y, 1);

	//xyY > XYZ
	return xyY_XYZ(result);
}
static float3 xy_XYZ(float2 input) { return xy_XYZ(input.x, input.y); }

static float2 XYZ_xy(float3 input)
{
	//XYZ > xyY
	float3 result = XYZ_xyY(input);

	//xyY > xy
	return float2(result.x, result.y);
}

//...

static float ComputeKa(float3 input)
{
	float3 cW = xy_XYZ(0.31006, 0.31616);
	if (input[0] == cW[0] && input[1] == cW[1] && input[2] == cW[2]) //C (2°)
		return 175;

	float Ka = 100 * (175 / 198.04) * (input[0] + input[1]);
	return Ka;
}

static float ComputeKb(float3 input)
{
	float3 cW = xy_XYZ(0.31006, 0.31616);
	if (input[0] == cW[0] && input[1] == cW[1] && input[2] == cW[2]) //C (2°)
		return 70;

	float Ka = 100 * (70 / 218.11) * (input[1] + input[2]);
	return Ka;
}

static float ComputeKu(float3 i)
{
	return 4 * i[0] / (i[0] + 15 * i[1] + 3 * i[2]);
}

static float ComputeKv(float3 i)
{
	return 9 * i[1] / (i[0] + 15 * i[1] + 3 * i[2]);
}

//...

//[Conversion] (from|to) += Done, -= Not done

//(+|+) [LCh]
static float3 FromLCh(float3 input)
{
	float c = input[1], h = input[2];
	h = GetRadian(h);

	float a = c * cos(h);
	float b = c * sin(h);
	return float3(input[0], a, b);
}
static float3 ToLCh(float3 input)
{
	return float3(0, 0, 0);
	float a = input[1], b = input[2];
	float c = sqrt(a * a + b * b);

	float h = atan2(b, a);
	h = NormalizeDegree(GetDegree(h));
	return float3(input[0], c, h);
}

//(+|-) [LCh(x)]
float3 FromLChx(float3 i)
{
	float u = GetDistance(0.5, 0.5, i[2] / 360, i[1] / 100);
	float v = GetDistance(0.5, 0.5, i[2] / 360, i[0] / 100);
	return float3(i[0], i[1], clamp(i[2] * u / v, 0, 359));
}
float3 ToLChx(float3 i)
{
	float h = i[2]; //?
	return float3(i[0], i[1], h);
}

//(+|-) [LCh(y)]
float3 FromLChy(float3 i)
{
	float w = GetDistance(0.5, 0.5, i[1] / 100, i[0] / 100);
	return float3(i[0], i[1], i[2] * w);
}
float3 ToLChy(float3 i)
{
	float h = i[2]; //?
	return float3(i[0], i[1], h);
}

//(+|-) [LCh(z)]
float3 FromLChz(float3 i)
{
	float l = i[0] / 100;
	float c = i[1] / 100;
	float h = i[2] / 360;

	float u = GetDistance(h, c);
	float v = GetDistance(h, l);

	float x = GetDistance(c, c);
	float y = GetDistance(c, l);

	return float3(i[0], clamp(i[1] * x / y, 0, 100), clamp(i[2] * u / v, 0, 359));
}
float3 ToLChz(float3 i)
{
	float h = i[2]; //?
	return float3(i[0], i[1], h);
}

//...

//(+|+) [CMY]
float3 CMY_Lrgb(float3 input)
{
	return float3(1 - input[0], 1 - input[1], 1 - input[2]);
}
float3 Lrgb_CMY(float3 input)
{
	return float3(1 - input[0], 1 - input[1], 1 - input[2]);
}

//(+|-) [CMYK]
float3 CMYK_Lrgb(float4 input)
{
	//input = input / 100;
	float r = (1 - input[0]) * (1 - input[3]);
	float g = (1 - input[1]) * (1 - input[3]);
	float b = (1 - input[2]) * (1 - input[3]);
	return float3(r, g, b);
}
float4 Lrgb_CMYK(float3 input)
{
	return float4(0, 0, 0, 0);
}

//(+|-) [CMYW]
float3 CMYW_Lrgb(float4 input)
{
	float c = 1 - input[0]; /// 255;
	float m = 1 - input[1]; /// 255;
	float y = 1 - input[2]; /// 255;
	float w = input[3];

	c *= (1 - w);
	c += w;

	m *= (1 - w);
	m += w;

	y *= (1 - w);
	y += w;

	return float3(c, m, y);
}
float4 Lrgb_CMYW(float3 input)
{
	return float4(0, 0, 0, 0);
}

//(+|+) [HCV]
float3 HCV_Lrgb(float3 input)
{
	float3 result = { 0, 0, 0 };

	float h = input[0] / 360.0, c = input[1] / 100.0, g = input[2] / 100.0;

	if (c == 0)
	{
		result[0] = result[1] = result[2] = g;
		return result;
	}

	float hi = (h % 1.0) * 6.0;
	float v = hi % 1.0;
	float3 pure = { 0, 0, 0 };
	float w = 1.0 - v;

	float fhi = floor(hi);

	if (fhi == 0)
	{
		pure[0] = 1;
		pure[1] = v;
		pure[2] = 0;
	}
	else if (fhi == 1)
	{
		pure[0] = w;
		pure[1] = 1;
		pure[2] = 0;
	}
	else if (fhi == 2)
	{
		pure[0] = 0;
		pure[1] = 1;
		pure[2] = v;
	}
	else if (fhi == 3)
	{
		pure[0] = 0;
		pure[1] = w;
		pure[2] = 1;
	}
	else if (fhi == 4)
	{
		pure[0] = v;
		pure[1] = 0;
		pure[2] = 1;
	}
	else
	{
		pure[0] = 1;
		pure[1] = 0;
		pure[2] = w;
	}

	float mg = (1.0 - c) * g;

	result[0] = c * pure[0] + mg;
	result[1] = c * pure[1] + mg;
	result[2] = c * pure[2] + mg;
	return result;
}
float3 Lrgb_HCV(float3 input)
{
	float min = GetMinimum(input), max = GetMaximum(input);
	float h = 0, c = max - min, v = 0;

	if (c < 1)
	{
		v = min / (1.0 - c);
	}
	if (c > 0)
	{
		if (max == input[0])
		{
			h = ((input[1] - input[2]) / c) % 6;
		}
		else if (max == input[1])
		{
			h = 2 + (input[2] - input[0]) / c;
		}
		else
		{
			h = 4 + (input[0] - input[1]) / c;
		}

		h /= 6;
		h %= 1;
	}
	return float3(h * 360.0, c * 100.0, v * 100.0);
}

//(+|+) [HCY]
float3 HCY_Lrgb(float3 input)
{
	float h = (input[0] < 0 ? (input[0] % 360) + 360 : (input[0] % 360)) * pi / 180;
	float s = input[1] / 100;
	float i = input[2] / 255;

	float pi3 = pi / 3;

	float r, g, b;
	if (h < (2 * pi3)) 
	{
		b = i * (1 - s);
		r = i * (1 + (s * cos(h) / cos(pi3 - h)));
		g = i * (1 + (s * (1 - cos(h) / cos(pi3 - h))));
	}
	else if (h < (4 * pi3)) 
	{
		h = h - 2 * pi3;
		r = i * (1 - s);
		g = i * (1 + (s * cos(h) / cos(pi3 - h)));
		b = i * (1 + (s * (1 - cos(h) / cos(pi3 - h))));
	}
	else 
	{
		h = h - 4 * pi3;
		g = i * (1 - s);
		b = i * (1 + (s * cos(h) / cos(pi3 - h)));
		r = i * (1 + (s * (1 - cos(h) / cos(pi3 - h))));
	}

	return float3(r, g, b);
}
float3 Lrgb_HCY(float3 input)
{
	float sum = GetSum(input);
	float r = input[0] / sum, g = input[1] / sum, b = input[2] / sum;

	float h = acos((0.5 * ((r - g) + (r - b))) / sqrt((r - g) * (r - g) + (r - b) * (g - b)));
	if (b > g)
	{
		h = 2 * pi - h;
	}

	float min = GetMinimum(float3(r, g, b));
	float s = 1 - 3 * min;
	float i = sum / 3;

	return float3(h * 180 / pi, s * 100, i);
}

//(+|+) [HSB]
float3 HSB_Lrgb(float3 input)
{
	float3 result = { 0, 0, 0 };

	float _h = input[0], _s = input[1] / 100, _b = input[2] / 100;
	float r = 0, g = 0, b = 0;
	
	_h = _h > 359 ? 359 : _h; //360 produces BLACK!
	_h /= 360;

	if (_s == 0)
	{
	    r = g = b = _b;
	}
	else
	{
	    _h *= 360;
	
	    //The color wheel consists of 6 sectors: Figure out which sector we're in...
	    float SectorPosition = _h / 60.0;
	    float SectorNumber = floor(SectorPosition);
	
	    //Get the fractional part of the sector
	    float FractionalSector = SectorPosition - SectorNumber;
	
	    //Calculate values for the three axes of the color. 
	    float p = _b * (1.0 - _s);
	    float q = _b * (1.0 - (_s * FractionalSector));
	    float t = _b * (1.0 - (_s * (1.0 - FractionalSector)));
	
	    //Assign the fractional colors to r, g, and b based on the sector the angle is in.
        if (SectorNumber == 0)
        {
            r = _b;
            g = t;
            b = p;
        }
        else if (SectorNumber == 1)
        {
            r = q;
            g = _b;
            b = p;
        }
        else if (SectorNumber == 2)
        {
            r = p;
            g = _b;
            b = t;
        }
        else if (SectorNumber == 3)
        {
            r = p;
            g = q;
            b = _b;
        }
        else if (SectorNumber == 4)
        {
            r = t;
            g = p;
            b = _b;
        }
        else if (SectorNumber == 5)
        {
            r = _b;
            g = p;
            b = q;
        }
	}
	result[0] = r;
	result[1] = g;
	result[2] = b;
	return result;
}
float3 Lrgb_HSB(float3 input)
{
	float r = input[0], g = input[1], b = input[2];

	float min = GetMinimum(input);
	float max = GetMaximum(input);

	float chroma = max - min;

	float _h = 0.0, _s = 0.0, _b = max;
	if (chroma == 0)
	{
		_h = 0; _s = 0;
	}
	else
	{
		_s = chroma / max;

		if (input[0] == max)
		{
			_h = (input[1] - input[2]) / chroma;
			_h = input[1] < input[2] ? _h + 6 : _h;
		}
		else if (input[1] == max)
		{
			_h = 2.0 + ((input[2] - input[0]) / chroma);
		}
		else if (input[2] == max)
			_h = 4.0 + ((input[0] - input[1]) / chroma);

		_h *= 60;
	}

	return float3(_h, _s * 100, _b * 100);
}

//(+|+) [HSL]
float3 HSL_Lrgb(float3 input)
{

	float h = input[0], s = input[1] / 100, l = input[2] / 100;
	h = h > 359 ? 359 : h; //360 produces BLACK!

	h /= 60;

	float3 result = { 0, 0, 0 };

	if (s > 0)
	{
		float chroma = (1.0 - abs(2.0 * l - 1.0)) * s;
		float v = chroma * (1.0 - abs((h % 2.0) - 1));

		if (0 <= h && h <= 1)
		{
			result[0] = chroma;
			result[1] = v;
			result[2] = 0;
		}
		else if (1 <= h && h <= 2)
		{
			result[0] = v;
			result[1] = chroma;
			result[2] = 0;
		}
		else if (2 <= h && h <= 3)
		{
			result[0] = 0;
			result[1] = chroma;
			result[2] = v;
		}
		else if (3 <= h && h <= 4)
		{
			result[0] = 0;
			result[1] = v;
			result[2] = chroma;
		}
		else if (4 <= h && h <= 5)
		{
			result[0] = v;
			result[1] = 0;
			result[2] = chroma;
		}
		else if (5 <= h && h <= 6)
		{
			result[0] = chroma;
			result[1] = 0;
			result[2] = v;
		}

		float w = l - (0.5 * chroma);
		result[0] += w;
		result[1] += w;
		result[2] += w;
	}
	else
	{
		result[0] = result[1] = result[2] = l;
	}
	return result;
}
float3 Lrgb_HSL(float3 input)
{
	float min = GetMinimum(input);
	float max = GetMaximum(input);

	float chroma = max - min;

	float h = 0, s = 0, l = (max + min) / 2.0;
	if (chroma != 0)
	{
		s = l < 0.5
			? chroma / (2.0 * l)
			: chroma / (2.0 - 2.0 * l);

		if (input[0] == max)
		{
			h = (input[1] - input[2]) / chroma;
			h = input[1] < input[2]
				? h + 6.0 : h;
		}
		else if (input[2] == max)
		{
			h = 4.0 + ((input[0] - input[1]) / chroma);
		}
		else if (input[1] == max)
		{
			h = 2.0 + ((input[2] - input[0]) / chroma);
		}

		h *= 60;
	}
	return float3(h, s * 100, l * 100);
}

//(-|-) [HSM]
float3 HSM_Lrgb(float3 input)
{
	float h = cos(GetRadian(input[0])), s = input[1] / 100, m = input[2] / 255;
	float r, g, b;

	float i = h * s;
	float j = i * sqrt(41);

	float x = 4 / 861;
	float y = 861 * Pow2(s);
	float z = 1 - Pow2(h);

	r = (3 / 41 * i) + m - (x * sqrt(y * z));
	g = (j + (23 * m) - (19 * r)) / 4;
	b = ((11 * r) - (9 * m) - j) / 2;
	return float3(r, g, b);
}
float3 Lrgb_HSM(float3 input)
{
	float3 max = Maximum3[Model];

	float m = ((4 * input[0]) + (2 * input[1]) + input[2]) / 7;

	float t, w;

	float j = (3 * (input[0] - m) - 4 * (input[1] - m) - 4 * (input[2] - m)) / sqrt(41);
	float k = sqrt(pow(input[0] - m, 2) + pow(input[1] - m, 2) + pow(input[2] - m, 2));

	t = acos(j / k);
	w = input[2] <= input[1] ? t : pi2 - t;

	float r = input[0], g = input[1], b = input[2];

	float u, v = 0;
	u = pow(r - m, 2) + pow(g - m, 2) + pow(b - m, 2);

	if (AB(m, 0 / 7, 1 / 7))
	{
		v = Pow2(0 - m) + Pow2(0 - m) + Pow2(7 - m);
	}
	else if (aB(m, 1 / 7, 3 / 7))
	{
		v = Pow2(0 - m) + Pow2(((7 * m - 1) / 2) - m) + Pow2(1 - m);
	}
	else if (aB(m, 3 / 7, 1 / 2))
	{
		v = Pow2(((7 * m - 3) / 2) - m) + Pow2(1 - m) + Pow2(1 - m);
	}
	else if (aB(m, 1 / 2, 4 / 7))
	{
		v = Pow2(((7 * m) / 4) - m) + Pow2(0 - m) + Pow2(0 - m);
	}
	else if (aB(m, 4 / 7, 6 / 7))
	{
		v = Pow2(1 - m) + Pow2(((7 * m - 4) / 2) - m) + Pow2(0 - m);
	}
	else if (aB(m, 6 / 7, 7 / 7))
	{
		v = Pow2(1 - m) + Pow2(1 - m) + Pow2((7 * m - 6) - m);
	}

	float h = w / pi2;
	float s = sqrt(u) / sqrt(v);

	return float3(h * max[0], s * max[1], m * max[2]);
}

//(+|+) [HSP]
float3 HSP_Lrgb(float3 input)
{
	float3 result = { 0, 0, 0 };

	const float Pr = 0.299;
	const float Pg = 0.587;
	const float Pb = 0.114;
	
	float h = input[0] / 360.0, s = input[1] / 100.0, p = input[2];
	float r = 0, g= 0, b= 0;
	
	float part= 0, minOverMax = 1.0 - s;
	
	if (minOverMax > 0.0)
	{
	    // R > G > B
	    if (h < 1.0 / 6.0)
	    {
	        h = 6.0 * (h - 0.0 / 6.0);
	        part = 1.0 + h * (1.0 / minOverMax - 1.0);
	        b = p / sqrt(Pr / minOverMax / minOverMax + Pg * part * part + Pb);
	        r = (b) / minOverMax;
	        g = (b) + h * ((r) - (b));
	    }
	    // G > R > B
	    else if (h < 2.0 / 6.0)
	    {
	        h = 6.0 * (-h + 2.0 / 6.0);
	        part = 1.0 + h * (1.0 / minOverMax - 1.0);
	        b = p / sqrt(Pg / minOverMax / minOverMax + Pr * part * part + Pb);
	        g = (b) / minOverMax;
	        r = (b) + h * ((g) - (b));
	    }
	    // G > B > R
	    else if (h < 3.0 / 6.0)
	    {
	        h = 6.0 * (h - 2.0 / 6.0);
	        part = 1.0 + h * (1.0 / minOverMax - 1.0);
	        r = p / sqrt(Pg / minOverMax / minOverMax + Pb * part * part + Pr);
	        g = (r) / minOverMax;
	        b = (r) + h * ((g) - (r));
	    }
	    // B > G > R
	    else if (h < 4.0 / 6.0)
	    {
	        h = 6.0 * (-h + 4.0 / 6.0);
	        part = 1.0 + h * (1.0 / minOverMax - 1.0);
	        r = p / sqrt(Pb / minOverMax / minOverMax + Pg * part * part + Pr);
	        b = (r) / minOverMax;
	        g = (r) + h * ((b) - (r));
	    }
	    // B > R > G
	    else if (h < 5.0 / 6.0)
	    {
	        h = 6.0 * (h - 4.0 / 6.0);
	        part = 1.0 + h * (1.0 / minOverMax - 1.0);
	        g = p / sqrt(Pb / minOverMax / minOverMax + Pr * part * part + Pg);
	        b = (g) / minOverMax;
	        r = (g) + h * ((b) - (g));
	    }
	    // R > B > G
	    else
	    {
	        h = 6.0 * (-h + 6.0 / 6.0);
	        part = 1.0 + h * (1.0 / minOverMax - 1.0);
	        g = p / sqrt(Pr / minOverMax / minOverMax + Pb * part * part + Pg);
	        r = (g) / minOverMax;
	        b = (g) + h * ((r) - (g));
	    }
	}
	else
	{
	    // R > G > B
	    if (h < 1.0 / 6.0)
	    {
	        h = 6.0 * (h - 0.0 / 6.0);
	        r = sqrt(p * p / (Pr + Pg * h * h));
	        g = (r) * h;
	        b = 0.0;
	    }
	    // G > R > B
	    else if (h < 2.0 / 6.0)
	    {
	        h = 6.0 * (-h + 2.0 / 6.0);
	        g = sqrt(p * p / (Pg + Pr * h * h));
	        r = (g) * h;
	        b = 0.0;
	    }
	    // G > B > R
	    else if (h < 3.0 / 6.0)
	    {
	        h = 6.0 * (h - 2.0 / 6.0);
	        g = sqrt(p * p / (Pg + Pb * h * h));
	        b = (g) * h;
	        r = 0.0;
	    }
	    // B > G > R
	    else if (h < 4.0 / 6.0)
	    {
	        h = 6.0 * (-h + 4.0 / 6.0);
	        b = sqrt(p * p / (Pb + Pg * h * h));
	        g = (b) * h;
	        r = 0.0;
	    }
	    // B > R > G
	    else if (h < 5.0 / 6.0)
	    {
	        h = 6.0 * (h - 4.0 / 6.0);
	        b = sqrt(p * p / (Pb + Pr * h * h));
	        r = (b) * h;
	        g = 0.0;
	    }
	    // R > B > G
	    else
	    {
	        h = 6.0 * (-h + 6.0 / 6.0);
	        r = sqrt(p * p / (Pr + Pb * h * h));
	        b = (r) * h;
	        g = 0.0;
	    }
	}
	result[0] = clamp(round(r) / 255.0, 0, 1);
	result[1] = clamp(round(g) / 255.0, 0, 1);
	result[2] = clamp(round(b) / 255.0, 0, 1);
	return result;
}
float3 Lrgb_HSP(float3 input)
{
	float Pr = 0.299, Pg = 0.587, Pb = 0.114;

	float3 _input = input * 255;
	float r = _input[0], g = _input[1], b = _input[2];
	float h = 0, s = 0, p = 0;

	p = sqrt(r * r * Pr + g * g * Pg + b * b * Pb);

	if (r == g && r == b)
	{
		h = 0.0; s = 0.0;
	}
	else
	{
		//R is largest
		if (r >= g && r >= b)
		{
			if (b >= g)
			{
				h = 6.0 / 6.0 - 1.0 / 6.0 * (b - g) / (r - g);
				s = 1.0 - g / r;
			}
			else
			{
				h = 0.0 / 6.0 + 1.0 / 6.0 * (g - b) / (r - b);
				s = 1.0 - b / r;
			}
		}

		//G is largest
		if (g >= r && g >= b)
		{
			if (r >= b)
			{
				h = 2.0 / 6.0 - 1.0 / 6.0 * (r - b) / (g - b);
				s = 1 - b / g;
			}
			else
			{
				h = 2.0 / 6.0 + 1.0 / 6.0 * (b - r) / (g - r);
				s = 1.0 - r / g;
			}
		}

		//B is largest
		if (b >= r && b >= g)
		{
			if (g >= r)
			{
				h = 4.0 / 6.0 - 1.0 / 6.0 * (g - r) / (b - r);
				s = 1.0 - r / b;
			}
			else
			{
				h = 4.0 / 6.0 + 1.0 / 6.0 * (r - g) / (b - g);
				s = 1.0 - g / b;
			}
		}
	}
	return float3(round(h * 360.0), s * 100.0, round(p));
}

//(+|+) [HWB]
float3 HWB_Lrgb(float3 input)
{
	float white = input[1] / 100;
	float black = input[2] / 100;

	if (white + black >= 1)
	{
		float gray = white / (white + black);
		return float3(gray, gray, gray);
	}

	float3 hsb = float3(input[0], 100, 100);
	float3 rgb = HSB_Lrgb(hsb);

	rgb[0] *= (1 - white - black);
	rgb[0] += white;

	rgb[1] *= (1 - white - black);
	rgb[1] += white;

	rgb[2] *= (1 - white - black);
	rgb[2] += white;
	return rgb;
}
float3 Lrgb_HWB(float3 input)
{
	float3 hsb = Lrgb_HSB(input);
	float white = min(input[0], min(input[1], input[2]));
	float black = 1 - max(input[0], max(input[1], input[2]));
	return float3(hsb[0], white * 100, black * 100);
}

//(+|-) [IPT]
float3 IPT_Lrgb(float3 input)
{
	return Multiply(float3x3(float3(0.999779, 1.0709400, 0.324891), float3(1.000150, -0.3777440, 0.220439), float3(0.999769, 0.0629496, -0.809638)), input);
}
float3 Lrgb_IPT(float3 input)
{
	return input;
}

//(+|+) [TSL]
float3 TSL_Lrgb(float3 input)
{
	float T = input[0] / 4, S = input[1], L = input[2];

	float x = tan(2 * pi * (T - 1 / 4));
	x = x * x;

	float r = sqrt(5 * S * S / (9 * (1 / x + 1))) + 1 / 3;
	float g = sqrt(5 * S * S / (9 * (x + 1))) + 1 / 3;

	float k = L / (.185 * r + .473 * g + .114);

	float B = k * (1 - r - g);
	float G = k * g;
	float R = k * r;

	return float3(R, G, B);
}
float3 Lrgb_TSL(float3 input)
{
	float sum = input[0] + input[1] + input[2];
	float x = (sum == 0 ? 0 : input[0] / sum) - 1 / 3;
	float y = (sum == 0 ? 0 : input[1] / sum) - 1 / 3;

	float3 tsl = float3(0, 0, 0);
	tsl[0] = y > 0 ? .5 * atan(x / y) / pi + .25 : y < 0 ? .5 * atan(x / y) / pi + .75 : 0;
	tsl[1] = sqrt(9 / 5 * (x * x + y * y));
	tsl[2] = (input[0] * 0.299) + (input[1] * 0.587) + (input[2] * 0.114);
	return tsl;
}
 
//...

//(+|+) [XYZ]
float3 XYZ_Lrgb(float3 input)
{
	float3x3 m = GetMatrix(XYZ_RGB_x, XYZ_RGB_y, XYZ_RGB_z);
	return Multiply(m, input);
}
float3 Lrgb_XYZ(float3 input)
{
	float3x3 m = GetMatrix(RGB_XYZ_x, RGB_XYZ_y, RGB_XYZ_z);
	return Multiply(m, input);
}

//(+|-) [CAM02]
float inverse_nonlinear_adaptation(float c, float fl) { return (100.0 / fl) * pow((27.13 * abs(c - 0.1)) / (400.0 - abs(c - 0.1)), 1.0 / 0.42); }

float3 CAM02_XYZ(CAM02 input, CAM02Conditions conditions)
{
	float r, g, b;
	float rw = 0, gw = 0, bw = 0;
	float rc = 0, gc = 0, bc = 0;
	float rp, gp, bp;
	float rpa = 0, gpa = 0, bpa = 0;
	float a, ca, cb;
	float et, t;
	float p1, p2, p3, p4, p5, hr;
	float tx = 0, ty = 0, tz = 0;

	float3 rgbw = Multiply(XYZ_CAT02, float3(conditions.xw, conditions.yw, conditions.zw));
	rw = rgbw[0]; gw = rgbw[1]; bw = rgbw[2];

	t = pow(input.C / (sqrt(input.J / 100.0) * pow(1.64 - pow(0.29, conditions.n), 0.73)), (1.0 / 0.9));
	et = (1.0 / 4.0) * (cos(((input.h * pi) / 180.0) + 2.0) + 3.8);

	a = pow(input.J / 100.0, 1.0 / (conditions.c * conditions.z)) * conditions.aw;

	p1 = ((50000.0 / 13.0) * conditions.nc * conditions.ncb) * et / t;
	p2 = (a / conditions.nbb) + 0.305;
	p3 = 21.0 / 20.0;

	hr = (input.h * pi) / 180.0;

	if (abs(sin(hr)) >= abs(cos(hr)))
	{
		p4 = p1 / sin(hr);
		cb = (p2 * (2.0 + p3) * (460.0 / 1403.0)) / (p4 + (2.0 + p3) * (220.0 / 1403.0) * (cos(hr) / sin(hr)) - (27.0 / 1403.0) + p3 * (6300.0 / 1403.0));
		ca = cb * (cos(hr) / sin(hr));
	}
	else
	{
		p5 = p1 / cos(hr);
		ca = (p2 * (2.0 + p3) * (460.0 / 1403.0)) / (p5 + (2.0 + p3) * (220.0 / 1403.0) - ((27.0 / 1403.0) - p3 * (6300.0 / 1403.0)) * (sin(hr) / cos(hr)));
		cb = ca * (sin(hr) / cos(hr));
	}

	float3 rgbpa = Multiply(Aab_RGB, float3((a / conditions.nbb) + 0.305, ca, cb));
	rpa = rgbpa[0]; gpa = rgbpa[1]; bpa = rgbpa[2];

	rp = inverse_nonlinear_adaptation(rpa, conditions.fl);
	gp = inverse_nonlinear_adaptation(gpa, conditions.fl);
	bp = inverse_nonlinear_adaptation(bpa, conditions.fl);

	float3 xyzt = Multiply(HPE_XYZ, float3(rp, gp, bp));
	tx = xyzt[0]; ty = xyzt[1]; tz = xyzt[2];

	float3 rgbc = Multiply(XYZ_CAT02, float3(tx, ty, tz));
	rc = rgbc[0]; gc = rgbc[1]; bc = rgbc[2];

	r = rc / (((conditions.yw * conditions.d) / rw) + (1.0 - conditions.d));
	g = gc / (((conditions.yw * conditions.d) / gw) + (1.0 - conditions.d));
	b = bc / (((conditions.yw * conditions.d) / bw) + (1.0 - conditions.d));

	return Multiply(CAT02_XYZ, float3(r, g, b));
}
CAM02 XYZ_CAM02(float3 input, CAM02Conditions conditions)
{
	CAM02 result;
	result.J = 0; result.Q = 0;
	result.C = 0; result.M = 0; result.s = 0;
	result.h = 0;
	return result;
}

//(+|-) [CAM02] > [JCh]
float3 JCh_Lrgb(CAM02 input, CAM02Conditions conditions)
{
	//JCh > XYZ
	float3 result = CAM02_XYZ(input, conditions);

	//XYZ > Lrgb
	return XYZ_Lrgb(result);
}
float3 Lrgb_JCh(float3 input) { return input; }

//(+|-) [CAM02] > [JMh]
float3 JMh_Lrgb(CAM02 input, CAM02Conditions conditions)
{
	//JMh > XYZ
	float3 result = CAM02_XYZ(input, conditions);

	//XYZ > Lrgb
	return XYZ_Lrgb(result);
}
float3 Lrgb_JMh(float3 input) { return input; }

//(+|-) [CAM02] > [Jsh]
float3 Jsh_Lrgb(CAM02 input, CAM02Conditions conditions)
{
	//Jsh > XYZ
	float3 result = CAM02_XYZ(input, conditions);

	//XYZ > Lrgb
	return XYZ_Lrgb(result);
}
float3 Lrgb_Jsh(float3 input) { return input; }

//(+|-) [CAM02] > [QCh]
float3 QCh_Lrgb(CAM02 input, CAM02Conditions conditions)
{
	//QCh > XYZ
	float3 result = CAM02_XYZ(input, conditions);

	//XYZ > Lrgb
	return XYZ_Lrgb(result);
}
float3 Lrgb_QCh(float3 input) { return input; }

//(+|-) [CAM02] > [QMh]
float3 QMh_Lrgb(CAM02 input, CAM02Conditions conditions)
{ 	
	//QMh > XYZ
	float3 result = CAM02_XYZ(input, conditions);

	//XYZ > Lrgb
	return XYZ_Lrgb(result);
}
float3 Lrgb_QMh(float3 input) { return input; }

//(+|-) [CAM02] > [Qsh]
float3 Qsh_Lrgb(CAM02 input, CAM02Conditions conditions)
{
	//Qsh > XYZ
	float3 result = CAM02_XYZ(input, conditions);

	//XYZ > Lrgb
	return XYZ_Lrgb(result);
}
float3 Lrgb_Qsh(float3 input) { return input; }

//(+|+) [Lab]
float3 Lab_Lrgb(float3 input)
{
	float L = input[0], a = input[1], b = input[2];

	float fy = (L + 16) / 116;
	float fx = a / 500 + fy;
	float fz = fy - b / 200;

	float fx3 = pow(fx, 3);
	float fz3 = pow(fz, 3);

	float xr = fx3 > Epsilon ? fx3 : (116 * fx - 16) / Kappa;
	float yr = L > Kappa * Epsilon ? pow((L + 16) / 116, 3) : L / Kappa;
	float zr = fz3 > Epsilon ? fz3 : (116 * fz - 16) / Kappa;

	float3 w = xy_XYZ(WhiteX, WhiteY);
	return float3(xr * w[0], yr * w[1], zr * w[2]);
}
float3 Lrgb_Lab(float3 input)
{
	/*
	float3 w = xy_XYZ(WhiteX, WhiteY);
	float Xr = w[0], Yr = w[1], Zr = w[2];

	float xr = input[0] / Xr, yr = input[1] / Yr, zr = input[2] / Zr;

	float fx = xr > Epsilon ? pow(xr, 1 / 3) : (Kappa * xr + 16) / 116;
	float fy = yr > Epsilon ? pow(yr, 1 / 3) : (Kappa * yr + 16) / 116;
	float fz = zr > Epsilon ? pow(zr, 1 / 3) : (Kappa * zr + 16) / 116;

	float l = 116 * fy - 16;
	float a = 500 * (fx - fy);
	float b = 200 * (fy - fz);
	*/
	return float3(0, 0, 0);
}

//(+|+) [Lab] > [LCHab]
float3 LCHab_Lrgb(float3 input)
{
	float3 lab = FromLCh(input);
	return Lab_Lrgb(lab);
}
float3 Lrgb_LCHab(float3 input)
{
	float3 lab = Lrgb_Lab(input);
	return ToLCh(lab);
}

//(+|+) [Labh]
float3 Labh_Lrgb(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float L = input[0], a = input[1], b = input[2];
	float xN = white[0], yN = white[1], zN = white[2];

	float Ka = ComputeKa(white);
	float Kb = ComputeKb(white);

	float Y = pow(L / 100, 2) * yN;
	float X = (a / Ka * sqrt(Y / yN) + Y / yN) * xN;
	float Z = (b / Kb * sqrt(Y / yN) - Y / yN) * -zN;

	return XYZ_Lrgb(float3(X, Y, Z));
}
float3 Lrgb_Labh(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float3 xyz = Lrgb_XYZ(input);

	float X = xyz[0], Y = xyz[1], Z = xyz[2];
	float xN = white[0], yN = white[1], zN = white[2];

	float Ka = ComputeKa(white);
	float Kb = ComputeKb(white);

	float L = 100 * sqrt(Y / yN);
	float a = Ka * ((X / xN - Y / yN) / sqrt(Y / yN));
	float b = Kb * ((Y / yN - Z / zN) / sqrt(Y / yN));

	return float3(L, a, b);
}

//(+|+) [Labh] > [LCHabh]
float3 LCHabh_Lrgb(float3 input)
{
	float3 labh = FromLCh(input);
	return Labh_Lrgb(labh);
}
float3 Lrgb_LCHabh(float3 input)
{
	float3 labh = Lrgb_Labh(input);
	return ToLCh(labh);
}

//(+|-) [Labi]
float3 Labi_Lrgb(float3 input) { return input; }
float3 Lrgb_Labi(float3 input) { return input; }

//(+|+) [Labj]
float3 Labj_Lrgb(float3 input)
{
	float Jz = input[0]; float az = input[1]; float bz = input[2];

	Jz = Jz + 1.6295499532821566e-11;
	float Iz = Jz / (0.44 + 0.56 * Jz);

	float L = PerceptualQuantizerInverse(Iz + 1.386050432715393e-1 * az + 5.804731615611869e-2 * bz);
	float M = PerceptualQuantizerInverse(Iz - 1.386050432715393e-1 * az - 5.804731615611891e-2 * bz);
	float S = PerceptualQuantizerInverse(Iz - 9.601924202631895e-2 * az - 8.118918960560390e-1 * bz);

	float X = +1.661373055774069e+00 * L - 9.145230923250668e-01 * M + 2.313620767186147e-01 * S;
	float Y = -3.250758740427037e-01 * L + 1.571847038366936e+00 * M - 2.182538318672940e-01 * S;
	float Z = -9.098281098284756e-02 * L - 3.127282905230740e-01 * M + 1.522766561305260e+00 * S;

	float3 xyz = float3(X / 10000, Y / 10000, Z / 10000);
	return XYZ_Lrgb(xyz);
}
float3 Lrgb_Labj(float3 input)
{
	float3 xyz = Lrgb_XYZ(input);

	float X = xyz[0] * 10000; float Y = xyz[1] * 10000; float Z = xyz[2] * 10000;

	float Lp = PerceptualQuantizer(0.674207838 * X + 0.382799340 * Y - 0.047570458 * Z);
	float Mp = PerceptualQuantizer(0.149284160 * X + 0.739628340 * Y + 0.083327300 * Z);
	float Sp = PerceptualQuantizer(0.070941080 * X + 0.174768000 * Y + 0.670970020 * Z);

	float Iz = 0.5 * (Lp + Mp);

	float az = 3.524000 * Lp - 4.066708 * Mp + 0.542708 * Sp;
	float bz = 0.199076 * Lp + 1.096799 * Mp - 1.295875 * Sp;
	float Jz = 0.44 * Iz / (1 - 0.56 * Iz) - 1.6295499532821566e-11;

	return float3(Jz, az, bz);
}

//(+|+) [Labj] > [LCHabj]
float3 LCHabj_Lrgb(float3 input)
{
	float3 jzazbz = FromLCh(input);
	return Labj_Lrgb(jzazbz);
}
float3 Lrgb_LCHabj(float3 input)
{
	float3 jzazbz = Lrgb_Labj(input);
	return ToLCh(jzazbz);
}

//(+|+) [Labk]
float3 Labk_Lrgb(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float3x3 m = GetMatrix(LABk_LMSk_x, LABk_LMSk_y, LABk_LMSk_z);
	float3x3 n = GetMatrix(LMSk_XYZk_x, LMSk_XYZk_y, LMSk_XYZk_z);

	//Labk > LMS
	float3 u = Multiply(m, input);
	float3 v = float3(Pow3(u[0]), Pow3(u[1]), Pow3(u[2]));

	//LMS > XYZ
	float3 xyz = Multiply(n, v) * white;

	//XYZ > Lrgb
	return XYZ_Lrgb(xyz);
}
float3 Lrgb_Labk(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float3x3 m = GetMatrix(XYZk_LMSk_x, XYZk_LMSk_y, XYZk_LMSk_z);
	float3x3 n = GetMatrix(LMSk_LABk_x, LMSk_LABk_y, LMSk_LABk_z);

	//Lrgb > XYZ
	float3 xyz = Lrgb_XYZ(input);
	xyz /= white;

	//XYZ > LMS
	float3 u = Multiply(m, xyz);
	float3 v = float3(Cbrt(u[0]), Cbrt(u[1]), Cbrt(u[2]));

	//LMS > Labk
	return Multiply(n, v);
}

//(-|-) [Labk] > [Labksb]
float3 Labksb_Lrgb(float3 input) { return HSB_Lrgb(input); }
float3 Lrgb_Labksb(float3 input) { return Lrgb_HSB(input); }

//(-|-) [Labk] > [Labksl]
float3 Labksl_Lrgb(float3 input) { return HSL_Lrgb(input); }
float3 Lrgb_Labksl(float3 input) { return Lrgb_HSL(input); }

//(-|-) [Labk] > [Labkwb]
float3 Labkwb_Lrgb(float3 input) { return HWB_Lrgb(input); }
float3 Lrgb_Labkwb(float3 input) { return Lrgb_HWB(input); }

//(+|+) [Luv]
float3 Luv_Lrgb(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float L = input[0], u = input[1], v = input[2];

	float u0 = ComputeKu(white);
	float v0 = ComputeKv(white);

	float Y = L > Kappa * Epsilon ? pow((L + 16) / 116, 3) : L / Kappa;

	float a = (52 * L / (u + 13 * L * u0) - 1) / 3;
	float b = -5 * Y;
	float c = -1 / 3;
	float d = Y * (39 * L / (v + 13 * L * v0) - 5);

	float X = (d - b) / (a - c);
	float Z = X * a + b;

	if (X < 0)
		X = 0;

	if (Y < 0)
		Y = 0;

	if (Z < 0)
		Z = 0;

	return XYZ_Lrgb(float3(X, Y, Z));
}
float3 Lrgb_Luv(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float3 xyz = Lrgb_XYZ(input);

	float yr = input[1] / white[1];
	float up = ComputeKu(input);
	float vp = ComputeKv(input);

	float upr = ComputeKu(white);
	float vpr = ComputeKv(white);

	float L = yr > Epsilon ? 116 * pow(yr, 1 / 3) - 16 : Kappa * yr;

	if (L < 0)
		L = 0;

	float u = 0;
	u = 13 * L * (up - upr);

	float v = 0;
	v = 13 * L * (vp - vpr);

	return float3(L, u, v);
}

//(+|+) [Luv] > [LCHuv]
float3 LCHuv_Lrgb(float3 input)
{
	float3 luv = FromLCh(input);
	return Luv_Lrgb(luv);
}
float3 Lrgb_LCHuv(float3 input)
{
	float3 luv = Lrgb_Luv(input);
	return ToLCh(luv);
}

//(+|-) [Luv] > [LCHuv] > [HPLuv]
float3 HPLuv_Lrgb(float3 input)
{
	//HPLuv > LCHuv
	float H = input[0], S = input[1], L = input[2];

	if (L > 99.9999999)
		return float3(100, 0, H);

	if (L < 0.00000001)
		return float3(0, 0, H);

	float max = GetChroma1(L);
	float C = max / 100 * S;

	//LCHuv > Lrgb
	float3 lch = float3(L, C, H);
	return LCHuv_Lrgb(lch);
}
float3 Lrgb_HPLuv(float3 input)
{
	return Lrgb_HSL(input);
	/*
	//Lrgb > LCHuv
	float3 lch = Lrgb_LCHuv(input);
	float L = lch[0], C = lch[1], H = lch[2];

	//LCHuv > HPLuv
	if (L > 99.9999999)
	{
		return float3(H, 0, 100);
	}

	if (L < 0.00000001)
	{
		return float3(H, 0, 0);
	}

	float max = GetChroma(L);
	float S = C / max * 100;

	return float3(H, S, L);
	*/
}

//(+|-) [Luv] > [LCHuv] > [HSLuv]
float3 HSLuv_Lrgb(float3 input)
{
	//HSLuv > LCHuv
	float H = input[0], S = input[1], L = input[2];

	if (L > 99.9999999)
		return float3(100, 0, H);

	if (L < 0.00000001)
		return float3(0, 0, H);

	float max = GetChroma2(L, H);
	float C = max / 100 * S;

	//LCHuv > Lrgb
	float3 lch = float3(L, C, H);
	return LCHuv_Lrgb(lch);
}
float3 Lrgb_HSLuv(float3 input)
{
	return Lrgb_HSL(input);
	/*
	//Lrgb > LCHuv
	float3 lch = Lrgb_LCHuv(input);
	float L = lch[0], C = lch[1], H = lch[2];

	//LCHuv > HSLuv
	if (L > 99.9999999)
	{
		return float3(H, 0, 100);
	}

	if (L < 0.00000001)
	{
		return float3(H, 0, 0);
	}

	float max = GetChroma(L, H);
	float S = C / max * 100;

	return float3(H, S, L);
	*/
}

//(+|+) [LMS]
float3 LMS_Lrgb(float3 input)
{
	float3x3 m = GetMatrix(LMS_XYZ_x, LMS_XYZ_y, LMS_XYZ_z);

	float3 xyz = Multiply(m, input);
	return XYZ_Lrgb(xyz);
}
float3 Lrgb_LMS(float3 input)
{
	float3x3 m = GetMatrix(XYZ_LMS_x, XYZ_LMS_y, XYZ_LMS_z);

	float3 xyz = Lrgb_XYZ(input);
	return Multiply(m, xyz);
}

//(+|-) [RCA]
float3 RCA_Lrgb(float3 input)
{
	input /= 255;
	float r = input.x, c = input.y, a = input.z;
	return float3(0.75 * r + 0.25 * c, 0.75 * c + 0.25 * a, 0.75 * a + 0.25 * r);
}
float3 Lrgb_RCA(float3 input)
{
	return input;
}

//(+|-) [RGBK]
float3 RGBK_Lrgb(float4 input)
{
	//input = input / 255;
	float r = input[0] * (1 - input[3]);
	float g = input[1] * (1 - input[3]);
	float b = input[2] * (1 - input[3]);
	return float3(r, g, b);
}
float4 Lrgb_RGBK(float3 input)
{
	return float4(0, 0, 0, 0);
}

//(+|-) [RGBW]
float3 RGBW_Lrgb(float4 input)
{
	float r = input[0]; /// 255;
	float g = input[1]; /// 255;
	float b = input[2]; /// 255;
	float w = input[3];

	r *= (1 - w);
	r += w;

	g *= (1 - w);
	g += w;

	b *= (1 - w);
	b += w;

	return float3(r, g, b);
}
float4 Lrgb_RGBW(float3 input)
{
	return float4(0, 0, 0, 0);
}

//(+|+) [rgG]
float3 rgG_Lrgb(float3 input)
{
	return xyY_XYZ(input);
}
float3 Lrgb_rgG(float3 input)
{
	return XYZ_xyY(input);
}

//(+|-) [rgG] > LCHrg
float3 LCHrg_Lrgb(float3 input)
{
	//LCHrg > Lab
	float3 result = FromLCh(input);

	//Lab > rgG
	result = float3(result.y, result.z, result.x);
	result = (result + float3(100, 100, 0)) / float3(200, 200, 100);

	//rgG > Lrgb
	return rgG_Lrgb(result);
}
float3 Lrgb_LCHrg(float3 input)
{
	float3 result = Lrgb_rgG(input);
	return ToLCh(float3(result.z, result.x, result.y));
}

//(+|-) [RGV]
float3 RGV_Lrgb(float3 input)
{
	input /= 255;
	float r = input.x, g = input.y, v = input.z;
	return float3(0.75 * r + 0.25 * v, 0.75 * g + 0.25 * r, 0.75 * v + 0.25 * g);
}
float3 Lrgb_RGV(float3 input)
{
	return input;
}

//(+|-) [RYB]
float3 RYB_Lrgb(float3 input)
{
	float r = input.x; float y = input.y; float b = input.z;

	float white = min(r, min(y, b));

	r -= white;
	y -= white;
	b -= white;

	float mY = max(r, max(y, b));

	float g = min(y, b);

	y -= g;
	b -= g;

	if (b > 0 && g > 0)
	{
		b *= 2.0;
		g *= 2.0;
	}

	r += y;
	g += y;

	float mG = max(r, max(g, b));

	if (mG > 0)
	{
		float mN = mY / mG;

		r *= mN;
		g *= mN;
		b *= mN;
	}

	r += white;
	g += white;
	b += white;

	return float3(floor(r) / 255, floor(g) / 255, floor(b) / 255);
}
float3 Lrgb_RYB(float3 input) { return input; }

//(+|+) [UCS]
float3 UCS_Lrgb(float3 input)
{
	float u = input[0], v = input[1], w = input[2];
	return XYZ_Lrgb(float3(1.5 * u, v, 1.5 * u - 3 * v + 2 * w));
}
float3 Lrgb_UCS(float3 input)
{
	float3 xyz = Lrgb_XYZ(input);
	float x = xyz[0], y = xyz[1], z = xyz[2];
	return float3(x * 2 / 3, y, 0.5 * (-x + 3 * y + z));
}

//(+|+) [UVW]
float3 UVW_Lrgb(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float _u, _v, w, u, v, x, y, z, xN, yN, zN, uN, vN;
	u = input[0]; v = input[1]; w = input[2];

	if (w == 0)
		return float3(0, 0, 0);

	xN = white[0]; yN = white[1]; zN = white[2];
	uN = (4 * xN) / (xN + (15 * yN) + (3 * zN));
	vN = (6 * yN) / (xN + (15 * yN) + (3 * zN));

	y = pow((w + 17) / 25, 3);

	_u = 13 * w == 0 ? 0 : u / (13 * w) + uN;
	_v = 13 * w == 0 ? 0 : v / (13 * w) + vN;

	x = (6 / 4) * y * _u / _v;
	z = y * (2 / _v - 0.5 * _u / _v - 5);

	float3 xyz = float3(x / 100, y / 100, z / 100);
	return XYZ_Lrgb(xyz);
}
float3 Lrgb_UVW(float3 input)
{
	float3 white = xy_XYZ(WhiteX, WhiteY);

	float3 xyz = Lrgb_XYZ(input);

	float x = xyz[0] * 100, y = xyz[1] * 100, z = xyz[2] * 100, xN, yN, zN, uN, vN;

	xN = white[0]; yN = white[1]; zN = white[2];
	uN = (4 * xN) / (xN + (15 * yN) + (3 * zN));
	vN = (6 * yN) / (xN + (15 * yN) + (3 * zN));

	float uv = x + 15 * y + 3 * z;
	float _u = uv == 0 ? 0 : 4 * x / uv;
	float _v = uv == 0 ? 0 : 6 * y / uv;

	float w = 25 * pow(y, 1 / 3) - 17;
	float u = 13 * w * (_u - uN);
	float v = 13 * w * (_v - vN);
	return float3(u, v, w);
}

//(+|+) [xyY]
float3 xyY_Lrgb(float3 input)
{
	return XYZ_Lrgb(xyY_XYZ(input));
}
float3 Lrgb_xyY(float3 input)
{
	return XYZ_xyY(Lrgb_XYZ(input));
}

//(+|+) [xyY] > [LCHxy]
float3 LCHxy_Lrgb(float3 input)
{
	//LCHxy > Lab
	float3 result = FromLCh(input);

	//Lab > xyY
	result = float3(result.y, result.z, result.x);
	result = (result + float3(100, 100, 0)) / float3(200, 200, 100);

	//xyY > Lrgb
	return xyY_Lrgb(result);
}
float3 Lrgb_LCHxy(float3 input)
{
	float3 result = Lrgb_xyY(input);
	return ToLCh(float3(result.z, result.x, result.y));
}

//(-|-) [xyY] > [xyYC]
float3 xyYC_Lrgb(float3 input)
{
	//xyYC > xyY
	float A = input[0], T = input[1], V = input[2];

	float3 white = xy_XYZ(WhiteX, WhiteY) * 100;
	float Xn = white.x, Yn = white.y, Zn = white.z;

	float y0 = Xn / (Xn + Yn + Zn);
	float x0 = Yn / (Xn + Yn + Zn);
	float ew = (Xn + Yn + Zn) / 100;

	float yl = xyYC_exy.z, el = xyYC_exy.x, xl = xyYC_exy.y;

	float Y = V * V / 100;

	float Yl = yl * el * 100;

	float x = (100 * Y * x0 * ew + 100 * xl * el * T - Yl * T * x0 * ew) / (100 * T * el - Yl * T * ew + 100 * Y * ew);
	float y = (100 * Y + 100 * T * yl * el - Yl * T) / (Y * ew * 100 + T * 100 * el - T * Yl * ew);

	float3 result = float3(x, y, Y);

	//xyY > *
	return xyY_Lrgb(result);
}
float3 Lrgb_xyYC(float3 input)
{
	//* > xyY
	float3 xyy = Lrgb_xyY(input);

	//xyY > xyYC
	return xyy; //To do...
}

//(+|+) [YCoCg]
float3 YCoCg_Lrgb(float3 input)
{
	float y = input[0], cg = input[1], co = input[2];

	float c = y - cg;
	return float3(c + co, y + cg, c - co);
}
float3 Lrgb_YCoCg(float3 input)
{
	float r = input[0], g = input[1], b = input[2];
	return float3(0.25 * r + 0.5 * g + 0.25 * b, -0.25 * r + 0.5 * g - 0.25 * b, 0.5 * r - 0.5 * b);
}

//(+|+) [YDbDr]
float3 YDbDr_Lrgb(float3 input)
{
	float y = input[0], db = input[1], dr = input[2];

	float r = y + 0.000092303716148 * db - 0.525912630661865 * dr;
	float g = y - 0.129132898890509 * db + 0.267899328207599 * dr;
	float b = y + 0.664679059978955 * db - 0.000079202543533 * dr;
	return float3(r, g, b);
}
float3 Lrgb_YDbDr(float3 input)
{
	float r = input[0], g = input[1], b = input[2];
	return float3
	(
		 0.299 * r + 0.587 * g + 0.114 * b,
		-0.450 * r - 0.883 * g + 1.333 * b,
		-1.333 * r + 1.116 * g + 0.217 * b
	);
}

//(+|+) [YES]
float3 YES_Lrgb(float3 input)
{
	float y = input[0], e = input[1], s = input[2];

	float3x3 m = float3x3
	(
		float3(1,  1.431,  0.126),
		float3(1, -0.569,  0.126),
		float3(1,  0.431, -1.874)
	);

	float
		r = y * m[0][0] + e * m[0][1] + s * m[0][2],
		g = y * m[1][0] + e * m[1][1] + s * m[1][2],
		b = y * m[2][0] + e * m[2][1] + s * m[2][2];

	return float3(r, g, b);
}
float3 Lrgb_YES(float3 input)
{
	float r = input[0], g = input[1], b = input[2];

	float3x3 m = float3x3
	(
		float3(0.253,  0.684,  0.063),
		float3(0.500, -0.500,  0),
		float3(0.250,  0.250, -0.500)
	);

	return float3(r * m[0][0] + g * m[0][1] + b * m[0][2], r * m[1][0] + g * m[1][1] + b * m[1][2], r * m[2][0] + g * m[2][1] + b * m[2][2]);
}

//(+|+) [YIQ]
float3 YIQ_Lrgb(float3 input)
{
	float y = input[0], i = input[1], q = input[2], r, g, b;
	r = (y * 1) + (i *  0.956) + (q *  0.621);
	g = (y * 1) + (i * -0.272) + (q * -0.647);
	b = (y * 1) + (i * -1.108) + (q *  1.705);

	r = min(max(0, r), 1);
	g = min(max(0, g), 1);
	b = min(max(0, b), 1);
	return float3(r, g, b);
}
float3 Lrgb_YIQ(float3 input)
{
	float r = input[0], g = input[1], b = input[2];

	float y = (r * 0.299) + (g * 0.587) + (b * 0.114);
	float i = 0, q = 0;

	if (r != g || g != b)
	{
		i = (r * 0.596) + (g * -0.275) + (b * -0.321);
		q = (r * 0.212) + (g * -0.528) + (b * 0.311);
	}
	return float3(y, i, q);
}

//(+|+) [YPbPr]
float3 YPbPr_Lrgb(float3 input)
{
	float y = input[0], pb = input[1], pr = input[2];

	float kb = 0.0722;
	float kr = 0.2126;

	float r = y + 2 * pr * (1 - kr);
	float b = y + 2 * pb * (1 - kb);
	float g = (y - kr * r - kb * b) / (1 - kr - kb);

	return float3(r, g, b);
}
float3 Lrgb_YPbPr(float3 input)
{
	float r = input[0], g = input[1], b = input[2];

	float kb = 0.0722;
	float kr = 0.2126;

	float y = kr * r + (1 - kr - kb) * g + kb * b;
	float pb = 0.5 * (b - y) / (1 - kb);
	float pr = 0.5 * (r - y) / (1 - kr);

	return float3(y, pb, pr);
}

//(+|+) [YPbPr] > [xvYCC]
float3 xvYCC_Lrgb(float3 input)
{
	//xvYCC > YPbPr
	float y = input[0], cb = input[1], cr = input[2];
	float3 ypbpr = float3((y - 16) / 219, (cb - 128) / 224, (cr - 128) / 224);

	//YPbPr > RGB
	return YPbPr_Lrgb(ypbpr);
}
float3 Lrgb_xvYCC(float3 input)
{
	//RGB > YPbPr
	float3 ypbpr = Lrgb_YPbPr(input);

	//YPbPr > xvYCC
	float y = ypbpr[0], pb = ypbpr[1], pr = ypbpr[2];
	return float3(16 + 219 * y, 128 + 224 * pb, 128 + 224 * pr);
}

//(+|+) [YPbPr] > [YCbCr]
float3 YCbCr_Lrgb(float3 input)
{
	//YCbCr > YPbPr
	float y = input[0], cb = input[1], cr = input[2];
	float3 ypbpr = float3((y - 16) / 219, (cb - 128) / 224, (cr - 128) / 224);

	//YPbPr > RGB
	return YPbPr_Lrgb(ypbpr);
}
float3 Lrgb_YCbCr(float3 input)
{
	//RGB > YPbPr
	float3 ypbpr = Lrgb_YPbPr(input);

	//YPbPr > YCbCr
	float y = ypbpr[0], pb = ypbpr[1], pr = ypbpr[2];
	return float3(16 + 219 * y, 128 + 224 * pb, 128 + 224 * pr);
}

//(+|+) [YPbPr] > [YCbCr] > [JPEG]
float3 JPEG_Lrgb(float3 input)
{
	//JPEG > YCbCr
	float y = input[0], cb = input[1], cr = input[2];
	float3 ycbcr = float3(y + 1.402 * (cr - 128), y - 0.34414 * (cb - 128) - 0.71414 * (cr - 128), y + 1.772 * (cb - 128));

	//YCbCr > YPbPr > RGB
	return YCbCr_Lrgb(ycbcr);
}
float3 Lrgb_JPEG(float3 input)
{
	//RGB > YPbPr > YCbCr
	float3 ycbcr = Lrgb_YCbCr(input);

	//YCbCr > JPEG
	float r = ycbcr[0], g = ycbcr[1], b = ycbcr[2];
	return float3(0.299 * r + 0.587 * g + 0.114 * b, 128 - 0.168736 * r - 0.331264 * g + 0.5 * b, 128 + 0.5 * r - 0.418688 * g - 0.081312 * b);
}

//(+|+) [YUV]
float3 YUV_Lrgb(float3 input)
{
	float
		y = input[0],
		u = input[1],
		v = input[2],
		r, g, b;

	r = (y * 1)
		+ (u * 0)
		+ (v * 1.13983);
	g = (y * 1)
		+ (u * -0.39465)
		+ (v * -0.58060);
	b = (y * 1)
		+ (u * 2.02311)
		+ (v * 0);

	r = min(max(0, r), 1);
	g = min(max(0, g), 1);
	b = min(max(0, b), 1);

	return float3(r, g, b);
}
float3 Lrgb_YUV(float3 input)
{
	float
		r = input[0],
		g = input[1],
		b = input[2];

	float y = (r * 0.299)
		+ (g * 0.587)
		+ (b * 0.114);
	float u = (r * -0.14713)
		+ (g * -0.28886)
		+ (b * 0.436);
	float v = (r * 0.615)
		+ (g * -0.51499)
		+ (b * -0.10001);

	return float3(y, u, v);
}

//...

//[RGB] > [Lrgb] = [Non-Linear] > [Linear]

float TransferInverse(float input)
{
	//(0) Default
		 if (Companding == 0)
	{
		float V = input;
		float v = V <= Compression_E ? V / Compression_D : pow((V + Compression_B) / (Compression_B + 1), Compression_A);
		return v;
	}

	//(0) Gamma
	else if (Companding == 1)
	{
		float V = input;
		float v = pow(V, Compression_A);
		return v;
	}

	//(1) Gamma-Log
	else if (Companding == 2)
	{
		float a = 0.17883277;
		float b = 1 - 4 * a;
		float c = 0.5 - a * log(4 * a);

		if (input >= 0 && input <= 1 / 12)
			return sqrt(3 * input);

		return a * log(12 * input - b) + c; //1 / 12 < input <= 1
	}

	//(3) PQ
	else if (Companding == 3)
	{
		float c2 = 18.8515625;
		float c3 = 18.6875;
		float c1 = c3 - c2 + 1;

		float m1 = 0.1593017578125;
		float m2 = 78.84375;

		float a = c1 + c2 * pow(input, m1);
		float b =  1 + c3 * pow(input, m1);
		return pow(a / b, m2);
	}
	return input;
}

float3 RGB_Lrgb(float3 input)
{
	return float3(TransferInverse(input[0]), TransferInverse(input[1]), TransferInverse(input[2]));
}

//[Lrgb] > [RGB] = [Linear] > [Non-Linear]

float Transfer(float input)
{
	//(0) Default
		 if (Companding == 0)
	{
        float v = input;
		float V = v <= Compression_C ? Compression_D * v : (Compression_B + 1) * pow(v, 1 / Compression_A) - Compression_B;
        return V;
	}

	//(1) Gamma
	else if (Companding == 1)
	{
		float v = input;
		float V = pow(v, 1 / Compression_A);
		return V;
	}

	//(2) Gamma-Log
	else if (Companding == 2)
	{
		float r = 0.5;

		float a = 0.17883277;
		float b = 1 - 4 * a;
		float c = 0.5 - a * log(4 * a);

		if (input >= 0 && input <= 1)
			return r * sqrt(input);

		return a * log(input - b) + c; //1 < input
	}

	//(3) PQ
	else if (Companding == 3)
	{
		float c2 = 18.8515625;
		float c3 = 18.6875;
		float c1 = c3 - c2 + 1;

		float m1 = 0.1593017578125;
		float m2 = 78.84375;

		float a = max(pow(input, 1 / m2) - c1, 0);
		float b = c2 - c3 * pow(input, 1 / m2);
		return 10000 * pow(a / b, 1 / m1);
	}
	return input;
}

float3 Lrgb_RGB(float3 input)
{
	return float3(Transfer(input[0]), Transfer(input[1]), Transfer(input[2]));
}

//[Lrgb] > [*]

float3 FLrgb(float m, float3 input)
{
		 if (m == 0)  { return Lrgb_RCA(input); }
	else if (m == 2)  { return Lrgb_RGV(input); }
	else if (m == 3)  { return Lrgb_RYB(input); }
	else if (m == 4)  { return Lrgb_CMY(input); }
	else if (m == 5)  { return Lrgb_HCV(input); }
	else if (m == 6)  { return Lrgb_HCY(input); }
	else if (m == 7)  { return Lrgb_HPLuv(input); }
	else if (m == 8)  { return Lrgb_HSB(input); }
	else if (m == 9)  { return Lrgb_HSL(input); }

		 if (m == 10) { return Lrgb_HSLuv(input); }
	else if (m == 11) { return Lrgb_HSM(input); }
	else if (m == 12) { return Lrgb_HSP(input); }
	else if (m == 13) { return Lrgb_HWB(input); }
	else if (m == 14) { return Lrgb_IPT(input); }
	else if (m == 15) { return Lrgb_JCh(input); }
	else if (m == 16) { return Lrgb_JMh(input); }
	else if (m == 17) { return Lrgb_Jsh(input); }
	else if (m == 18) { return Lrgb_JPEG(input); }
	else if (m == 19) { return Lrgb_Lab(input); }

		 if (m == 20) { return Lrgb_Labh(input); }
	else if (m == 21) { return Lrgb_Labj(input); }
	else if (m == 22) { return Lrgb_Labk(input); }
	else if (m == 23) { return Lrgb_Labksb(input); }
	else if (m == 24) { return Lrgb_Labksl(input); }
	else if (m == 25) { return Lrgb_Labkwb(input); }
	else if (m == 26) { return Lrgb_LCHab(input); }
	else if (m == 27) { return Lrgb_LCHabh(input); }
	else if (m == 28) { return Lrgb_LCHabj(input); }
	else if (m == 29) { return Lrgb_LCHrg(input); }

		 if (m == 30) { return Lrgb_LCHuv(input); }
	else if (m == 31) { return Lrgb_LCHxy(input); }
	else if (m == 32) { return Lrgb_LMS(input); }
	else if (m == 33) { return Lrgb_Luv(input); }
	else if (m == 34) { return Lrgb_QCh(input); }
	else if (m == 35) { return Lrgb_QMh(input); }
	else if (m == 36) { return Lrgb_Qsh(input); }
	else if (m == 37) { return Lrgb_rgG(input); }
	else if (m == 38) { return Lrgb_TSL(input); }
	else if (m == 39) { return Lrgb_UCS(input); }

		 if (m == 40) { return Lrgb_UVW(input); }
	else if (m == 41) { return Lrgb_xvYCC(input); }
	else if (m == 42) { return Lrgb_xyY(input); }
	else if (m == 43) { return Lrgb_xyYC(input); }
	else if (m == 44) { return Lrgb_XYZ(input); }
	else if (m == 45) { return Lrgb_YCbCr(input); }
	else if (m == 46) { return Lrgb_YCoCg(input); }
	else if (m == 47) { return Lrgb_YDbDr(input); }
	else if (m == 48) { return Lrgb_YES(input); }
	else if (m == 49) { return Lrgb_YIQ(input); }

		 if (m == 50) { return Lrgb_YPbPr(input); }
	else if (m == 51) { return Lrgb_YUV(input); }

	return input;
}

//[*] > [Lrgb]

float3 TLrgb(float m, float3 input3)
{
		 if (m == 0)  { return RCA_Lrgb(input3); }
	else if (m == 2)  { return RGV_Lrgb(input3); }
	else if (m == 3)  { return RYB_Lrgb(input3); }
	else if (m == 4)  { return CMY_Lrgb(input3); }
	else if (m == 5)  { return HCV_Lrgb(input3); }
	else if (m == 6)  { return HCY_Lrgb(input3); }
	else if (m == 7)  { return HPLuv_Lrgb(input3); }
	else if (m == 8)  { return HSB_Lrgb(input3); }
	else if (m == 9)  { return HSL_Lrgb(input3); }
	
		 if (m == 10) { return HSLuv_Lrgb(input3); }
	else if (m == 11) { return HSM_Lrgb(input3); }
	else if (m == 12) { return HSP_Lrgb(input3); }
	else if (m == 13) { return HWB_Lrgb(input3); }
	else if (m == 14) { return IPT_Lrgb(input3); }
	//(15-17)
	else if (m == 18) { return JPEG_Lrgb(input3); }
	else if (m == 19) { return Lab_Lrgb(input3); }

		 if (m == 20) { return Labh_Lrgb(input3); }
	else if (m == 21) { return Labj_Lrgb(input3); }
	else if (m == 22) { return Labk_Lrgb(input3); }
	else if (m == 23) { return Labksb_Lrgb(input3); }
	else if (m == 24) { return Labksl_Lrgb(input3); }
	else if (m == 25) { return Labkwb_Lrgb(input3); }
	else if (m == 26) { return LCHab_Lrgb(input3); }
	else if (m == 27) { return LCHabh_Lrgb(input3); }
	else if (m == 28) { return LCHabj_Lrgb(input3); }
	else if (m == 29) { return LCHrg_Lrgb(input3); }

		 if (m == 30) { return LCHuv_Lrgb(input3); }
	else if (m == 31) { return LCHxy_Lrgb(input3); }
	else if (m == 32) { return LMS_Lrgb(input3); }
	else if (m == 33) { return Luv_Lrgb(input3); }
	//(34-36)
	else if (m == 37) { return rgG_Lrgb(input3); }
	else if (m == 38) { return TSL_Lrgb(input3); }
	else if (m == 39) { return UCS_Lrgb(input3); }

		 if (m == 40) { return UVW_Lrgb(input3); }
	else if (m == 41) { return xvYCC_Lrgb(input3); }
	else if (m == 42) { return xyY_Lrgb(input3); }
	else if (m == 43) { return xyYC_Lrgb(input3); }
	else if (m == 44) { return XYZ_Lrgb(input3); }
	else if (m == 45) { return YCbCr_Lrgb(input3); }
	else if (m == 46) { return YCoCg_Lrgb(input3); }
	else if (m == 47) { return YDbDr_Lrgb(input3); }
	else if (m == 48) { return YES_Lrgb(input3); }
	else if (m == 49) { return YIQ_Lrgb(input3); }

		 if (m == 50) { return YPbPr_Lrgb(input3); }
	else if (m == 51) { return YUV_Lrgb(input3); }

	return input3;
}

float3 TLrgb(float m, float4 input)
{
		 if (m == 52) { return CMYK_Lrgb(input); }
	else if (m == 53) { return CMYW_Lrgb(input); }
	else if (m == 54) { return RGBK_Lrgb(input); }
	else if (m == 55) { return RGBW_Lrgb(input); }
	return float3(0, 0, 0);
}

float3 TLrgb(float m, CAM02 input, CAM02Conditions conditions)
{
		 if (m == 15) { return JCh_Lrgb(input, conditions); }
	else if (m == 16) { return JMh_Lrgb(input, conditions); }
	else if (m == 17) { return Jsh_Lrgb(input, conditions); }

	else if (m == 34) { return QCh_Lrgb(input, conditions); }
	else if (m == 35) { return QMh_Lrgb(input, conditions); }
	else if (m == 36) { return Qsh_Lrgb(input, conditions); }
	return float3(0, 0, 0);
}

//[*] > [Lrgb] > [RGB]

float3 ToRGB(float m, float3 input)
{
	//* > Lrgb
	float3 result = TLrgb(m, input);

	//Lrgb > RGB
	return Lrgb_RGB(result);
}

float3 ToRGB(float m, float4 input)
{
	//* > Lrgb
	float3 result = TLrgb(m, input);

	//Lrgb > RGB
	return Lrgb_RGB(result);
}

float3 ToRGB(float m, CAM02 input, CAM02Conditions conditions)
{
	//* > Lrgb
	float3 result = TLrgb(m, input, conditions);

	//Lrgb > RGB
	return Lrgb_RGB(result);
}

//[RGB] > [Lrgb] > [*]

float3 ToXYZ(float m, float3 input)
{
	//RGB > Lrgb
	float3 result = RGB_Lrgb(input);

	//Lrgb > *
	return FLrgb(m, result);
}

//...

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	float4 color = tex2D(input, uv.xy);

	float3 input3 = { 0, 0, 0 }; float4 input4 = { 0, 0, 0, 0 };
	float3 output;

	float3 min3, max3; float4 min4, max4;

	float M, N;

	bool is4 = Model >= Total3;

	if (is4)
	{
		min4 = Minimum4[Total3 - Model];
		max4 = Maximum4[Total3 - Model];

		input4[0] = X; input4[1] = Y; input4[2] = Z; input4[3] = W;
	}
	else
	{
		min3 = Minimum3[Model];
		max3 = Maximum3[Model];

		input3[0] = X; input3[1] = Y; input3[2] = Z;
	}
	
	float xC = XComponent;
	float yC = YComponent;

	//X
	if (Mode == 2)
	{
		//Square
		N = 1 - uv.xy.y;

		//Circle
		if (Shape == 1)
		{
			N = 1 - GetDistance(0.5, 0.5, uv.xy.x, uv.xy.y) / GetDistance(0.5, 0.5, 1, 1);
		}

		if (xC == 0)
		{
			if (is4) { input4[0] = N; } else { input3[0] = N; }
		}
		else if (xC == 1)
		{
			if (is4) { input4[1] = N; } else { input3[1] = N; }
		}
		else if (xC == 2)
		{
			if (is4) { input4[2] = N; } else { input3[2] = N; }
		}
		else if (xC == 3)
		{
			if (is4) { input4[3] = N; }
		}
	}
	//XY
	else if (Mode == 1)
	{
		//Square
		M = uv.xy.x; N = 1 - uv.xy.y;

		//Circle
		if (Shape == 1)
		{
			float xN = 1 - GetDistance(0.5, 0.5, M, N) / GetDistance(0.5, 0.5, 1, 1);
			float yN = atan2(N - 0.5, M - 0.5) + GetRadian(270/*Replace with variable later...*/);
			yN = yN + pi;
			yN = yN > pi2 ? yN - pi2 : yN;
			yN /= pi2;

			M = xN;
			N = yN;
		}

		if (xC == 0)
		{
			if (is4) { input4[0] = M; } else { input3[0] = M; }
		}
		else if (xC == 1)
		{
			if (is4) { input4[1] = M; } else { input3[1] = M; }
		}
		else if (xC == 2)
		{
			if (is4) { input4[2] = M; } else { input3[2] = M; }
		}
		else if (xC == 3)
		{
			if (is4) { input4[3] = M; }
		}

		if (yC == 0)
		{
			if (is4) { input4[0] = N; } else { input3[0] = N; }
		}
		else if (yC == 1)
		{
			if (is4) { input4[1] = N; } else { input3[1] = N; }
		}
		else if (yC == 2)
		{
			if (is4) { input4[2] = N; } else { input3[2] = N; }
		}
		else if (yC == 3)
		{
			if (is4) { input4[3] = N; }
		}
	}
	//XYZ
	else if (Mode == 0)
	{
		//(1) Convert (rgb) > (xyz)
		float3 rgb = color.rgb;
		float3 xyz = ToXYZ(Model, rgb);

		//(2) Increase (current value) of each component
		float l = Lrgb_HSB(rgb)[2] / 100;

		float tx, ty, tz;
		//S = Shadow
		if (l <= ShadowRange)
		{
			tx = ShadowAmount * X / 100;
			ty = ShadowAmount * Y / 100;
			tz = ShadowAmount * Z / 100;
		}
		//M = Midtone
		else if (l <= MidtoneRange)
		{
			tx = MidtoneAmount * X / 100;
			ty = MidtoneAmount * Y / 100;
			tz = MidtoneAmount * Z / 100;
		}
		//H = Highlight
		else if (l <= HighlightRange)
		{
			tx = HighlightAmount * X / 100;
			ty = HighlightAmount * Y / 100;
			tz = HighlightAmount * Z / 100;
		}

		xyz = FLerp(xyz, min3, max3, float3(tx, ty, tz));

		//(3) Convert (xyz) > (rgb)
		rgb = ToRGB(Model, xyz);

		color.rgb = rgb;
		return color;
	}

	if 
	(
		//JCh, JMh, Jsh
		Model == 15 || Model == 16 || Model == 17
		||  
		//QCh, QMh, Qsh
	    Model == 34 || Model == 35 || Model == 36
	)
	{
		CAM02 input6;
		input6.h = input3.z;

			 if (Model == 15)
		{
			input6.J = input3.x; input6.C = input3.y; 
			input6.Q = 1; input6.M = 1; input6.s = 1; //?
		}
		else if (Model == 16)
		{
			input6.J = input3.x; input6.M = input3.y;
			input6.Q = 1;  input6.C = 1; input6.s = 1; //?
		}
		else if (Model == 17)
		{
			input6.J = input3.x; input6.s = input3.y; 
			input6.Q = 1; input6.C = 1; input6.M = 1; //?
		}
		else if (Model == 34)
		{
			input6.Q = input3.x; input6.C = input3.y;
			input6.J = 1;  input6.M = 1; input6.s = 1; //?
		}
		else if (Model == 35)
		{
			input6.Q = input3.x; input6.M = input3.y;
			input6.J = 1; input6.C = 1;  input6.s = 1; //?
		}
		else if (Model == 36)
		{
			input6.Q = input3.x; input6.s = input3.y;
			input6.J = 1; input6.C = 1; input6.M = 1; //?
		}

		//To do: Conditions need passed! The following is temporary and allows shader to compile...
		CAM02Conditions conditions;
		conditions.xw = 1;
		conditions.yw = 1; conditions.zw = 1; conditions.aw = 1;
		conditions.la = 1; conditions.yb = 1;
		conditions.surround = 1;
		conditions.n = 1; conditions.z = 1; conditions.f = 1;
		conditions.c = 1;
		conditions.nbb = 1;
		conditions.nc = 1;
		conditions.ncb = 1; conditions.fl = 1; conditions.d = 1;

		output = ToRGB(Model, input6, conditions);
	}
	//CMYK, CMYW, RGBK, RGBW
	else if (is4)
	{
		input4 = Depth > 0 ? round(input4 * Depth) / Depth : input4;
		//input4 = ConvertRange(input4, min4, max4);

		output = ToRGB(Model, input4);
	}
	//Everything else!
	else
	{
		input3 = Depth > 0 ? round(input3 * Depth) / Depth : input3;
		input3 = ConvertRange(input3, min3, max3);

		output = ToRGB(Model, input3);
	}

	color = float4(output, 1);
	return color;
}