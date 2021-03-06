// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Threading.Tasks;
using Xenko.Core.Assets;
using Xenko.Core.Assets.Compiler;
using Xenko.Core.BuildEngine;
using Xenko.Core;
using Xenko.Core.Serialization.Contents;
using Xenko.Graphics;
using Xenko.Rendering.RenderTextures;

namespace Xenko.Assets.Textures
{
    [AssetCompiler(typeof(RenderTextureAsset), typeof(AssetCompilationContext))]
    public class RenderTextureAssetCompiler : AssetCompilerBase
    {
        protected override void Prepare(AssetCompilerContext context, AssetItem assetItem, string targetUrlInStorage, AssetCompilerResult result)
        {
            var asset = (RenderTextureAsset)assetItem.Asset;
            var colorSpace = context.GetColorSpace();

            result.BuildSteps = new AssetBuildStep(assetItem);
            result.BuildSteps.Add(new RenderTextureConvertCommand(targetUrlInStorage, new RenderTextureParameters(asset, colorSpace), assetItem.Package));
        }

        /// <summary>
        /// Command used to convert the texture in the storage
        /// </summary>
        private class RenderTextureConvertCommand : AssetCommand<RenderTextureParameters>
        {
            public RenderTextureConvertCommand(string url, RenderTextureParameters parameters, IAssetFinder assetFinder)
                : base(url, parameters, assetFinder)
            {
            }

            protected override Task<ResultStatus> DoCommandOverride(ICommandContext commandContext)
            {
                var assetManager = new ContentManager(MicrothreadLocalDatabases.ProviderService);
                assetManager.Save(Url, new RenderTextureDescriptor
                {
                    Width = Parameters.Asset.Width,
                    Height = Parameters.Asset.Height,
                    Format = Parameters.Asset.Format,
                    ColorSpace = Parameters.Asset.IsSRgb(Parameters.ColorSpace) ? ColorSpace.Linear : ColorSpace.Gamma,
                });

                return Task.FromResult(ResultStatus.Successful);
            }
        }

        [DataContract]
        public struct RenderTextureParameters
        {
            public RenderTextureAsset Asset;
            public ColorSpace ColorSpace;

            public RenderTextureParameters(RenderTextureAsset asset, ColorSpace colorSpace)
            {
                Asset = asset;
                ColorSpace = colorSpace;
            }
        }
    }
}
