shader_type canvas_item;

uniform sampler2D ScreenTexture : hint_screen_texture, filter_linear_mipmap;
uniform float strength;

void fragment() {
	COLOR = textureLod(ScreenTexture, SCREEN_UV, strength);
	COLOR.a = 1.0;
}
