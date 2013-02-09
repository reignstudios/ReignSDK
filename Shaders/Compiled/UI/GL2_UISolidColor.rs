#GLOBAL
varying vec4 Position_VSPS;
#END

#VS
attribute vec2 Position0;

uniform mat4 Camera;
uniform vec2 Position;
uniform vec2 Size;

void main()
{
	vec3 loc = vec3((Position0 * Size) + Position, 0);
	gl_Position = Position_VSPS = ( vec4(loc, 1.0) * Camera);
}
#END

#PS
uniform vec4 Color;

void main()
{
	gl_FragData[0] = Color;
}
#END

