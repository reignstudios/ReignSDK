#GLOBAL
#END

#VS
in vec3 Position0;
in vec3 Normal0;

out vec4 Position_VSPS;
out vec3 Normal_VSPS;

uniform mat4 Camera;
uniform mat4 Transform;

void main()
{
	vec4 loc = (Transform *  vec4(Position0, 1));
	gl_Position = Position_VSPS = (loc * Camera);
	Normal_VSPS = normalize((Transform *  vec4(Normal0, 0)).xyz);
}
#END

#PS
in vec4 Position_VSPS;
in vec3 Normal_VSPS;

out vec4 glFragColorOut[1];

uniform vec3 LightDirection;
uniform vec4 LightColor;
uniform vec4 Diffuse;

void main()
{
	float light = dot(-LightDirection, Normal_VSPS);
	glFragColorOut[0] = Diffuse * LightColor * light;
}
#END

