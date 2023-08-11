namespace Repository;

public class AwsAccessModel
{
	public AwsAccessModel(string serviceUrl, string accessKey, string secretKey, string bucketName)
	{
		ServiceUrl = serviceUrl;
		AccessKey = accessKey;
		SecretKey = secretKey;
		BucketName = bucketName;
	}

	public string ServiceUrl { get; }
	public string AccessKey { get; }
	public string SecretKey { get; }
	public string BucketName { get; }
}