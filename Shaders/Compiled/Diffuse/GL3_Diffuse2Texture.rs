#GLOBAL
#END

#VS
in vec3 Position0;
in vec3 Normal0;
in vec2 Texcoord0;

out vec4 Position_VSPS;
out vec3 Normal_VSPS;
out vec2 UV_VSPS;

uniform mat4 Camera;
uniform mat4 Transform;

void main()
{
	vec4 loc = ( vec4(Position0, 1) * Transform);
	gl_Position = Position_VSPS = (loc * Camera);
	Normal_VSPS = ( vec4(Normal0, 0) * Transform).xyz;
	UV_VSPS = vec2(Texcoord0.x, 1.0-Texcoord0.y);
}
#END

#PS
in vec4 Position_VSPS;
in vec3 Normal_VSPS;
in vec2 UV_VSPS;

out vec4 glFragColorOut[1];

uniform vec3 LightDirection;
uniform vec3 LightDirection2;
uniform vec4 LightColor;
uniform vec4 LightColor2;
uniform sampler2D Diffuse;

void main()
{
	vec3 normal = normalize(Normal_VSPS);
	float light = max(dot(-LightDirection, normal), 0.0);
	float light2 = max(dot(-LightDirection2, normal), 0.0);
	glFragColorOut[0] = texture2D(Diffuse, UV_VSPS) * ((light * LightColor) + (light2 * LightColor2));
}
#END

