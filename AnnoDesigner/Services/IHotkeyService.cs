using System.Collections.Generic;
using System.Windows.Input;
using AnnoDesigner.Models;

namespace AnnoDesigner.Services;

/// <summary>
/// Abstraction for hotkey management so callers can be decoupled from the concrete
/// HotkeyCommandManager implementation. Implemented as a thin wrapper around
/// the existing HotkeyCommandManager for incremental migration.
/// </summary>
public interface IHotkeyService
{
    void HandleCommand(InputEventArgs e);
    void AddHotkey(string hotkeyId, InputBinding binding);
    void AddHotkey(Hotkey hotkey);
    void RemoveHotkey(string hotkeyId);
    IEnumerable<Hotkey> GetHotkeys();
    bool ContainsHotkey(string hotkeyId);
    Hotkey GetHotkey(string hotkeyId);
    void LoadHotkeyMappings(IDictionary<string, HotkeyInformation> mappings);
    Dictionary<string, HotkeyInformation> GetRemappedHotkeys();
    void UpdateLanguage();
    void ResetHotkeys();
}
