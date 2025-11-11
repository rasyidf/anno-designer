using AnnoDesigner.Core.Controls;
using AnnoDesigner.Core.Models;
using System;
using System.Windows.Input;

namespace AnnoDesigner.Core.CustomEventArgs;

public class ActionRecorderEventArgs : EventArgs
{
    public static new readonly ActionRecorderEventArgs Empty = new(Key.None, ExtendedMouseAction.None, ModifierKeys.None, ActionRecorder.ActionType.None);

    public Key Key { get; }
    public ExtendedMouseAction Action { get; }
    public ModifierKeys Modifiers { get; }
    public ActionRecorder.ActionType ResultType { get; }

    private ActionRecorderEventArgs() { }
    public ActionRecorderEventArgs(Key key, ExtendedMouseAction action, ModifierKeys modifiers, ActionRecorder.ActionType resultType)
    {
        Key = key;
        Action = action;
        Modifiers = modifiers;
        ResultType = resultType;
    }
}
