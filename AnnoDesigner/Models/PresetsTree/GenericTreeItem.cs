using AnnoDesigner.Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AnnoDesigner.Models.PresetsTree;

[DebuggerDisplay("{" + nameof(Header) + ",nq}")]
public class GenericTreeItem : Notify
{
    public GenericTreeItem(GenericTreeItem parent)
    {
        Parent = parent;
        Header = string.Empty;
        Children = [];
        IsExpanded = false;
        IsVisible = true;
        IsSelected = false;
    }

    public GenericTreeItem Parent
    {
        get;
        private set => UpdateProperty(ref field, value);
    }

    public GenericTreeItem Root => Parent == null ? this : Parent.Root;

    public string Header
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public AnnoObject AnnoObject
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public ObservableCollection<GenericTreeItem> Children
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public bool IsExpanded
    {
        get;
        set
        {
            _ = UpdateProperty(ref field, value);

            //also expand all parent nodes
            if (value && Parent != null)
            {
                Parent.IsExpanded = value;
            }
        }
    }

    public bool IsVisible
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public bool IsSelected
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    /// <summary>
    /// Id of this node is mainly used to save/restore a tree state.
    /// </summary>        
    public int Id
    {
        get;
        set => UpdateProperty(ref field, value);
    }
}
