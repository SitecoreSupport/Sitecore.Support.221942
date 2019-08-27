namespace Sitecore.FXM.Pipelines.Bundle
{
    using Sitecore;
    using Sitecore.Abstractions;
    using Sitecore.Diagnostics;
    using Sitecore.FXM.Abstractions;
    using Sitecore.FXM.Configuration;
    using Sitecore.StringExtensions;
    using System;
    using System.Web.Optimization;

    [UsedImplicitly]
    public class BeaconLoaderProcessor : IBundleProcessor
    {
        private readonly BaseSettings _settings;
        private readonly IFileUtil _fileUtil;

        public BeaconLoaderProcessor(BaseSettings settings, IFileUtil fileUtil)
        {
            Assert.ArgumentNotNull(settings, "settings");
            Assert.ArgumentNotNull(fileUtil, "fileUtil");
            this._settings = settings;
            this._fileUtil = fileUtil;
        }

        protected virtual string GenerateFileOutput(string endpoint)
        {
            return this._fileUtil.UnmapPath(string.Format("{0}/beacon_{1:N}.js", this._settings.Fxm().BundledJsFilesPath, endpoint.GenerateGuid())).Replace("//", "/");
        }

        public void Process(BundleGeneratorArgs args)
        {
            string endpoint = this.ResolveEndpoint(args.HostName);
            string path = this.GenerateFileOutput(endpoint);
            this._fileUtil.WriteToFile(path, string.Format("SCBeacon = new SCBeacon(\"{0}\");", endpoint), false);
            args.Bundle.Include("~" + path, Array.Empty<IItemTransform>());
        }

        protected virtual string ResolveEndpoint(string hostName)
        {
            char[] trimChars = new char[] { '/' };
            string str = this._settings.GetSetting("Sitecore.Services.RouteBase", "sitecore/api/ssc/").TrimEnd(trimChars);
            string str2 = "Beacon.Service".Replace('.', '/');
            object[] args = new object[4];
            args[0] = this._settings.Fxm().Protocol;
            char[] chArray2 = new char[] { '/' };
            args[1] = hostName.TrimEnd(chArray2);
            args[2] = str;
            args[3] = str2;
            return string.Format("{0}{1}/{2}/{3}", args);
        }
    }
}