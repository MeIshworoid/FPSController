# 🎮 FPS Controller

A modern, customizable First-Person Shooter controller built in Unity, featuring advanced movement mechanics and realistic physics.

## ✨ Features

- **Movement Mechanics**
  - Smooth ground movement with acceleration and deceleration
  - Jump mechanics with variable height based on input
  - Dash ability with visual and audio feedback
  - Ground detection and slope handling
  - Momentum-based movement system

- **Camera System**
  - Dynamic camera tilt based on horizontal movement
  - Smooth camera bob effect during movement
  - Post-processing effects (Chromatic Aberration during dash)
  - Dynamic depth of field based on focus distance
  - Landing drop camera effect with customizable curves

- **Audio Feedback**
  - Footstep sounds with random clip selection
  - Jump and landing sound effects with volume scaling
  - Dash sound effect with post-processing
  - Speed-based woosh sounds during air movement
  - Custom audio extension methods for random clip playback

- **Visual Effects**
  - Particle system for speed lines during fast movement
  - Post-processing volume integration
  - Dynamic depth of field adjustment
  - Camera shake and tilt effects
  - Visual feedback for movement states

## 🏗️ Project Structure

```
Assets/
├── Scripts/           # Core gameplay scripts
├── Scenes/           # Game scenes
├── Prefabs/          # Reusable game objects
├── Materials/        # Visual materials
├── Models/           # 3D models
├── AudioClip/        # Sound effects and music
├── AnimationClip/    # Character animations
├── AnimatorController/# Animation controllers
├── Shader/           # Custom shaders
└── Texture2D/        # Textures and sprites
```

## 🚀 Getting Started

1. Clone the repository
2. Open the project in Unity (2022.3 or later recommended)
3. Open the main scene in `Assets/Scenes`
4. Press Play to test the controller

## 🛠️ Customization

The controller is highly customizable through the Unity Inspector:
- Adjust movement parameters (speed, acceleration, etc.)
- Configure camera effects (tilt, post-processing)
- Customize audio settings (footsteps, jumps, etc.)
- Modify visual effects (speed lines, particles)

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Contact

For questions or suggestions, please open an issue in the repository. 

