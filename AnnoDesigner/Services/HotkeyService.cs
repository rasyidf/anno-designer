using AnnoDesigner.Models;
using AnnoDesigner.ViewModels;
using System.Collections.Generic;
using System.Windows.Input;

namespace AnnoDesigner.Services;

/// <summary>
/// Thin wrapper around the existing <see cref="HotkeyCommandManager"/> to provide
/// an <see cref="IHotkeyService"/> implementation. This enables gradual migration
/// of callsites to depend on an abstraction instead of the concrete manager.
/// migrated from migration plan — HotkeyService abstraction (Phase 1/2)
/// </summary>
public class HotkeyService : IHotkeyService
{
    private readonly HotkeyCommandManager _manager;

    public HotkeyService(HotkeyCommandManager manager)
    {
        _manager = manager;
    }

    public void HandleCommand(InputEventArgs e)
    {
        _manager.HandleCommand(e);
    }

    public void AddHotkey(string hotkeyId, InputBinding binding)
    {
        _manager.AddHotkey(hotkeyId, binding);
    }

    public void AddHotkey(Hotkey hotkey)
    {
        _manager.AddHotkey(hotkey);
    }

    public void RemoveHotkey(string hotkeyId)
    {
        _manager.RemoveHotkey(hotkeyId);
    }

    public IEnumerable<Hotkey> GetHotkeys()
    {
        return _manager.GetHotkeys();
    }

    public bool ContainsHotkey(string hotkeyId)
    {
        return _manager.ContainsHotkey(hotkeyId);
    }

    public Hotkey GetHotkey(string hotkeyId)
    {
        return _manager.GetHotkey(hotkeyId);
    }

    public void LoadHotkeyMappings(IDictionary<string, HotkeyInformation> mappings)
    {
        _manager.LoadHotkeyMappings(mappings);
    }

    public Dictionary<string, HotkeyInformation> GetRemappedHotkeys()
    {
        return _manager.GetRemappedHotkeys();
    }

    public void UpdateLanguage()
    {
        _manager.UpdateLanguage();
    }

    public void ResetHotkeys()
    {
        _manager.ResetHotkeys();
    }
}
