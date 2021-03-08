using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace UmbracoFormsTranslations.Composers
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class UmbracoFormsTranslationsComposer : ComponentComposer<AddConvertToFormsMenuComponent>, IUserComposer
    {
    }

    public class AddConvertToFormsMenuComponent : IComponent
    {
        public void Initialize()
        {
            TreeControllerBase.MenuRendering += (s, evt) =>
            {
                switch (s.TreeAlias)
                {
                    case "form":
                        if (evt.NodeId != "0" &&
                            evt.NodeId != Umbraco.Core.Constants.System.RecycleBinMedia.ToString() &&
                            evt.NodeId != Umbraco.Core.Constants.System.Root.ToString())
                        {
                            var textService = DependencyResolver.Current.GetService<ILocalizedTextService>();

                            var convert = new MenuItem("convertNode", textService?.Localize(Constants.Lang.Convert.ConvertForm));
                            convert.AdditionalData.Add("actionView", "/App_Plugins/UmbracoFormsTranslations/backoffice/convert/convert.html");
                            convert.Icon = "settings color-blue";
                            evt.Menu.Items.Add(convert);
                        }
                        break;
                }
            };
        }

        public void Terminate() { }
    }
}
