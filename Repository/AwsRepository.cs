using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using IoC;
using Models;

namespace Repository;

public class AwsRepository : IAwsRepository
{
	private readonly string _bucketName;
	private readonly AmazonS3Client _s3Client;
	public AwsRepository()
	{
		AwsAccessModel model = IoCContainer.GetConstant<AwsAccessModel>(Constants.AWS_ACCESS);
		_bucketName = model.BucketName;
		AmazonS3Config configsS3 = new() {
				ServiceURL = model.ServiceUrl
		};
		AWSCredentials credentials = new BasicAWSCredentials(model.AccessKey, model.SecretKey);
		_s3Client = new AmazonS3Client(credentials, configsS3);
	}
	public async Task CreateFile(Stream stream, string name, string directoryName)
	{
		string fullPath = directoryName + "/" + name;
		using TransferUtility utility = new(_s3Client);
		await utility.UploadAsync(stream, _bucketName, fullPath);
	}

	public async Task DeleteFile(string name, string directoryName)
	{
		string fullPath = directoryName + "/" + name;
		await _s3Client.DeleteObjectAsync(_bucketName, fullPath);
	}
}