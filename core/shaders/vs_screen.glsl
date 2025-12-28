#version 450

in vec3 in_pos;
in vec3 in_normal;
in vec2 in_uv;
out vec2 uv;

void main() {
	gl_Position = vec4(in_pos, 1.0);
	uv = in_uv;
}