#GLOBAL
#END

#VS
in vec2 Position0;

out vec4 Position_VSPS;

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
in vec4 Position_VSPS;

out vec4 glFragColorOut[1];

uniform vec4 Color;

void main()
{
	glFragColorOut[0] = Color;
}
#END

