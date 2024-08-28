shader_type canvas_item;

uniform sampler2D maskTexture: source_color;
uniform sampler2D shapesTexture: source_color;
uniform sampler2D staticMapTexture: source_color;

uniform vec4 fogColor: source_color = vec4(0.0, 0.0, 0.0, 1.0);

void fragment() {
	vec4 maskSample = texture(maskTexture, UV);
	vec4 shapesSample = texture(shapesTexture, UV);
	vec4 staticMapSample = texture(staticMapTexture, UV);
	
	if (maskSample.r == 0.0){
		COLOR = fogColor;
	} else if (shapesSample.r == 0.0){
		COLOR = staticMapSample;
	}
}
