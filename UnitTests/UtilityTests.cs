using ServerlessFunc;
using System.Collections;
using System.Text;
using System.Text.Json;

namespace UnitTests
{
    [TestClass()]
    public class UtilityTests
    {
        [TestMethod()]
        public async Task BlobUtilityTest()
        {
            string blobName = "testblob";
            byte[] blobcontent = Encoding.ASCII.GetBytes("demotext");
            string containerName = "democontainer";
            string connectionString = "UseDevelopmentStorage=true";
            await BlobUtility.UploadSubmissionToBlob(blobName, blobcontent, connectionString, containerName);
            byte[] getBlobContent = await BlobUtility.GetBlobContentAsync(containerName, blobName, connectionString);
            await BlobUtility.DeleteContainer(containerName, connectionString);
            CollectionAssert.AreEqual(blobcontent, getBlobContent);
        }

        [TestMethod()]

        public void AnalysisResultEncodeDecodeTest()
        {
            AnalyzerResult result1 = new AnalyzerResult("100", 1, "Successful");
            string jsonString1 = JsonSerializer.Serialize(result1);
            byte[] analysisFile =  Encoding.UTF8.GetBytes(jsonString1);

            string jsonString2 = Encoding.UTF8.GetString(analysisFile);
            AnalyzerResult result2 = JsonSerializer.Deserialize<AnalyzerResult>(jsonString2);
            Assert.AreEqual(result1.AnalyserID, result2.AnalyserID);
            Assert.AreEqual(result1.Verdict, result2.Verdict);
            Assert.AreEqual(result1.ErrorMessage, result2.ErrorMessage);
        }
    }
}
