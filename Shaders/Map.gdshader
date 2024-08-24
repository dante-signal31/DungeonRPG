shader_type canvas_item;

uniform sampler2D maskTexture: source_color;
uniform vec4 fogColor: source_color = vec4(0.0, 0.0, 0.0, 1.0);

void fragment() {
	vec4 maskSample = texture(maskTexture, UV);
	if (maskSample.r == 0.0){
		COLOR = fogColor;
	}
}
