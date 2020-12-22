using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreUtils.Google;
using Xunit;

namespace NCoreUtils
{
    public abstract class GoogleUnitTestsBase : IDisposable
    {
        protected const string BucketName = "ncoreutils-integration";

        protected const string ObjectName = "tmp.jpg";

        protected const string ObjectAltName = "tmp2.jpg";

        private const string ObjectContentType = "image/jpeg";

        private const string ObjectCacheControl = "public, max-age=1000";

        protected static readonly byte[] _imageData;

        static GoogleUnitTestsBase()
        {
            using var stream = typeof(GoogleUnitTestsBase).Assembly.GetManifestResourceStream("NCoreUtils.Resources.img.jpg");
            using var buffer = new MemoryStream();
            stream!.CopyTo(buffer);
            _imageData = buffer.ToArray();
        }

        protected readonly ServiceProvider _serviceProvider;

        public GoogleUnitTestsBase(Action<IServiceCollection>? initServices)
        {
            var serviceCollection = new ServiceCollection();
            initServices?.Invoke(serviceCollection);
            _serviceProvider = serviceCollection
                .AddHttpClient()
                .AddGoogleCloudStorageUtils()
                .BuildServiceProvider(false);
        }

        protected virtual async Task ValidateExists(GoogleCloudStorageUtils utils, string name, bool testMeta = false)
        {
            var obj = await utils.GetAsync(BucketName, name);
            Assert.NotNull(obj);
            Assert.Equal(BucketName, obj!.BucketName);
            Assert.Equal(name, obj.Name);
            Assert.Equal(ObjectContentType, obj.ContentType);
            Assert.Equal(ObjectCacheControl, obj.CacheControl);
            Assert.Equal(_imageData.Length, (long)obj.Size!.Value);
            using var buffer = new MemoryStream();
            await utils.DownloadAsync(BucketName, name, buffer);
            Assert.True(_imageData.SequenceEqual(buffer.ToArray()));
            if (testMeta)
            {
                var guid = Guid.NewGuid().ToString();
                var user = "user-artyom@skape.io";
                var obj1 = await utils.PathAsync(BucketName, name, new GoogleObjectPatchData
                {
                    Acl = new List<GoogleAccessControlEntry> { new GoogleAccessControlEntry { Role = "READER", Entity = user }},
                    Metadata = new Dictionary<string, string> { { "guid", guid } }
                }, true);
                Assert.NotNull(obj1);
                Assert.NotNull(obj1!.Acl);
                Assert.Contains(obj1.Acl, e => e.Role == "READER" && e.Entity == user);
                Assert.NotNull(obj1.Metadata);
                Assert.Contains(obj1.Metadata, kv => kv.Key == "guid" && kv.Value == guid);
            }
        }

        protected virtual async Task ValidateDoesNotExist(GoogleCloudStorageUtils utils, string name)
        {
            var obj = await utils.GetAsync(BucketName, name);
            Assert.Null(obj);
        }

        public virtual async Task CreateGetCopyDelete(IMemoryOwner<byte>? chunkBuffer)
        {
            var utils = _serviceProvider.GetRequiredService<GoogleCloudStorageUtils>();
            {
                using var source = new MemoryStream(_imageData, 0, _imageData.Length, false, true);
                var obj = await utils.UploadAsync(
                    BucketName,
                    ObjectName,
                    source,
                    contentType: ObjectContentType,
                    cacheControl: ObjectCacheControl,
                    isPublic: true,
                    buffer: chunkBuffer
                );
                Assert.NotNull(obj);
                Assert.Equal(BucketName, obj.BucketName);
                Assert.Equal(ObjectName, obj.Name);
                Assert.Equal(ObjectContentType, obj.ContentType);
                Assert.Equal(ObjectCacheControl, obj.CacheControl);
                Assert.Equal(_imageData.Length, (long)obj.Size!.Value);
            }
            await ValidateExists(utils, ObjectName, true);
            await utils.CopyAsync(BucketName, ObjectName, BucketName, ObjectAltName, ObjectContentType, ObjectCacheControl, true);
            await ValidateExists(utils, ObjectName);
            await ValidateExists(utils, ObjectAltName);
            await utils.DeleteAsync(BucketName, ObjectName);
            await ValidateDoesNotExist(utils, ObjectName);
            await ValidateExists(utils, ObjectAltName);
            await utils.DeleteAsync(BucketName, ObjectAltName);
            await ValidateDoesNotExist(utils, ObjectName);
            await ValidateDoesNotExist(utils, ObjectAltName);
        }

        public void Dispose()
            => _serviceProvider.Dispose();
    }
}