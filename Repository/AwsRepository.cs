using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.Util;

namespace Repository;

public class AwsRepository : IAwsRepository
{
	private readonly string _bucketName;
	private readonly AmazonS3Client _s3Client;
	public AwsRepository(string serviceUrl, string accessKey, string secretKey, string bucketName)
	{
		_bucketName = bucketName;
		AmazonS3Config configsS3 = new() {
				ServiceURL = serviceUrl
		};
		AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
		_s3Client = new AmazonS3Client(credentials, configsS3);
	}
	public async Task CreateFile(Stream stream, string name, string directoryName)
	{
		TransferUtility utility = new(_s3Client);
		await utility.UploadAsync(stream, _bucketName, name);
	}

	public Task DeleteFile(string name, string directoryName)
	{
		throw new NotImplementedException();
	}
}