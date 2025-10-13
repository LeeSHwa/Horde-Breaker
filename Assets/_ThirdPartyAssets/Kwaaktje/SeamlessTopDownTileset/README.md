# 🎨 Tile Map Overlay Shaders

A set of shaders that blend overlay textures onto a tilemap based on the RGB values of the base texture. Useful for effects like heatmaps, zone highlights, or tile-state visualization.

---

## Shader: `Custom/TileMapOverlayShader`

### Overview
Overlays a single texture on top of a tilemap using the red channel of the base texture (`_MainTex`) as a mask.

### Properties

| Property      | Type   | Description                              |
|---------------|--------|------------------------------------------|
| `_OverlayTex` | 2D     | Overlay texture to blend with the tilemap |
| `_Scale`      | Float  | Controls tiling of the overlay texture in world space |

### How It Works
- Uses the red channel (`r`) of `_MainTex` as a mask.
- If the red channel value is `>= 1`, `_OverlayTex` is blended over the base color.
- Blending is done using `lerp(color, overlay, floor(color.r))`.
- The overlay's UVs are based on world position multiplied by `_Scale`.

---

## Shader: `Custom/TileMapOverlayShader2Textures`

### Overview
Overlays **two** textures based on the **red** and **blue** channels of the base texture. Useful when multiple states or overlays need to be applied.

### Properties

| Property       | Type   | Description                              |
|----------------|--------|------------------------------------------|
| `_OverlayTexR` | 2D     | Overlay for red channel mask              |
| `_OverlayTexB` | 2D     | Overlay for blue channel mask             |
| `_ScaleR`      | Float  | Tiling for red overlay texture            |
| `_ScaleB`      | Float  | Tiling for blue overlay texture           |

### How It Works
- If `color.r >= 1`, applies `_OverlayTexR` using `worldPos * _ScaleR`.
- If `color.b >= 1`, applies `_OverlayTexB` using `worldPos * _ScaleB`.
- Blends both overlays additively onto the base texture using nested `lerp()` calls.

---

## Shader: `Custom/TileMapOverlayShader3Textures`

### Overview
Same idea as the 2-texture version, but now supports **three overlays** (red, green, blue channels). Perfect for combining multiple layers of tile states.

### Properties

| Property       | Type   | Description                              |
|----------------|--------|------------------------------------------|
| `_OverlayTexR` | 2D     | Overlay for red channel mask              |
| `_OverlayTexG` | 2D     | Overlay for green channel mask            |
| `_OverlayTexB` | 2D     | Overlay for blue channel mask             |
| `_ScaleR`      | Float  | Tiling for red overlay texture            |
| `_ScaleG`      | Float  | Tiling for green overlay texture          |
| `_ScaleB`      | Float  | Tiling for blue overlay texture           |

### How It Works
- Reads `color.r`, `color.g`, and `color.b` from `_MainTex`.
- For each channel with value `>= 1`, it blends in the respective overlay texture using world-space UVs.
- Blending order: red → blue → green using nested `lerp()` calls.

---

## 💬 Feedback & Contact

If you enjoy using this tileset, **leaving a good review would mean a lot** — seriously, it’ll make my day!

If you have questions, issues, or ideas for improvements, feel free to reach out:
📬 **kwaaktje@gmail.com**

Thanks for checking it out!
