using AnnoDesigner.Core.Models;
using System.Collections.Generic;

namespace AnnoDesigner.Core.Services;

public interface IClipboardService
{
    void Copy(IEnumerable<AnnoObject> objects);

    ICollection<AnnoObject> Paste();
}
