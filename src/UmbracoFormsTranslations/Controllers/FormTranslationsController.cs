using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using UmbracoFormsTranslations.Extensions;
using UmbracoFormsTranslations.Models;
using System;
using System.IO;
using System.Text;
using System.Web.Http;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Data.FileSystem;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UmbracoFormsTranslations.Controllers
{
    [PluginController("UmbracoFormsTranslations")]
    public class ConvertController : UmbracoAuthorizedJsonController
    {
        private readonly FormsFileSystemForSavedData _fileSystem;
        private readonly ILocalizationService _localizationService;
        private const string _autoFormsKey = "UmbracoFormsTranslations";
        private const string _prefix = "UmbracoFormsTranslations";
        private const string _fileFormat = "forms/{0}.json";

        public ConvertController(FormsFileSystemForSavedData fileSystem, ILocalizationService localizationService)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        [HttpPost]
        public ConvertResponse IsValid(ConvertModel model)
        {
            try
            {
                if (_fileSystem.FileExists(string.Format(_fileFormat, model.Id)))
                    return new ConvertResponse(string.Empty) { ResultType = Enums.ResultType.Success };

                return new ConvertResponse("Form Not Valid") { ResultType = Enums.ResultType.Error };
            }
            catch (Exception ex)
            {
                Current.Logger.Error(typeof(ConvertController), ex);
                return new ConvertResponse("Form Not Valid") { ResultType = Enums.ResultType.Error };
            }
        }

        [HttpPost]
        public ConvertResponse Convert(ConvertModel model)
        {
            try
            {
                string path = string.Format(_fileFormat, model.Id);
                string fullPath = _fileSystem.GetFullPath(path);

                var form = new Url(fullPath)
                      .GetJsonAsync<Form>()
                      .GetAwaiter()
                      .GetResult();
                 
                if (form == null)
                    return new ConvertResponse("Form convertion was unsuccessful - form not found") { ResultType = Enums.ResultType.Error };

                ILanguage language = new Language(_localizationService.GetDefaultLanguageIsoCode());
                if (language == null)
                    return new ConvertResponse("Form convertion was unsuccessful - language") { ResultType = Enums.ResultType.Error };

                IDictionaryItem parent = AddOrUpdateDictionaryItem(language, _autoFormsKey);
                if (parent == null)
                    return new ConvertResponse("Form convertion was unsuccessful, parent") { ResultType = Enums.ResultType.Error };

                foreach (var field in form.AllFields)
                {
                    field.Caption = UpdateField(language, parent, field.Caption, ".Caption");
                    field.ToolTip = UpdateField(language, parent, field.ToolTip, ".ToolTip");
                    field.Placeholder = UpdateField(language, parent, field.Placeholder, ".Placeholder");
                    field.InvalidErrorMessage = UpdateField(language, parent, field.InvalidErrorMessage, ".InvalidErrorMessage");
                    field.RequiredErrorMessage = UpdateField(language, parent, field.RequiredErrorMessage, ".RequiredErrorMessage");

                    if (field.Settings.ContainsKey("Placeholder"))
                        field.Settings["Placeholder"] = UpdateField(language, parent, field.Settings["Placeholder"], ".Placeholder");

                    if (field.Settings.ContainsKey("DefaultValue"))
                        field.Settings["DefaultValue"] = UpdateField(language, parent, field.Settings["DefaultValue"], ".DefaultValue");                    
                }

                var request = JsonConvert.SerializeObject(form);
                if (string.IsNullOrWhiteSpace(request))
                    return new ConvertResponse("Form convertion was unsuccessful, serialisation") { ResultType = Enums.ResultType.Error };

                _fileSystem.AddFile(path, GenerateStreamFromString(request), true);

                AppCaches.RuntimeCache.Clear();

                return new ConvertResponse("Form converted any errors") { ResultType = Enums.ResultType.Success };

            }
            catch (Exception ex)
            {
                Current.Logger.Error(typeof(ConvertController), ex);
                return new ConvertResponse("Form convertion was unsuccessful") { ResultType = Enums.ResultType.Error };
            }
        }

        private string UpdateField(ILanguage language, IDictionaryItem parent, string fieldName, string suffix = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName)) return fieldName;
            if (fieldName.StartsWith("#")) return fieldName;

            var fieldAlias = fieldName.ToDictionaryItemName(_prefix, suffix);
            AddOrUpdateDictionaryItem(language, fieldAlias, fieldName, parent.Key);
            return $"#{fieldAlias}";
        }

        private IDictionaryItem AddOrUpdateDictionaryItem(ILanguage language, string key, string value = null, Guid? parentId = null)
        {
            var item = _localizationService.GetDictionaryItemByKey(key);
            if (item == null)
                return _localizationService.CreateDictionaryItemWithIdentity(key, parentId, value);

            _localizationService.AddOrUpdateDictionaryValue(item, language, value);
            return item;

        }
        private MemoryStream GenerateStreamFromString(string value) => new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty));
    }
}