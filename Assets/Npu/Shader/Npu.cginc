#define GREYSCALE(rgb, factor) lerp(rgb, dot(rgb, half3(0.21, 0.71, 0.07)), factor)