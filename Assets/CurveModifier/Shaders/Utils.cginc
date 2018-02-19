
#define PI 3.141592653589
#define PI2 6.283185307179

float oscillation (float t, float speed)
{
 return sin(t * speed) * 0.5 + 0.5;
}

float segment(float amount, float segments)
{
	return floor(amount * segments) / segments;
}

float2 pixelize(float2 uv, float segments)
{
	return floor(uv * segments) / segments;
}

float2 pixelize(float2 uv, float2 segments)
{
	return floor(uv * segments) / segments;
}

float4 posterize ( float4 color, float segments )
{
	return float4(floor(color.rgb * segments) / segments, 1.0);
}

float2 videoUV (float2 uv)
{
	return float2(uv.x, 1.0 - uv.y);
}

float2 wrapUV (float2 uv)
{
	return fmod(abs(uv), 1.0);
}

float2 kaleidoGrid(float2 p)
{
	return fmod(lerp(p, 1.0 - p, float2(step(fmod(p, 2.0), float2(1.0, 1.0)))), 1.0);
}

float kaleido (float d, float t)
{
  d += t * lerp(-1.0, 1.0, fmod(floor(abs(d)), 2.0));
  float dMod = fmod(abs(d), 1.0);
  return lerp(1.0 - dMod, dMod, fmod(floor(abs(d)), 2.0));
}

float2 mouseFromCenter (float2 mouse, float2 resolution)
{
	mouse /= resolution;
	mouse = (mouse - float2(0.5, 0.5)) * 2.0;
	mouse.y *= -1.0;
	return mouse;
}

float luminance ( float3 color )
{
	return (color.r + color.g + color.b) / 3.0;
}

float reflectance(float3 a, float3 b)
{
	return dot(normalize(a), normalize(b)) * 0.5 + 0.5;
}

half4 filter (sampler2D bitmap, float2 uv, float2 dimension)
{
  half4 color = half4(0.0, 0.0, 0.0, 0.0);

  color += -1.0 * tex2D(bitmap, uv + float2(-2, -2) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-2, -1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-2,  0) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-2,  1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-2,  2) / dimension);

  color += -1.0 * tex2D(bitmap, uv + float2(-1, -2) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-1, -1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-1,  0) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-1,  1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2(-1,  2) / dimension);

  color += -1.0 * tex2D(bitmap, uv + float2( 0, -2) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 0, -1) / dimension);
  color += 24.0 * tex2D(bitmap, uv + float2( 0,  0) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 0,  1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 0,  2) / dimension);

  color += -1.0 * tex2D(bitmap, uv + float2( 1, -2) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 1, -1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 1,  0) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 1,  1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 1,  2) / dimension);

  color += -1.0 * tex2D(bitmap, uv + float2( 2, -2) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 2, -1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 2,  0) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 2,  1) / dimension);
  color += -1.0 * tex2D(bitmap, uv + float2( 2,  2) / dimension);

  return color;
}

half4 cheesyBlur (sampler2D bitmap, float2 uv, float2 dimension)
{
  half4 color = half4(0.0, 0.0, 0.0, 0.0);
  color += 0.2 * tex2D(bitmap, uv + float2(0, 0) / dimension);
  color += 0.2 * tex2D(bitmap, uv + float2(0,  -1) / dimension);
  color += 0.2 * tex2D(bitmap, uv + float2( -1, 0) / dimension);
  color += 0.2 * tex2D(bitmap, uv + float2( 0,  1) / dimension);
  color += 0.2 * tex2D(bitmap, uv + float2( 1,  0) / dimension);
  return color;
}

float2 lightDirection (sampler2D bitmap, float2 uv, float2 dimension)
{ 
  float2 force = float2(0.0, 0.0);
  float3 c = tex2D(bitmap, uv).rgb;
  float l = Luminance(c);

  c = tex2D(bitmap, uv - float2(1.0, 0.0) / dimension).rgb;
  force.x += Luminance(c) - l;

  c = tex2D(bitmap, uv + float2(1.0, 0.0) / dimension).rgb;
  force.x += l - Luminance(c);

  c = tex2D(bitmap, uv - float2(0.0, 1.0) / dimension).rgb;
  force.y += Luminance(c) - l;

  c = tex2D(bitmap, uv + float2(0.0, 1.0) / dimension).rgb;
  force.y += l - Luminance(c);

  return force;
  // return normalize(force);
}

float2 lightDirectionUnit (sampler2D bitmap, float2 uv, float2 dimension)
{ 
  float2 force = float2(0.0, 0.0);
  float3 c = tex2D(bitmap, uv).rgb;
  float l = Luminance(c);

  c = tex2D(bitmap, uv - float2(1.0, 0.0) / dimension).rgb;
  force.x += step(l, Luminance(c)) * 2 - 1;

  c = tex2D(bitmap, uv + float2(1.0, 0.0) / dimension).rgb;
  force.x += step(Luminance(c), l) * 2 - 1;

  c = tex2D(bitmap, uv - float2(0.0, 1.0) / dimension).rgb;
  force.y += step(l, Luminance(c)) * 2 - 1;

  c = tex2D(bitmap, uv + float2(0.0, 1.0) / dimension).rgb;
  force.y += step(Luminance(c), l) * 2 - 1;

  return force;
  // return normalize(force);
}

float grid2D (float x, float lineDistance, float lineThickness)
{
  return step(fmod(x, lineDistance), lineThickness);
}

float dentDeScie (float x)
{
  return 1.0 - abs(fmod(abs(x), 1.0) * 2.0 - 1.0);
}

