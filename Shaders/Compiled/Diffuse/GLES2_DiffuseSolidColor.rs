#GLOBAL
varying vec4 Position_VSPS;
varying vec3 Normal_VSPS;
#END

#VS
attribute vec3 Position0;
attribute vec3 Normal0;

uniform mat4 Camera;
uniform mat4 Transform;

void main()
{
	vec4 loc = ( vec4(Position0, 1) * Transform);
	gl_Position = Position_VSPS = (loc * Camera);
	Normal_VSPS = ( vec4(Normal0, 0) * Transform).xyz;
}
#END

#PS
uniform vec3 LightDirection;
uniform vec4 LightColor;
uniform vec4 Diffuse;

void main()
{
	vec3 normal = normalize(Normal_VSPS);
	float light = dot(-LightDirection, normal);
	gl_FragData[0] = Diffuse * LightColor * light;
}
#END

