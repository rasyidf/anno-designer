using AnnoDesigner.Core.Layout.Exceptions;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Services;
using AnnoDesigner.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoDesigner.Services
{
    public interface ILayoutFileService
    {
        /// <summary>
        /// Loads a layout from a file path.
        /// </summary>
        Task<LayoutFile> LoadLayoutFromFileAsync(string filePath, bool forceLoad = false);

        /// <summary>
        /// Loads a layout from a JSON string.
        /// </summary>
        Task<LayoutFile> LoadLayoutFromJsonAsync(string jsonString, bool forceLoad = false);

        /// <summary>
        /// Saves a layout to a file.
        /// </summary>
        void SaveLayoutToFile(LayoutFile layout, string filePath);

        /// <summary>
        /// Serializes a layout to a JSON string.
        /// </summary>
        string SerializeLayoutToJson(LayoutFile layout);

        /// <summary>
        /// Creates LayoutObjects from a LayoutFile.
        /// </summary>
        List<LayoutObject> CreateLayoutObjects(LayoutFile layout, ICoordinateHelper coordinateHelper, IBrushCache brushCache, IPenCache penCache);

        /// <summary>
        /// Creates a LayoutFile from LayoutObjects.
        /// </summary>
        LayoutFile CreateLayoutFile(IEnumerable<LayoutObject> layoutObjects, Version layoutVersion);
    }

    public class LayoutFileService : ILayoutFileService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ILayoutLoader _layoutLoader;
        private readonly IFileSystem _fileSystem;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ILocalizationHelper _localizationHelper;

        public LayoutFileService(
            ILayoutLoader layoutLoader,
            IFileSystem fileSystem,
            IMessageBoxService messageBoxService,
            ILocalizationHelper localizationHelper)
        {
            _layoutLoader = layoutLoader ?? throw new ArgumentNullException(nameof(layoutLoader));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
            _localizationHelper = localizationHelper ?? throw new ArgumentNullException(nameof(localizationHelper));
        }

        public async Task<LayoutFile> LoadLayoutFromFileAsync(string filePath, bool forceLoad = false)
        {
            try
            {
                return await Task.Run(() => _layoutLoader.LoadLayout(filePath, forceLoad));
            }
            catch (LayoutFileUnsupportedFormatException layoutEx)
            {
                logger.Warn(layoutEx, "Version of layout file is not supported.");

                return await _messageBoxService.ShowQuestion(
                    null,
                    _localizationHelper.GetLocalization("FileVersionUnsupportedMessage"),
                    _localizationHelper.GetLocalization("FileVersionUnsupportedTitle"))
                    ? await LoadLayoutFromFileAsync(filePath, true)
                    : null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading layout from file.");
                _messageBoxService.ShowError(
                    null,
                    ex.Message,
                    _localizationHelper.GetLocalization("LayoutLoadingError"));
                return null;
            }
        }

        public async Task<LayoutFile> LoadLayoutFromJsonAsync(string jsonString, bool forceLoad = false)
        {
            try
            {
                return string.IsNullOrWhiteSpace(jsonString)
                    ? null
                    : await Task.Run(() =>
                {
                    byte[] jsonArray = Encoding.UTF8.GetBytes(jsonString);
                    using MemoryStream ms = new(jsonArray);
                    return _layoutLoader.LoadLayout(ms, forceLoad);
                });
            }
            catch (LayoutFileUnsupportedFormatException layoutEx)
            {
                logger.Warn(layoutEx, "Version of layout does not match.");

                return await _messageBoxService.ShowQuestion(
                    null,
                    _localizationHelper.GetLocalization("FileVersionMismatchMessage"),
                    _localizationHelper.GetLocalization("FileVersionMismatchTitle"))
                    ? await LoadLayoutFromJsonAsync(jsonString, true)
                    : null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading layout from JSON.");
                _messageBoxService.ShowError(
                    null,
                    _localizationHelper.GetLocalization("LayoutLoadingError"),
                    _localizationHelper.GetLocalization("Error"));
                return null;
            }
        }

        public void SaveLayoutToFile(LayoutFile layout, string filePath)
        {
            try
            {
                _layoutLoader.SaveLayout(layout, filePath);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error saving layout to file.");
                _messageBoxService.ShowError(
                    null,
                    ex.Message,
                    _localizationHelper.GetLocalization("IOErrorMessage"));
                throw;
            }
        }

        public string SerializeLayoutToJson(LayoutFile layout)
        {
            try
            {
                using MemoryStream ms = new();
                _layoutLoader.SaveLayout(layout, ms);
                ms.Position = 0;
                using StreamReader reader = new(ms);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error serializing layout to JSON.");
                _messageBoxService.ShowError(
                    null,
                    _localizationHelper.GetLocalization("LayoutLoadingError"),
                    _localizationHelper.GetLocalization("Error"));
                return null;
            }
        }

        public List<LayoutObject> CreateLayoutObjects(
            LayoutFile layout,
            ICoordinateHelper coordinateHelper,
            IBrushCache brushCache,
            IPenCache penCache)
        {
            if (layout == null || layout.Objects == null)
            {
                return [];
            }

            List<LayoutObject> layoutObjects = new(layout.Objects.Count);
            foreach (AnnoObject curObj in layout.Objects)
            {
                layoutObjects.Add(new LayoutObject(curObj, coordinateHelper, brushCache, penCache));
            }

            return layoutObjects;
        }

        public LayoutFile CreateLayoutFile(IEnumerable<LayoutObject> layoutObjects, Version layoutVersion)
        {
            return new LayoutFile(layoutObjects.Select(x => x.WrappedAnnoObject))
            {
                LayoutVersion = layoutVersion
            };
        }
    }
}
