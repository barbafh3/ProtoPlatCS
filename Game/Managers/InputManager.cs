using LanguageExt;
using Raylib_cs;
using Tomlyn.Model;
using static LanguageExt.Prelude;

namespace ProtoPlat.Managers;

public enum InputState
{
    Pressed,
    Released,
    Up,
    Down
}

public class InputAction
{
    public readonly string Name;
    public InputState State = InputState.Up;
    public readonly List<KeyboardKey> KeyboardKeys;
    public readonly List<MouseButton> Buttons;
    public readonly bool MultiKey = false;

    public InputAction(string name, List<KeyboardKey> keyboardKeys, List<MouseButton> buttons, bool multiKey)
    {
      Name = name;
      KeyboardKeys = keyboardKeys;
      Buttons = buttons;
      MultiKey = multiKey;
    } 
}

public static class InputManager
{
    private static readonly List<InputAction> Actions = new ();

    public static void LoadInputConfig(TomlTable inputConfig)
    {
        try
        {
            var keyboardActions = (TomlTableArray)inputConfig["actions"];
            foreach (var action in keyboardActions)
            {
                var keys = (TomlArray)action["KeyboardKeys"];
                var buttons = (TomlArray)action["MouseButtons"];
                var kbKeyList = new List<KeyboardKey>();
                var mouseButtonList = new List<MouseButton>();
                foreach (var key in keys)
                    kbKeyList.Add(Enum.Parse<KeyboardKey>(key.ToString()));
                foreach (var button in buttons)
                    mouseButtonList.Add(Enum.Parse<MouseButton>(button.ToString()));
                Actions.Add(new((string)action["Name"], kbKeyList, mouseButtonList, (bool)action["MultiKey"]));
            }
        }
        catch (Exception e)
        {
            GameLogger.Log(LogLevel.ERROR, e.Message);
            Environment.Exit(1);
        }
    }

    public static List<InputAction> GetInputActions() => new(Actions);

    /// <summary>
    /// Returns the state of the given input action.
    /// </summary>
    /// <param name="actionName">Name of the input action</param>
    /// <returns>The <b>InputState</b> of the given action. Returns <b>None</b> if action is not found.</returns>
    public static Option<InputState> GetInputActionState(string actionName)
    {
        return Actions
            .Find(pred: kbAction => kbAction.Name == actionName) // Used 'pred:' to force the LanguageExt version of 'Find'
            .Match(
                Some: kbAction => Some(kbAction.State), 
                None: () => None);
    }
    
    public static bool IsInputActionDown(string actionName)
    {
        return Actions
            .Find(pred: kbAction => kbAction.Name == actionName && kbAction.State == InputState.Down) // Used 'pred:' to force the LanguageExt version of 'Find'
            .Match(Some: _ => true, None: () => false);
    }
    
    public static bool IsInputActionUp(string actionName)
    {
        return Actions
            .Find(pred: kbAction => kbAction.Name == actionName && kbAction.State == InputState.Up) // Used 'pred:' to force the LanguageExt version of 'Find'
            .Match(Some: _ => true, None: () => false);
    }
    
    public static bool IsInputActionPressed(string actionName)
    {
        return Actions
            .Find(pred: kbAction => kbAction.Name == actionName && kbAction.State == InputState.Pressed) // Used 'pred:' to force the LanguageExt version of 'Find'
            .Match(Some: _ => true, None: () => false);
    }
    
    public static bool IsInputActionReleased(string actionName)
    {
        return Actions
            .Find(pred: kbAction => kbAction.Name == actionName && kbAction.State == InputState.Released) // Used 'pred:' to force the LanguageExt version of 'Find'
            .Match(Some: _ => true, None: () => false);
    }
    
    /// <summary>
    /// Handles input filtering for all configured input actions.
    /// </summary>
    public static void HandleInput()
    {
        foreach (var action in Actions)
        {
            foreach (var key in action.KeyboardKeys)
            {
                if (Raylib.IsKeyPressed(key))
                {
                    action.State = InputState.Pressed;
                    break;
                }
                if (Raylib.IsKeyReleased(key))
                {
                    action.State = InputState.Released;
                    break;
                }
                if (Raylib.IsKeyUp(key))
                {
                    action.State = InputState.Up;
                    break;
                }
                if (Raylib.IsKeyDown(key))
                {
                    action.State = InputState.Down;
                    break;
                }
            } 
            
            foreach (var button in action.Buttons)
            {
                if (Raylib.IsMouseButtonPressed(button))
                {
                    action.State = InputState.Pressed;
                    break;
                }
                if (Raylib.IsMouseButtonReleased(button))
                {
                    action.State = InputState.Released;
                    break;
                }
                if (Raylib.IsMouseButtonDown(button))
                {
                    action.State = InputState.Down;
                    break;
                }
                if (Raylib.IsMouseButtonUp(button))
                {
                    action.State = InputState.Up;
                    break;
                }
            } 
        }
    }
}