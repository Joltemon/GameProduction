shader_type spatial;

uniform vec3 Color : source_color;
uniform float Transparency : hint_range(0.0, 1.0, 0.1);

void fragment() {
	ALBEDO = Color;
	ALPHA = 1.0;
}
