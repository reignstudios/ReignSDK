#GLOBAL
varying vec4 Position_VSPS;
varying vec2 UV_VSPS;
#END

#VS
attribute vec2 Position0;

uniform mat4 Camera;
uniform vec3 Location;
uniform vec2 Size;
uniform vec2 LocationUV;
uniform vec2 SizeUV;
uniform vec2 TexelOffset;

void main()
{
	vec3 loc = vec3((Position0 * Size) + Location.xy, Location.z);
	gl_Position = Position_VSPS = ( vec4(loc, 1.0) * Camera);

	vec2 uv = Position0 + TexelOffset;
	UV_VSPS = ( vec2(uv.x, 1.0-uv.y) * SizeUV) + LocationUV;
}
#END

#PS
uniform vec4 Color;
uniform sampler2D DiffuseTexture;

void main()
{
	gl_FragData[0] = texture2D(DiffuseTexture, UV_VSPS) * Color;
}
#END

