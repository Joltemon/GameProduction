shader_type spatial;

uniform float height_scale = 0.5;
uniform float red = 0.5;
uniform float green = 0.5;
uniform float blue = 0.5;
uniform sampler2D noise;
uniform sampler2D normalmap;

varying vec2 tex_position;

void vertex() {
  tex_position = VERTEX.xz / 2.0 + 0.5;
  float height = texture(noise, tex_position).x;
  VERTEX.y += height * height_scale;
}

void fragment() {
  NORMAL_MAP = texture(normalmap, tex_position).xyz;
  ALBEDO = vec3(red, green, blue);
}