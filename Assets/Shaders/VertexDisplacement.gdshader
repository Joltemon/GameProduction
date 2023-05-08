shader_type spatial;

uniform float height_scale = 0.5;
uniform sampler2D displacement;

varying vec2 tex_position;

void vertex() {
  tex_position = VERTEX.xz / 2.0 + 0.5;
  float height = texture(displacement, tex_position).x;
  VERTEX.y += height * height_scale;
}