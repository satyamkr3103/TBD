# Text to Build

Text to Build is a Unity platformer where the player can describe changes in natural language, use voice input, and generate or modify gameplay elements through an AI-driven workflow. The project combines a side-scrolling level, player movement, ladder climbing, energy management, prompt validation, and Groq-powered responses that shape the world.

## Features

- Voice input using Unity dictation on Windows
- Text prompt submission through the UI
- Groq API integration for AI-generated build/modification requests
- Energy-based action cost system
- Prompt guardrails and required-word validation
- Player movement, jumping, and ladder climbing
- Level progression through a goal trigger
- Main menu with play and quit actions

## Requirements

- Unity 2022.3.62f3
- Windows for `UnityEngine.Windows.Speech` voice dictation support
- A Groq API key for AI requests

## Getting Started

1. Open the project in Unity Hub using Unity 2022.3.62f3.
2. Open the main scene, typically `Assets/Scenes/Level1.unity`.
3. In the Inspector, assign the scene references used by the UI and gameplay managers.
4. Set your Groq API key on the `GroqAILogicController` component.
5. Enter Play Mode and test the UI prompt or voice input.

## How It Works

The player can move with the keyboard, jump, and climb ladders. The UI accepts a prompt, validates it, checks required words such as `bridge`, `ladder`, `box`, or `platform`, and then sends the request to Groq. The AI response is parsed into build or modification data, which the game uses to update the level.

Voice input is handled separately by `VoiceInputManager`, which listens for speech and forwards recognized text to the UI.

## Controls

- `A` / `D` or left / right arrows: Move
- `Space`: Jump or jump off a ladder
- Voice input: Speak a prompt after starting listening
- UI prompt field: Type a request and submit it

## Project Structure

- `Assets/Scripts/VoiceInputManager.cs` - Speech recognition and voice forwarding
- `Assets/Scripts/UIManager.cs` - Prompt handling, selection state, and AI request flow
- `Assets/Scripts/GroqAILogicController.cs` - Groq API communication
- `Assets/Scripts/PlayerMovement.cs` - Movement, jumping, and ladder climbing
- `Assets/Scripts/PromptGuardrails.cs` - Prompt validation and cost checks
- `Assets/Scripts/RequiredWordSystem.cs` - Required-word checks for prompts
- `Assets/Scripts/MainMenuManager.cs` - Main menu actions

## Notes

- Do not commit real API keys to source control.
- Voice dictation support is tied to Windows.
- If AI responses fail, check the Groq API key, network access, and the JSON returned by the model.

