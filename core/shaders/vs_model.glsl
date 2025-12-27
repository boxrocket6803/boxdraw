#version 450

in vec3 pos;
in vec2 in_uv;
uniform mat4 proj;
uniform mat4 view;
out vec2 uv;

void main() {
	gl_Position = proj * view * vec4(pos, 1.0);
	uv = in_uv;
}