float vectorToNumber1 (float3 v)
{
  return ((v.x + v.y + v.z) / 3.0) * 0.5 + 0.5;
}

float vectorToNumber2 (float3 v)
{
  return (atan2(v.y, v.x) * 0.5 / PI + 0.5 + atan2(v.z, v.x) * 0.5 / PI + 0.5 ) / 2.0;
}

float vectorToNumber3 (float normal1, float normal2)
{
  return 1.0 - dot(normal1, normal2) * 0.5 + 0.5;
}

// http://stackoverflow.com/questions/12964279/whats-the-origin-of-this-glsl-rand-one-liner
float rand(float2 co)
{
  return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
}

// From Anton Roy -> https://www.shadertoy.com/view/Xs23DG
float4 filter5x5 (float filter[25], sampler2D bitmap, float2 uv, float2 dimension)
{
  float4 color = float4(0.0, 0.0, 0.0, 0.0);
  for (int i = 0; i < 5; ++i)
  for (int j = 0; j < 5; ++j)
  color += filter[i * 5 + j] * tex2D(bitmap, uv + float2(i - 2, j - 2) / dimension);
  return color;
}

float3 rotateY(float3 v, float t)
{
  float cost = cos(t); float sint = sin(t);
  return float3(v.x * cost + v.z * sint, v.y, -v.x * sint + v.z * cost);
}

float3 rotateX(float3 v, float t)
{
  float cost = cos(t); float sint = sin(t);
  return float3(v.x, v.y * cost - v.z * sint, v.y * sint + v.z * cost);
}

float3 getNormal(float3 a, float3 b, float3 c)
{
  float3 u = b - a;
  float3 v = c - a;
  float3 normal = float3(1.0, 0.0, 0.0);
  normal.x = u.y * v.z - u.z * v.y;
  normal.y = u.z * v.x - u.x * v.z;
  normal.z = u.x * v.y - u.y * v.x;
  return normalize(normal);
}

// hash based 3d value noise
// function taken from https://www.shadertoy.com/view/XslGRr
// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// ported from GLSL to HLSL
float hash( float n )
{
  return frac(sin(n)*43758.5453);
}

float noiseIQ( float3 x )
{
  // The noise function returns a value in the range -1.0f -> 1.0f
  float3 p = floor(x);
  float3 f = frac(x);
  f       = f*f*(3.0-2.0*f);
  float n = p.x + p.y*57.0 + 113.0*p.z;
  return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
  lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
}

// by Inigo Quilez

float sphere( float3 p, float s )
{
  return length(p)-s;
}

float addition( float d1, float d2 )
{
  return min(d1,d2);
}

float substraction( float d1, float d2 )
{
  return max(-d1,d2);
}

float intersection( float d1, float d2 )
{
  return max(d1,d2);
}

float3 grid(float3 p, float3 size)
{
  return fmod(p, size) - size * 0.5;
}


// Sam Hocevar
float3 rgb2hsv(float3 c)
{
  float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
  float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
  float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

  float d = q.x - min(q.w, q.y);
  float e = 1.0e-10;
  return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Sam Hocevar
float3 hsv2rgb(float3 c)
{
  float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
  return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Many thanks to  Brandon Pelfrey
// https://github.com/brandonpelfrey/complex-function-plot/blob/master/index.htm

#define M_E 2.7182818284

float arg(float2 x) { return atan2(x.y, x.x); }
float complex_r(float x, float y) { return length(float2(x,y)); }
float complex_theta(float x, float y) { return arg(float2(x,y)); }
float2 complex(float x, float y) { return float2(x,y); }

// https://en.wikipedia.org/wiki/Complex_number#Elementary_operations
float2 complex_add(float2 x, float2 y) { return x + y; }
float2 complex_sub(float2 x, float2 y) { return x - y; }
float2 complex_mul(float2 x, float2 y) { return float2( x.x*y.x-x.y*y.y, x.y*y.x+x.x*y.y); }
float2 complex_div(float2 x, float2 y) { return float2( (x.x*y.x+x.y*y.y)/(y.x*y.x+y.y*y.y), (x.y*y.x-x.x*y.y)/(y.x*y.x+y.y*y.y)); }

// http://www.abecedarical.com/zenosamples/zs_complexnumbers.html
float2 complex_pow(float2 x, float2 y) {
  float rho = length(x);
  float theta = arg(x);
  float angle = y.x * theta + y.y * log(rho);
  float real = cos(angle);
  float imag = sin(angle);
  return float2(real, imag) * (pow(rho, y.x) * pow(M_E, -y.y * theta));
}

float2 complex_sin(float2 x) {
  float2 iz = complex_mul(float2(0.0, 1.0), x);
  float2 inz = complex_mul(float2(0.0, -1.0), x);
  float2 eiz = complex_pow( float2(M_E, 0.0), iz );
  float2 einz = complex_pow( float2(M_E, 0.0), inz );
  return complex_div( eiz - einz, float2(0.0, 2.0));
}

float2 complex_cos(float2 x) {
  float2 iz = complex_mul(float2(0.0, 1.0), x);
  float2 inz = complex_mul(float2(0.0, -1.0), x);
  float2 eiz = complex_pow( float2(M_E, 0.0), iz );
  float2 einz = complex_pow( float2(M_E, 0.0), inz );
  return complex_div( eiz + einz, float2(2.0, 0.0));
}

float2 complex_log(float2 x) {
  return float2( log( length(x) ), arg(x) );
}
