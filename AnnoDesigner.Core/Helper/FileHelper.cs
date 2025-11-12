using System;
using System.IO;
using System.IO.Abstractions;

namespace AnnoDesigner.Core.Helper;

public static class FileHelper
{
    private static IFileSystem _fileSystem = new FileSystem();

    /// <summary>
    /// Sets the attributes of a file to 'normal'. If the file was set to ReadOnly the attribute will be removed.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <exception cref="Exception">The attributes of the file could not be set to 'normal'.</exception>        
    public static void ResetFileAttributes(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath), "The path to the file was not specified.");
        }

        if (!_fileSystem.File.Exists(filePath))
        {
            return;
        }
#if DEBUG
        FileAttributes fileAttributes = _fileSystem.File.GetAttributes(filePath);

        //check whether a file is read only
        bool isReadOnly = fileAttributes.HasFlag(FileAttributes.ReadOnly);

        //check whether a file is hidden
        bool isHidden = fileAttributes.HasFlag(FileAttributes.Hidden);

        //check whether a file has archive attribute
        bool isArchive = fileAttributes.HasFlag(FileAttributes.Archive);

        //check whether a file is system file
        bool isSystem = fileAttributes.HasFlag(FileAttributes.System);
#endif            
        try
        {
            _fileSystem.File.SetAttributes(filePath, FileAttributes.Normal);
        }
        catch (Exception ex)
        {
            string errorMessage = $"The attributes of the file \"{filePath}\" could not be set to 'normal'.";
            throw new IOException(errorMessage, ex);
        }
    }

    /// <summary>
    /// Allows setting a custom IFileSystem implementation for testing purposes.
    /// </summary>
    /// <param name="fileSystem">The IFileSystem implementation to use.</param>
    public static void SetFileSystem(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
}
