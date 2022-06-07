// There is a limitation with increasing the size of the arrays {Maximum} and {Minimum}: 
// "error X4505: maximum temp register index exceeded". 
// 
// The shader has presumably exhausted all available memory. This suggests no more color models (and, by extension, variables) can be defined. 
// To support additional color spaces, additional shaders are required. I haven't done this yet to avoid redundancy (copy/paste). 
//  
// [Labk], [HSBk], and [HSLk] are omitted temporarily to overcome this limitation. The conversion for the latter two color spaces are 
//  not yet defined anyway. 
// 
// Assess alternatives...

sampler2D input				: register(S0);

//...

float Model				: register(C0);
float Component			: register(C1);

float Mode				: register(C2);
float View				: register(C22);

float X					: register(C3);
float Y					: register(C4);
float Z					: register(C5);

float Companding		: register(C6);
float Gamma				: register(C7);

float WhiteX			: register(C8);
float WhiteY			: register(C9);

float3 LMS_XYZ_x		: register(C10);
float3 LMS_XYZ_y		: register(C11);
float3 LMS_XYZ_z		: register(C12);

float3 RGB_XYZ_x		: register(C13);
float3 RGB_XYZ_y		: register(C14);
float3 RGB_XYZ_z		: register(C15);

float3 XYZ_LMS_x		: register(C16);
float3 XYZ_LMS_y		: register(C17);
float3 XYZ_LMS_z		: register(C18);

float3 XYZ_RGB_x		: register(C19);
float3 XYZ_RGB_y		: register(C20);
float3 XYZ_RGB_z		: register(C21);

//...

float3 LABk_LMSk_x		: register(C23);
float3 LABk_LMSk_y		: register(C24);
float3 LABk_LMSk_z		: register(C25);

float3 LMSk_LABk_x		: register(C26);
float3 LMSk_LABk_y		: register(C27);
float3 LMSk_LABk_z		: register(C28);

float3 LMSk_XYZk_x		: register(C29);
float3 LMSk_XYZk_y		: register(C30);
float3 LMSk_XYZk_z		: register(C31);

float3 XYZk_LMSk_x		: register(C32);
float3 XYZk_LMSk_y		: register(C33);
float3 XYZk_LMSk_z		: register(C34);

//...

float3  xyYC_exy		: register(C35);

//...

float HighlightAmount	: register(C36);
float HighlightRange	: register(C37);

float MidtoneAmount		: register(C38);
float MidtoneRange		: register(C39);

float ShadowAmount		: register(C40);
float ShadowRange		: register(C41);

//[Constants]

static float pi = 3.1415926535897932384626433832795028841971693993751058209749445923078164062;
static float pi2 = pi * 2;
static float pi3 = pi * 3;

static float Epsilon = 216.0 / 24389.0;
static float Kappa = 24389.0 / 27.0;

static float MaxValue = 3.40282347E+38;

