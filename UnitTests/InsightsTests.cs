using ServerlessFunc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace UnitTests
{
    [TestClass()]
    public class InsightsTests
    {
        private string analysisUrl = "http://localhost:7074/api/analysis";
        private string submissionUrl = "http://localhost:7074/api/submission";
        private string sessionUrl = "http://localhost:7074/api/session";
        private string insightsUrl = "http://localhost:7074/api/insights";

        private DownloadApi _downloadClient;
        private UploadApi _uploadClient;
        private InsightsApi _insightsClient;

        public InsightsTests()
        {
            _downloadClient = new DownloadApi(sessionUrl, submissionUrl, analysisUrl);
            _uploadClient = new UploadApi(sessionUrl, submissionUrl, analysisUrl);
            _insightsClient = new InsightsApi(insightsUrl);
        }

        public AnalysisData GetDummyAnalysisData(string sessionId, string studentName)
        {
            AnalysisData analysisData = new AnalysisData();
            analysisData.SessionId = sessionId;
            analysisData.UserName = studentName;
            Dictionary<string, int> map = new Dictionary<string, int>();
            map["Test1"] = 0;
            map["Test2"] = 1;
            string json = JsonSerializer.Serialize(map);
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            analysisData.AnalysisFile = byteArray;
            return analysisData;
        }
        public SessionData GetDummySessionData()
        {
            SessionData sessionData = new SessionData();
            sessionData.HostUserName = "name1";
            sessionData.SessionId = "1";
            List<string> Test = new List<string>
            {
                "Test1",
                "Test2"
            };
            sessionData.Tests = InsightsUtility.ListToByte(Test);
            return sessionData;
        }

        [TestMethod()]
        public async Task CompareTwoSessionsTest()
        {
            await _downloadClient.DeleteAllAnalysisAsync();
            AnalysisData analysisData1 = GetDummyAnalysisData("1", "Student1");
            AnalysisData analysisData2 = GetDummyAnalysisData("1", "Student2");
            AnalysisData analysisData3 = GetDummyAnalysisData("2", "Student1");
            AnalysisData analysisData4 = GetDummyAnalysisData("2", "Student2");
            await _uploadClient.PostAnalysisAsync(analysisData1);
            await _uploadClient.PostAnalysisAsync(analysisData2);
            await _uploadClient.PostAnalysisAsync(analysisData3);
            await _uploadClient.PostAnalysisAsync(analysisData4);

            List<Dictionary<string, int>> result = await _insightsClient.CompareTwoSessoins("1", "2");
            Assert.AreEqual(result[0]["Test1"], 0);
            Assert.AreEqual(result[0]["Test2"], 2);
            Assert.AreEqual(result[1]["Test1"], 0);
            Assert.AreEqual(result[1]["Test2"], 2);
            await _downloadClient.DeleteAllAnalysisAsync();
        }

        [TestMethod()]
        public async Task GetFailedStudentsGivenTestTest()
        {
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
            SessionData sessionData = GetDummySessionData();
            await _uploadClient.PostSessionAsync(sessionData);

            AnalysisData analysisData1 = GetDummyAnalysisData("1", "Student1");
            AnalysisData analysisData2 = GetDummyAnalysisData("1", "Student2");
            await _uploadClient.PostAnalysisAsync(analysisData1);
            await _uploadClient.PostAnalysisAsync(analysisData2);
            List<string> students = await _insightsClient.GetFailedStudentsGivenTest("name1", "Test1");
            students.Sort();
            List<string> expectedStudents = new List<string>
            {
                "Student1",
                "Student2"
            };
            CollectionAssert.AreEqual(expectedStudents, students);
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
        }
    }
}