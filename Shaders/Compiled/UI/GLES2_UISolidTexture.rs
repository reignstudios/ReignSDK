#GLOBAL
varying vec4 Position_VSPS;
varying vec2 UV_VSPS;
#END

#VS
attribute vec2 Position0;

uniform mat4 Camera;
uniform vec2 Position;
uniform vec2 Size;
uniform vec2 TexelOffset;

void main()
{
	vec3 loc = vec3((Position0 * Size) + Position, 0);
	gl_Position = Position_VSPS = ( vec4(loc, 1.0) * Camera);
	UV_VSPS = vec2(Position0.x, 1.0-Position0.y) + TexelOffset;
}
#END

#PS
uniform vec4 Color;
uniform sampler2D MainTexture;

void main()
{
	gl_FragData[0] = texture2D(MainTexture, UV_VSPS) * Color;
}
#END

