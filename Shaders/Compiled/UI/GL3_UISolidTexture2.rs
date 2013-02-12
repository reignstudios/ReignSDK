#GLOBAL
#END

#VS
in vec2 Position0;

out vec4 Position_VSPS;
out vec2 UV_VSPS;

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
in vec4 Position_VSPS;
in vec2 UV_VSPS;

out vec4 glFragColorOut[1];

uniform vec4 Color;
uniform float Fade;
uniform sampler2D MainTexture;
uniform sampler2D MainTexture2;

void main()
{
	vec4 outColor = texture2D(MainTexture, UV_VSPS);
	outColor += (texture2D(MainTexture2, UV_VSPS) - outColor) * Fade;

	glFragColorOut[0] = outColor * Color;
}
#END