static float3 Maximum[43] =
{
	//RGB
	float3(1, 1, 1),
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
	//HSBk
	float3(360, 100, 100),
	//HSL
	float3(360, 100, 100),
	//HSLk
	float3(360, 100, 100),
	//HSLuv
	float3(360, 100, 100),
	//HSM
	float3(360, 100, 255),
	//HSP
	float3(360, 100, 255),
	//HWB
	float3(360, 100, 100),
	//HWBk
	float3(360, 100, 100),
	//ICtCp
	float3(1, 1, 1),
	//IPT
	float3(1, 1, 1),
	//JPEG
	float3(255, 255, 255),
	//Lab
	float3(100, 100, 100),
	//Labh
	float3(100, 128, 128),
	//Labi
	float3(8, 12, 6),
	//Labj
	float3(1, 1, 1),
	//Labk
	float3(1, 1, 1),
	//LCHab
	float3(100, 100, 360),
	//LCHabh
	float3(100, 100, 360),
	//LCHabj
	float3(1, 1, 360),
	//LCHuv
	float3(100, 100, 360),
	//LMS
	float3(1, 1, 1),
	//Luv
	float3(100, 224, 122),
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
	//YCwCm
	float3(1, 1, 1),
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

static float3 Minimum[43] =
{
	//RGB
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
	//HSBk
	float3(0, 0, 0),
	//HSL
	float3(0, 0, 0),
	//HSLk
	float3(0, 0, 0),
	//HSLuv
	float3(0, 0, 0),
	//HSM
	float3(0, 0, 0),
	//HSP
	float3(0, 0, 0),
	//HWB
	float3(0, 0, 0),
	//HWBk
	float3(0, 0, 0),
	//ICtCp
	float3(0, -1, -1),
	//IPT
	float3(0, 0, 0),
	//JPEG
	float3(0, 0, 0),
	//Lab
	float3(0, 0, 0),
	//Labh
	float3(0, -128, -128),
	//Labi
	float3(-10, -6, -10),
	//Labj
	float3(0, -1, -1),
	//Labk
	float3(0, 0, 0),
	//LCHab
	float3(0, 0, 0),
	//LCHabh
	float3(0, 0, 0),
	//LCHabj
	float3(0, 0, 0),
	//LCHuv
	float3(0, 0, 0),
	//LMS
	float3(0, 0, 0),
	//Luv
	float3(0, -134, -140),
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
	//YCwCm
	float3(0, 0, 0),
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

static float Cbrt(float input) { return pow(input, 1 / 3); }

static float ConvertRange(float value, float minimum, float maximum) { return (value * (maximum - minimum)) + minimum; }

static float3 ConvertRange(float3 value, float3 minimum, float3 maximum)
{
	value[0] = ConvertRange(value[0], minimum[0], maximum[0]);
	value[1] = ConvertRange(value[1], minimum[1], maximum[1]);
	value[2] = ConvertRange(value[2], minimum[2], maximum[2]);
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

static float GetChroma2(int i, float L, float result)
{
	float2x2 xB = GetBounds1(0, L);

	float m1 = xB[i][0];
	float b1 = xB[i][1];

	float x = GetIntersection(float2(m1, b1), float2(-1 / m1, 0));
	float length = GetDistance(float2(x, b1 + x * m1));

	result = min(result, length);
	return result;
}

static float GetChroma1(float L)
{
	float result = MaxValue;
	result = GetChroma2(0, L, result);
	result = GetChroma2(1, L, result);
	return result;
}

static float GetChroma4(float2 xy, float hrad, float result)
{
	float length = xy.y / (sin(hrad) - xy.x * cos(hrad));
	if (length >= 0)
		result = min(result, length);

	return result;
}

static float GetChroma3(float L, float H)
{
	float hrad = H / 360 * pi2;

	float2x2 x = GetBounds1(0, L);
	float2x2 y = GetBounds1(1, L);
	float2x2 z = GetBounds1(2, L);

	float result = MaxValue;

	result = GetChroma4(x[0], hrad, result);
	result = GetChroma4(x[1], hrad, result);

	result = GetChroma4(y[0], hrad, result);
	result = GetChroma4(y[1], hrad, result);

	result = GetChroma4(z[0], hrad, result);
	result = GetChroma4(z[1], hrad, result);
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

static float3 GetWhite(float x, float y)
{
	return float3((1 / y) * x, 1, (1 / y) * (1 - x - y));
}

static float3 GetWhite(float2 input) { return GetWhite(input.x, input.y); }

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

static float Pow2(float input) { return pow(input, 2); }

static float Pow3(float input) { return pow(input, 3); }

//...

static float ComputeKa(float3 input)
{
	float3 cW = GetWhite(0.31006, 0.31616);
	if (input[0] == cW[0] && input[1] == cW[1] && input[2] == cW[2]) //C (2°)
		return 175;

	float Ka = 100 * (175 / 198.04) * (input[0] + input[1]);
	return Ka;
}

static float ComputeKb(float3 input)
{
	float3 cW = GetWhite(0.31006, 0.31616);
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

//(+|+) [HWb]
float3 ToHWb(float3 input)
{
	float h = input[0];
	float w = (1 - input[1]) * input[2];
	float b = 1 - input[2];
	return float3(h, w, b);
}
float3 FromHWb(float3 input)
{
	float h = input[0];
	float s = 1 - (input[1] / (1 - input[2]));
	float b = 1 - input[2];
	return float3(h, s, b);
}

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

	float _h = input[0] / 360, _s = input[1] / 100, _b = input[2] / 100;
	float r = 0, g = 0, b = 0;
	
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

//(+|+) [HSB] > [HWB]
float3 HWB_Lrgb(float3 input)
{	
	//HWB > HSB
	float3 hsb = FromHWb(input);

	//HSB > *
	return HSB_Lrgb(hsb);
	/*
	float3 result = { 0, 0, 0 };

	float white = input[1] / 100;
	float black = input[2] / 100;

	if (white + black >= 1)
	{
		float gray = white / (white + black);
		return float3(gray, gray, gray);
	}

	float3 hsl = float3(input[0], 100, 50);
	float3 rgb = HSL_Lrgb(hsl);

	rgb[0] *= (1 - white - black);
	rgb[0] += white;

	rgb[1] *= (1 - white - black);
	rgb[1] += white;

	rgb[2] *= (1 - white - black);
	rgb[2] += white;
	*/
}
float3 Lrgb_HWB(float3 input)
{	
	//* > HSB
	input = Lrgb_HSB(input);

	//HSB > HWB
	return ToHWb(input);
	/*
	float3 hsl = Lrgb_HSL(input);
	float white = min(input[0], min(input[1], input[2]));
	float black = 1 - max(input[0], max(input[1], input[2]));
	return float3(hsl[0], white * 100, black * 100);
	*/
}

//(+|+) [HSL]
float3 HSL_Lrgb(float3 input)
{
	float h = input[0], s = input[1] / 100, l = input[2] / 100;
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
	float3 max = Maximum[Model];

	float h = input[0] / max[0], s = input[1] / max[1], m = input[2] / max[2];
	float r, g, b;

	float u = cos(h);
	float v = s * u;
	float w = sqrt(41);

	r = (3 / 41) * v + m - (4 / 861 * sqrt(861 * pow(s, 2) * (1 - pow(u, 2))));
	g = (w * v + (23 * m) - (19 * r)) / 4;
	b = ((11 * r) - (9 * m) - (w * v)) / 2;

	return float3(r, g, b);
}
float3 Lrgb_HSM(float3 input)
{
	float3 max = Maximum[Model];

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

//(-|-) [ICtCp]
float3 ICtCp_Lrgb(float3 input)
{
	return float3(input[0], (input[1] + 1) / 2, (input[2] + 1) / 2);
}
float3 Lrgb_ICtCp(float3 input)
{
	return float3(input[0], (input[1] * 2) - 1, (input[2] * 2) - 1);
}

//(-|-) [IPT]
float3 IPT_Lrgb(float3 input)
{
	return input;
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

	float3 w = GetWhite(WhiteX, WhiteY);
	return float3(xr * w[0], yr * w[1], zr * w[2]);
}
float3 Lrgb_Lab(float3 input)
{
	float3 w = GetWhite(WhiteX, WhiteY);
	float Xr = w[0], Yr = w[1], Zr = w[2];

	float xr = input[0] / Xr, yr = input[1] / Yr, zr = input[2] / Zr;

	float fx = xr > Epsilon ? pow(xr, 1 / 3) : (Kappa * xr + 16) / 116;
	float fy = yr > Epsilon ? pow(yr, 1 / 3) : (Kappa * yr + 16) / 116;
	float fz = zr > Epsilon ? pow(zr, 1 / 3) : (Kappa * zr + 16) / 116;

	float l = 116 * fy - 16;
	float a = 500 * (fx - fy);
	float b = 200 * (fy - fz);
	return float3(l, a, b);
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
	float3 white = GetWhite(WhiteX, WhiteY);

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
	float3 white = GetWhite(WhiteX, WhiteY);

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
	float3 white = GetWhite(WhiteX, WhiteY);

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
	float3 white = GetWhite(WhiteX, WhiteY);

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

//(-|-) [Labk] > [HSBk]
float3 HSBk_Lrgb(float3 input)
{
	return HSB_Lrgb(input);
}
float3 Lrgb_HSBk(float3 input)
{
	return Lrgb_HSB(input);
}

//(+|+) [Labk] > [HSBk] > [HWBk]
float3 HWBk_Lrgb(float3 input)
{
	//HWBk > HSBk
	float3 hsb = FromHWb(input);

	//HSBk > *
	return HSBk_Lrgb(hsb);
}
float3 Lrgb_HWBk(float3 input)
{
	//* > HSBk
	input = Lrgb_HSBk(input);

	//HSBk > HWBk
	return ToHWb(input);
}

//(-|-) [Labk] > [HSLk]
float3 HSLk_Lrgb(float3 input)
{
	return HSL_Lrgb(input);
}
float3 Lrgb_HSLk(float3 input)
{
	return Lrgb_HSL(input);
}

//(+|+) [Luv]
float3 Luv_Lrgb(float3 input)
{
	float3 white = GetWhite(WhiteX, WhiteY);

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
	float3 white = GetWhite(WhiteX, WhiteY);

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

	float max = GetChroma3(L, H);
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
	float3 white = GetWhite(WhiteX, WhiteY);

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
	float3 white = GetWhite(WhiteX, WhiteY);

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
	float x = input[0]; float y = input[1]; float Y = input[2];
	if (y == 0)
	{
		return float3(0, 0, 0);
	}
	return XYZ_Lrgb(float3(x * Y / y, Y, (1 - x - y) * Y / y));
}
float3 Lrgb_xyY(float3 input)
{
	float3 xyz = Lrgb_XYZ(input);
	float sum, X, Y, Z;
	X = xyz[0]; Y = xyz[1]; Z = xyz[2];

	sum = X + Y + Z;
	if (sum == 0)
	{
		return float3(0, 0, Y);
	}
	return float3(X / sum, Y / sum, Y);
}

//(-|-) [xyY] > [xyYC]
float3 xyYC_Lrgb(float3 input)
{
	//xyYC > xyY
	float A = input[0], T = input[1], V = input[2];

	float3 white = GetWhite(WhiteX, WhiteY);
	float Xn = white.x, Yn = white.y, Zn = white.z;

	float yM = Xn / (Xn + Yn + Zn);
	float xM = Yn / (Xn + Yn + Zn);
	float zM = (Xn + Yn + Zn) / 100;

	float xL = xyYC_exy.z, yL = xyYC_exy.x, zL = xyYC_exy.y;

	float Y = V * V / 100;

	float xyL = xL * yL * 100;

	float x = (100 * Y * xM * zM + 100 * zL * yL * T - xyL * T * xM * zM) / (100 * T * yL - xyL * T * zM + 100 * Y * zM);
	float y = (100 * Y + 100 * T * xL * yL - xyL * T) / (Y * zM * 100 + T * 100 * yL - T * xyL * zM);

	float3 xyy = float3(x, y, Y);

	//xyY > *
	return xyY_Lrgb(xyy);
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

//(-|-) [YCwCm]
float3 YCwCm_Lrgb(float3 input)
{
	return input;
}
float3 Lrgb_YCwCm(float3 input)
{
	return input;
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

//[RGB] > [Lrgb] = [Non-Linear] > [Linear]

float CompandInverse(float channel)
{
	//(0) Gamma
	if (Companding == 0)
	{
		float v = channel;
		float V = pow(v, 1 / Gamma);
		return V;
	}

	//(1) L
	if (Companding == 1)
	{
		float v = channel;
		float V = v <= Epsilon ? v * Kappa / 100 : pow(1.16 * v, 1 / 3) - 0.16;
		return V;
	}

	//(2) Rec709
	if (Companding == 2)
	{
		float V = channel;
		float L = V < 0.081 ? V / 4.5 : pow((V + 0.099) / 1.099, 1 / 0.45);
		return L;
	}

	//(3) Rec2020
	if (Companding == 3)
	{
		float V = channel;
		float L = V < 0.08145 ? V / 4.5 : pow((V + 0.0993) / 1.0993, 1 / 0.45);
		return L;
	}

	//(4) sRGB
	if (Companding == 4)
	{
		float V = channel;
		float v = V <= 0.04045 ? V / 12.92 : pow((V + 0.055) / 1.055, 2.4);
		return v;
	}
	return channel;
}

float3 RGB_Lrgb(float3 input)
{
	return float3(CompandInverse(input[0]), CompandInverse(input[1]), CompandInverse(input[2]));
}

//[Lrgb] > [RGB] = [Linear] > [Non-Linear]

float Compand(float channel)
{
	//(0) Gamma
	if (Companding == 0)
	{
		float V = channel;
		float v = pow(V, Gamma);
		return v;
	}

	//(1) L
	if (Companding == 1)
	{
		float V = channel;
		float v = V <= 0.08 ? 100 * V / Kappa : pow((V + 0.16) / 1.16, 3);
		return v;
	}

	//(2) Rec709
	if (Companding == 2)
	{
		float L = channel;
		float V = L < 0.018 ? 4500 * L : 1.099 * L - 0.099;
		return V;
	}

	//(3) Rec2020
	if (Companding == 3)
	{
		float L = channel;
		float V = L < 0.0181 ? 4500 * L : 1.0993 * L - 0.0993;
		return V;
	}

	//(4) sRGB
	if (Companding == 4)
	{
		float v = channel;
		float V = v <= 0.0031308 ? 12.92 * v : 1.055 * pow(v, 1 / 2.4) - 0.055;
		return V;
	}
	return channel;
}

float3 Lrgb_RGB(float3 input)
{
	return float3(Compand(input[0]), Compand(input[1]), Compand(input[2]));
}

//[Lrgb] > [*]

float3 FLrgb(float m, float3 input)
{
	if (m == 1)  { return Lrgb_CMY(input); }
	if (m == 2)  { return Lrgb_HCV(input); }
	if (m == 3)  { return Lrgb_HCY(input); }
	if (m == 4)  { return Lrgb_HPLuv(input); }
	if (m == 5)  { return Lrgb_HSB(input); }
	if (m == 6)  { return Lrgb_HSBk(input); }
	if (m == 7)  { return Lrgb_HSL(input); }
	if (m == 8)  { return Lrgb_HSLk(input); }
	if (m == 9)  { return Lrgb_HSLuv(input); }
	if (m == 10) { return Lrgb_HSM(input); }
	if (m == 11) { return Lrgb_HSP(input); }
	if (m == 12) { return Lrgb_HWB(input); }
	if (m == 13) { return Lrgb_HWBk(input); }
	if (m == 14) { return Lrgb_ICtCp(input); }
	if (m == 15) { return Lrgb_IPT(input); }
	if (m == 16) { return Lrgb_JPEG(input); }
	if (m == 17) { return Lrgb_Lab(input); }
	if (m == 18) { return Lrgb_Labh(input); }
	if (m == 19) { return Lrgb_Labi(input); }
	if (m == 20) { return Lrgb_Labj(input); }
	if (m == 21) { return Lrgb_Labk(input); }
	if (m == 22) { return Lrgb_LCHab(input); }
	if (m == 23) { return Lrgb_LCHabh(input); }
	if (m == 24) { return Lrgb_LCHabj(input); }
	if (m == 25) { return Lrgb_LCHuv(input); }
	if (m == 26) { return Lrgb_LMS(input); }
	if (m == 27) { return Lrgb_Luv(input); }
	if (m == 28) { return Lrgb_TSL(input); }
	if (m == 29) { return Lrgb_UCS(input); }
	if (m == 30) { return Lrgb_UVW(input); }
	if (m == 31) { return Lrgb_xvYCC(input); }
	if (m == 32) { return Lrgb_xyY(input); }
	if (m == 33) { return Lrgb_xyYC(input); }
	if (m == 34) { return Lrgb_XYZ(input); }
	if (m == 35) { return Lrgb_YCbCr(input); }
	if (m == 36) { return Lrgb_YCoCg(input); }
	if (m == 37) { return Lrgb_YCwCm(input); }
	if (m == 38) { return Lrgb_YDbDr(input); }
	if (m == 39) { return Lrgb_YES(input); }
	if (m == 40) { return Lrgb_YIQ(input); }
	if (m == 41) { return Lrgb_YPbPr(input); }
	if (m == 42) { return Lrgb_YUV(input); }
	return input;
}

//[*] > [Lrgb]

float3 TLrgb(float m, float3 input)
{
	if (m == 1)  { return CMY_Lrgb(input); }
	if (m == 2)  { return HCV_Lrgb(input); }
	if (m == 3)  { return HCY_Lrgb(input); }
	if (m == 4)  { return HPLuv_Lrgb(input); }
	if (m == 5)  { return HSB_Lrgb(input); }
	if (m == 6)  { return HSBk_Lrgb(input); }
	if (m == 7)  { return HSL_Lrgb(input); }
	if (m == 8)  { return HSLk_Lrgb(input); }
	if (m == 9)  { return HSLuv_Lrgb(input); }
	if (m == 10) { return HSM_Lrgb(input); }
	if (m == 11) { return HSP_Lrgb(input); }
	if (m == 12) { return HWB_Lrgb(input); }
	if (m == 13) { return HWBk_Lrgb(input); }
	if (m == 14) { return ICtCp_Lrgb(input); }
	if (m == 15) { return IPT_Lrgb(input); }
	if (m == 16) { return JPEG_Lrgb(input); }
	if (m == 17) { return Lab_Lrgb(input); }
	if (m == 18) { return Labh_Lrgb(input); }
	if (m == 19) { return Labi_Lrgb(input); }
	if (m == 20) { return Labj_Lrgb(input); }
	if (m == 21) { return Labk_Lrgb(input); }
	if (m == 22) { return LCHab_Lrgb(input); }
	if (m == 23) { return LCHabh_Lrgb(input); }
	if (m == 24) { return LCHabj_Lrgb(input); }
	if (m == 25) { return LCHuv_Lrgb(input); }
	if (m == 26) { return LMS_Lrgb(input); }
	if (m == 27) { return Luv_Lrgb(input); }
	if (m == 28) { return TSL_Lrgb(input); }
	if (m == 29) { return UCS_Lrgb(input); }
	if (m == 30) { return UVW_Lrgb(input); }
	if (m == 31) { return xvYCC_Lrgb(input); }
	if (m == 32) { return xyY_Lrgb(input); }
	if (m == 33) { return xyYC_Lrgb(input); }
	if (m == 34) { return XYZ_Lrgb(input); }
	if (m == 35) { return YCbCr_Lrgb(input); }
	if (m == 36) { return YCoCg_Lrgb(input); }
	if (m == 37) { return YCwCm_Lrgb(input); }
	if (m == 38) { return YDbDr_Lrgb(input); }
	if (m == 39) { return YES_Lrgb(input); }
	if (m == 40) { return YIQ_Lrgb(input); }
	if (m == 41) { return YPbPr_Lrgb(input); }
	if (m == 42) { return YUV_Lrgb(input); }
	return input;
}

//[*] > [Lrgb] > [RGB]

float3 ToRGB(float m, float3 input)
{
	//* > Lrgb
	float3 result = TLrgb(m, input);

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

	//[0, 1]
	float x, y, z;

	float3 output; float3 input;

	float c = Component;
	float m = Model;

	float3 min = Minimum[Model], max = Maximum[Model];

	//XYZ
	if (Mode == 0)
	{
		//(1) Convert (rgb) > (xyz)
		float3 rgb = color.rgb;
		float3 xyz = ToXYZ(m, rgb);

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
		
		xyz = FLerp(xyz, min, max, float3(tx, ty, tz));

		//(3) Convert (xyz) > (rgb)
		rgb = ToRGB(m, xyz);

		color.rgb = rgb;
		return color;
	}
	//XY
	if (Mode == 1)
	{
		x = uv.xy.x; y = 1 - uv.xy.y; z = Z;

		//Square
		if (View == 0) { }

		//Circle
		if (View == 1)
		{
			float xN = 1 - GetDistance(0.5, 0.5, x, y) / GetDistance(0.5, 0.5, 1, 1);
			float yN = atan2(y - 0.5, x - 0.5) + GetRadian(270/*Replace with variable later...*/);
			yN = yN + pi;
			yN = yN > pi2 ? yN - pi2 : yN;
			yN /= pi2;

			x = xN;
			y = yN;
		}
	}
	//Z
	if (Mode == 2)
	{
		x = X; y = Y; z = 1 - uv.xy.y;
	}

	if (c == 0)
	{
		input[0] = z; input[1] = x; input[2] = y;
	}
	if (c == 1)
	{
		input[0] = x; input[1] = z; input[2] = y;
	}
	if (c == 2)
	{
		input[0] = x; input[1] = y; input[2] = z;
	}

	input = ConvertRange(input, min, max);
	output = ToRGB(m, input);

	color.r = output[0]; color.g = output[1]; color.b = output[2];
	return color;
}