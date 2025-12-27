#version 450

in vec3 pos;
in vec2 in_uv;
out vec2 uv;

void main() {
	gl_Position = vec4(pos, 1.0);
	uv = in_uv;
}