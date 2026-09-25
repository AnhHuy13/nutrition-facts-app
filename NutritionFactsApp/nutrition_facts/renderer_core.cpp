#include <stdint.h>

extern "C" {
    __declspec(dllexport) void fill_rect(uint8_t* pixels, int img_width, int img_height, int x, int y, int w, int h, uint8_t r, uint8_t g, uint8_t b) {
        if (x < 0) { w += x; x = 0; }
        if (y < 0) { h += y; y = 0; }
        if (x + w > img_width) w = img_width - x;
        if (y + h > img_height) h = img_height - y;
        if (w <= 0 || h <= 0) return;

        for (int row = y; row < y + h; ++row) {
            uint8_t* ptr = pixels + (row * img_width + x) * 3;
            for (int col = 0; col < w; ++col) {
                ptr[0] = r;
                ptr[1] = g;
                ptr[2] = b;
                ptr += 3;
            }
        }
    }
}
