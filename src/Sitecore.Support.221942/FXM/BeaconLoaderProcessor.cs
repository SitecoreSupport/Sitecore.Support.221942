using Sitecore.Abstractions;
using Sitecore.Configuration;
using Sitecore.FXM.Abstractions;
using Sitecore.FXM.Pipelines.Bundle;
using Sitecore.FXM.Utilities;
using Sitecore.StringExtensions;
using System;
using System.Web.Optimization;

namespace Sitecore.Support.FXM
{
    public class BeaconLoaderProcessor : IBundleProcessor<OnRequestBundleGeneratorArgs>
    {
        private readonly IFileUtil fileUtil;
        private readonly string protocol;
        private readonly string service;
        private readonly ISettings settings;

        public BeaconLoaderProcessor() : this(new SettingsWrapper(), new FileUtilWrapper())
        {
        }

        public BeaconLoaderProcessor(ISettings settings, IFileUtil fileUtil)
        {
            this.settings = settings;
            this.fileUtil = fileUtil;
            this.protocol = this.settings.GetSetting("FXM.Protocol", string.Empty);
            this.service = "Beacon.Service".Replace('.', '/');
        }

        public void Process(OnRequestBundleGeneratorArgs args)
        {
            string setting = this.settings.GetSetting("Sitecore.Services.RouteBase", "sitecore/api/ssc/");

            #region----modified part of code to use "FXM.Hostname" setting---------
            string requestUriHost = FxmUtility.GetUriHost(args.Context.Request.Url);
            string uriHost = Sitecore.Configuration.Settings.GetSetting("FXM.Hostname");
            if (string.IsNullOrEmpty(uriHost))
            {
                uriHost = requestUriHost;
            }
            #endregion----------

            string str3 = "{0}{1}/{2}/{3}".FormatWith(new object[] { this.protocol, uriHost.TrimEnd(new char[] { '/' }), setting.TrimEnd(new char[] { '/' }), this.service });
            string uniqueFilename = this.fileUtil.GetUniqueFilename(Settings.TempFolderPath + "/beacon.js");
            string text = "SCBeacon = new SCBeacon(\"" + str3 + "\");";
            this.fileUtil.WriteToFile(uniqueFilename, text, true);
            args.Bundle.Include("~" + uniqueFilename, new IItemTransform[0]);
        }
    }
}
