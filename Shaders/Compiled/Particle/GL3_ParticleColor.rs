#GLOBAL
#END

#VS
in vec3 Position0;
in vec2 Texcoord0;

out vec4 Position_VSPS;
out vec2 UV_VSPS;

uniform mat4 Camera;
uniform mat4 BillboardTransform;
uniform vec4 ColorPallet[4];
uniform vec4 ScalePallet;
uniform vec4 Transforms[10];

void main()
{
	vec4 loc = ( vec4(Position0, 1) * BillboardTransform);
	gl_Position = Position_VSPS = (loc * Camera);
	UV_VSPS = vec2(Texcoord0.x, 1.0-Texcoord0.y);
}
#END

#PS
in vec4 Position_VSPS;
in vec2 UV_VSPS;

out vec4 glFragColorOut[1];

uniform sampler2D Diffuse;

void main()
{
	glFragColorOut[0] = texture2D(Diffuse, UV_VSPS);
}
#END

