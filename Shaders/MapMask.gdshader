shader_type canvas_item;

uniform sampler2D shapesTexture: source_color;
uniform sampler2D prev_frame_tex: hint_screen_texture;

void fragment() {
	vec4 shapeColor = texture(shapesTexture, UV);
	vec4 previousColor = texture(prev_frame_tex, SCREEN_UV);
	if (shapeColor.r > 0.0){
		COLOR = vec4(1.0, 1.0, 1.0, 1.0);
	} else {
		COLOR = previousColor;
	}
}
