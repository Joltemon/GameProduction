shader_type canvas_item;

uniform float strength;
uniform sampler2D screenTexture:hint_screen_texture, filter_linear_mipmap;

void fragment() {
	COLOR = textureLod(screenTexture, SCREEN_UV, strength);
}