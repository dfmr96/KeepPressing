# Code Input System Documentation

## Overview
The Code Input System is the core mechanic for Keep Pressing. Players must enter the LOST bunker code (4 8 15 16 23 42) using a VR-interactive numeric keypad before the timer expires.

## Architecture

### Core Components

#### 1. **CountdownTimer.cs**
- Configurable countdown timer (120s or 60s)
- Fires events: OnTimerStart, OnTimerTick, OnTimerWarning, OnTimerExpired
- Supports pause/resume/reset
- Formatted time display (MM:SS)

**Key Methods:**
- `StartTimer()` - Begins countdown
- `SetDuration(float)` - Changes timer length (for 120s → 60s transition)
- `GetFormattedTime()` - Returns time as "MM:SS" string

#### 2. **LEDDisplay.cs**
- Digital LED-style display using TextMeshPro
- Shows up to 6 two-digit numbers
- Visual feedback: normal (green), error (red), success (cyan)
- Placeholder underscores for empty slots

**Key Methods:**
- `AddDigit(int)` - Adds a number to the display
- `Clear()` - Clears all digits
- `ShowError()` - Flashes red
- `ShowSuccess()` - Changes to cyan

#### 3. **NumericButton.cs**
- Individual button component with XR interactions
- Uses XRSimpleInteractable for VR poke/press
- Visual, audio, and haptic feedback
- Animates press/release

**Requirements:**
- Requires `XRSimpleInteractable` component
- Optional: MeshRenderer, AudioSource, Transform for visual

**Key Features:**
- Press depth animation
- Material swap on press/release
- Haptic feedback to controller
- Button value storage (0-99)

#### 4. **CodeInputPanel.cs**
- Main controller orchestrating all components
- Validates code against LOST sequence: [4, 8, 15, 16, 23, 42]
- Manages input flow and button states
- Auto-submit on completion (configurable)

**Key Events:**
- `OnCodeCorrect` - Correct code entered
- `OnCodeIncorrect` - Wrong code entered
- `OnCodeCleared` - Display cleared
- `OnDigitEntered` - Any digit pressed

**Key Methods:**
- `SubmitCode()` - Validates current input
- `ClearCode()` - Clears current input
- `LockPanel()` / `UnlockPanel()` - Enable/disable input

### Managers

#### 5. **ProtocolManager.cs**
- Singleton game state manager
- Tracks cycle count, successes, failures
- Switches timer from 120s → 60s after X successful cycles
- Suspends protocol and unlocks hatch on failure in final act

**Key Events:**
- `OnProtocolStarted` - Game begins
- `OnCycleCompleted` - Successful code entry
- `OnCycleFailed` - Timer expired
- `OnFinalActTriggered` - Switched to 60s timer
- `OnProtocolSuspended` - System failure
- `OnHatchUnlocked` - Meta-reveal unlocked

**Configuration:**
- `initialTimerDuration` - Starting interval (default: 120s)
- `finalActDuration` - Final act interval (default: 60s)
- `cyclesBeforeFinalAct` - Successes before acceleration (default: 3)

## Integration Guide

### Basic Setup
1. Create a GameObject with `CodeInputPanel` component
2. Attach `LEDDisplay` component to a child object with TextMeshPro
3. Attach `CountdownTimer` component
4. Create 10 buttons (0-9) with `NumericButton` components
5. Create Clear and Submit buttons
6. Assign all references in `CodeInputPanel` inspector
7. Add `ProtocolManager` to scene
8. Wire up `CodeInputPanel` and `CountdownTimer` references

### Prefab Structure (Recommended)
```
CodeInputPanel GameObject
├── ButtonGrid
│   ├── Button_4 (NumericButton, value: 4)
│   ├── Button_8 (NumericButton, value: 8)
│   ├── Button_15 (NumericButton, value: 15)
│   ├── Button_16 (NumericButton, value: 16)
│   ├── Button_23 (NumericButton, value: 23)
│   └── Button_42 (NumericButton, value: 42)
├── Controls
│   ├── ClearButton (NumericButton)
│   └── SubmitButton (NumericButton)
├── LEDDisplay (LEDDisplay + TextMeshProUGUI)
├── TimerDisplay (TextMeshProUGUI)
└── AudioSource
```

### XR Interaction Requirements
- Each button needs:
  - `XRSimpleInteractable` component
  - Collider for interaction
  - Visual mesh (for press animation)
- Player needs XR Origin with poke interactor

## Event Flow

1. **Game Start**
   - ProtocolManager.StartProtocol()
   - Timer starts at 120s

2. **Player Enters Code**
   - Player pokes buttons
   - NumericButton fires OnButtonPressed
   - CodeInputPanel adds digit to display
   - When 6 digits entered → auto-submit

3. **Code Validation**
   - **Correct:**
     - Success feedback
     - Cycle count increases
     - Timer resets
     - Check if final act should trigger
   - **Incorrect:**
     - Error feedback
     - Clear display
     - Timer continues

4. **Timer Expiration**
   - If code not entered in time
   - Cycle fails
   - If in final act: Protocol suspends → Hatch unlocks
   - Otherwise: Timer resets, continue

5. **Final Act Transition**
   - After 3 successful cycles
   - Timer switches to 60s
   - Increased pressure

6. **Protocol Failure**
   - First timer failure in final act
   - System suspends
   - Hatch unlocks (meta-reveal)

## Testing Checklist

- [ ] Timer counts down correctly
- [ ] Buttons respond to VR poke
- [ ] LED display shows entered numbers
- [ ] Correct code (4 8 15 16 23 42) triggers success
- [ ] Wrong code clears and allows retry
- [ ] Timer expiration handled correctly
- [ ] Final act triggers after X successes
- [ ] Timer switches to 60s in final act
- [ ] Protocol suspends on failure in final act
- [ ] Hatch unlock event fires
- [ ] Audio feedback works
- [ ] Haptic feedback works

## Customization

### Change the Code
```csharp
codeInputPanel.SetCorrectCode(new List<int> { 1, 2, 3, 4, 5, 6 });
```

### Change Timer Durations
```csharp
protocolManager.initialTimerDuration = 180f; // 3 minutes
protocolManager.finalActDuration = 30f;      // 30 seconds
```

### Disable Final Act
```csharp
protocolManager.enableFinalActSwitch = false;
```

## Future Enhancements
- Visual glitches as system malfunctions
- Sound design for LOST-style alarm
- Emergency lights/sirens on final act
- Procedural code generation (optional mode)
- Multiple difficulty levels
