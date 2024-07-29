# panoramic-skybox-creator
Capture a 360° panoramic texture to create a Skybox material

## Requirements
- Unity 2023.2.20f1
- Universal Render Pipeline (URP)

## Description
This project is designed to create a skybox material using a 360° panoramic texture of your scene.

- For demonstration purposes, it utilizes a URP sample scene named "Oasis" (Note: This asset is not included in this repository).
- With this code, you can import a high-quality scene, capture it, and use it as a skybox material.

## Procedure
1. Copy the script to "/Assets/Skybox"
2. Create a GameObject and add the "PanoramicCapture.cs" component
3. Add a Main Camera to the scene
4. Play the scene and press the Spacebar to save a panoramic image (Panorama.png)
5. Set the texture of Skybox.mat
6. Use it as a Skybox:
   - Go to Window > Rendering > Lighting
   - In the Environment section, set the Skybox material
