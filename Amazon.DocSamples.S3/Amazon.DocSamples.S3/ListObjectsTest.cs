using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.DocSamples.S3
{
    class ListObjectsTest
    {
        const string accessKeyId = "#################################";
        const string accessKeySecret = "#################################";
        const string endpoint = "https://s3.cn-east-2.jdcloud-oss.com";

        private const string bucketName = "s3fs-test";

        private static IAmazonS3 s3Client;
        static void Main(string[] args)
        {
            var s3ClientConfig = new AmazonS3Config
            {
                ServiceURL = endpoint,
                SignatureVersion = "4",
                UseHttp = true,
            };
            s3Client = new AmazonS3Client(accessKeyId, accessKeySecret, s3ClientConfig);

            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = "Item3",
                ContentBody = "This is sample content...",
                UseChunkEncoding = false
            };

            PutObjectResponse response = s3Client.PutObject(request);

            ListingObjectsAsync().Wait();

            Console.ReadLine();
        }

        static async Task ListingObjectsAsync()
        {
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 10
                };
                ListObjectsV2Response response;
                do
                {
                    response = await s3Client.ListObjectsV2Async(request);

                    // Process the response.
                    foreach (S3Object entry in response.S3Objects)
                    {
                        Console.WriteLine("key = {0} size = {1}",
                            entry.Key, entry.Size);
                    }
                    Console.WriteLine("Next Continuation Token: {0}", response.NextContinuationToken);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                Console.WriteLine("S3 error occurred. Exception: " + amazonS3Exception.ToString());
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                Console.ReadKey();
            }
        }
    }
}
