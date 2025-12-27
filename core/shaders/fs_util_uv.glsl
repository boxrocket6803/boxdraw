#version 450

in vec2 uv;
out vec4 out_color;

void main() {
	out_color = vec4(uv, 0, 1);
}