using AnnoDesigner.Core.Controls;
using AnnoDesigner.Core.Extensions;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Services;
using AnnoDesigner.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnnoDesigner.ViewModels;

public class ManageKeybindingsViewModel : Notify
{
    /// <summary>
    /// These keys match values in the Localization dictionary
    /// </summary>
    private const string EDIT = "Edit";
    private const string RECORDING = "Recording";
    private const string RESET_ALL = "ResetAll";
    private const string RESET_ALL_CONFIRMATION_MESSAGE = "ResetAllConfirmationMessage";

    private readonly ICommons commons;
    private readonly IMessageBoxService _messageBoxService;
    private readonly ILocalizationHelper _localizationHelper;

    public ManageKeybindingsViewModel(HotkeyCommandManager hotkeyCommandManager,
        ICommons commons,
        IMessageBoxService messageBoxServiceToUse,
        ILocalizationHelper localizationHelperToUse)
    {
        HotkeyCommandManager = hotkeyCommandManager;
        _messageBoxService = messageBoxServiceToUse;
        _localizationHelper = localizationHelperToUse;

        EditCommand = new RelayCommand<Hotkey>(ExecuteRebind);
        ResetHotkeysCommand = new RelayCommand(async (obj) => await ExecuteResetHotkeysAsync(obj));
        this.commons = commons;
        this.commons.SelectedLanguageChanged += Instance_SelectedLanguageChanged;

        UpdateRebindButtonText();
    }

    private void Instance_SelectedLanguageChanged(object sender, EventArgs e)
    {
        UpdateRebindButtonText();
    }

    public HotkeyCommandManager HotkeyCommandManager
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public ICommand EditCommand
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public ICommand ResetHotkeysCommand
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public string EditButtonText
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public string EditButtonCurrentTextKey { get; set; } = EDIT;

    private void ExecuteRebind(Hotkey hotkey)
    {
        EditButtonCurrentTextKey = RECORDING;
        UpdateRebindButtonText();

        HotkeyRecorderWindow window = new();
        (Key key, ModifierKeys modifiers, ExtendedMouseAction action, ActionRecorder.ActionType actionType, bool userCancelled) = window.RecordNewAction();

        //Only set new hotkeys if the user didn't click cancel, and they didn't close the window without a key/action bound
        if (!userCancelled && !(key == Key.None && action == ExtendedMouseAction.None))
        {
            hotkey.UpdateHotkey(key, action, modifiers, actionType == ActionRecorder.ActionType.KeyAction ? GestureType.KeyGesture : GestureType.MouseGesture);
        }
        EditButtonCurrentTextKey = EDIT;
        UpdateRebindButtonText();
    }

    private async Task ExecuteResetHotkeysAsync(object param)
    {
        if (await _messageBoxService.ShowQuestion(_localizationHelper.GetLocalization(RESET_ALL_CONFIRMATION_MESSAGE),
            _localizationHelper.GetLocalization(RESET_ALL)))
        {
            HotkeyCommandManager.ResetHotkeys();
        }
    }

    private void UpdateRebindButtonText()
    {
        EditButtonText = _localizationHelper.GetLocalization(EditButtonCurrentTextKey);
    }
}
