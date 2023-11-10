using ServerlessFunc;
using System.Text;
using System.Text.Json;

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

        public AnalysisData GetDummyAnalysisData(string sessionId, string studentName, Dictionary<string, int> map)
        {
            AnalysisData analysisData = new AnalysisData();
            analysisData.SessionId = sessionId;
            analysisData.UserName = studentName;
            string json = JsonSerializer.Serialize(map);
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            analysisData.AnalysisFile = byteArray;
            return analysisData;
        }
        public SessionData GetDummySessionData(string hostName, string sessionId, List<string> tests)
        {
            SessionData sessionData = new SessionData();
            sessionData.HostUserName = hostName;
            sessionData.SessionId = sessionId;

            sessionData.Tests = InsightsUtility.ListToByte(tests);
            return sessionData;
        }
        public SessionData GetDummySessionDataWithStudents(string hostName, string sessionId, List<string> tests,List<string> students)
        {
            SessionData sessionData = new SessionData();
            sessionData.HostUserName = hostName;
            sessionData.SessionId = sessionId;           
            sessionData.Tests = InsightsUtility.ListToByte(tests);
            sessionData.Students = InsightsUtility.ListToByte(students);
            return sessionData;
        }

        public async Task FillLotsOfRandomData()
        {
            SessionData sessionData = GetDummySessionData("name1", "1", ["Test1", "Test2"]);
            SessionData sessionData2 = GetDummySessionData("name1", "2", ["Test1", "Test2"]);
            SessionData sessionData3 = GetDummySessionData("name1", "3", ["Test1", "Test2"]);

            AnalysisData analysisData1 = GetDummyAnalysisData("1", "Student1", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
            AnalysisData analysisData2 = GetDummyAnalysisData("1", "Student2", new Dictionary<string, int>() { { "Test1", 1 }, { "Test2", 1 } });
            AnalysisData analysisData3 = GetDummyAnalysisData("2", "Student1", new Dictionary<string, int>() { { "Test1", 1 }, { "Test2", 1 } });
            AnalysisData analysisData4 = GetDummyAnalysisData("2", "Student2", new Dictionary<string, int>() { { "Test1", 1 }, { "Test2", 1 } });
            AnalysisData analysisData10 = GetDummyAnalysisData("3", "Student1", new Dictionary<string, int>() { { "Test1", 1 }, { "Test2", 1 } });
            AnalysisData analysisData11 = GetDummyAnalysisData("3", "Student2", new Dictionary<string, int>() { { "Test1", 1 }, { "Test2", 1 } });

            await _uploadClient.PostSessionAsync(sessionData);
            await _uploadClient.PostSessionAsync(sessionData2);
            await _uploadClient.PostSessionAsync(sessionData3);
            await _uploadClient.PostAnalysisAsync(analysisData1);
            await _uploadClient.PostAnalysisAsync(analysisData2);
            await _uploadClient.PostAnalysisAsync(analysisData3);
            await _uploadClient.PostAnalysisAsync(analysisData4);
            await _uploadClient.PostAnalysisAsync(analysisData10);
            await _uploadClient.PostAnalysisAsync(analysisData11);

            SessionData sessionData4 = GetDummySessionData("name2", "4", ["Test1", "Test3"]);
            SessionData sessionData5 = GetDummySessionData("name2", "5", ["Test1", "Test3"]);
            SessionData sessionData6 = GetDummySessionData("name2", "6", ["Test1", "Test5"]);

            AnalysisData analysisData5 = GetDummyAnalysisData("4", "Student1", new Dictionary<string, int>() { { "Test1", 0 }, { "Test3", 1 } });
            AnalysisData analysisData6 = GetDummyAnalysisData("4", "Student2", new Dictionary<string, int>() { { "Test1", 1 }, { "Test3", 1 } });
            AnalysisData analysisData7 = GetDummyAnalysisData("5", "Student1", new Dictionary<string, int>() { { "Test1", 1 }, { "Test3", 1 } });
            AnalysisData analysisData8 = GetDummyAnalysisData("5", "Student2", new Dictionary<string, int>() { { "Test1", 0 }, { "Test3", 1 } });
            AnalysisData analysisData9 = GetDummyAnalysisData("6", "Student1", new Dictionary<string, int>() { { "Test1", 0 }, { "Test5", 0 } });

            await _uploadClient.PostSessionAsync(sessionData4);
            await _uploadClient.PostSessionAsync(sessionData5);
            await _uploadClient.PostSessionAsync(sessionData6);
            await _uploadClient.PostAnalysisAsync(analysisData5);
            await _uploadClient.PostAnalysisAsync(analysisData6);
            await _uploadClient.PostAnalysisAsync(analysisData7);
            await _uploadClient.PostAnalysisAsync(analysisData8);
            await _uploadClient.PostAnalysisAsync(analysisData9);
        }
        [TestMethod()]
        public async Task CompareTwoSessionsTest()
        {
            await _downloadClient.DeleteAllAnalysisAsync();
            AnalysisData analysisData1 = GetDummyAnalysisData("1", "Student1", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
            AnalysisData analysisData2 = GetDummyAnalysisData("1", "Student2", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
            AnalysisData analysisData3 = GetDummyAnalysisData("2", "Student1", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
            AnalysisData analysisData4 = GetDummyAnalysisData("2", "Student2", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
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
            SessionData sessionData = GetDummySessionData("name1", "1", ["Test1", "Test2"]);
            await _uploadClient.PostSessionAsync(sessionData);

            AnalysisData analysisData1 = GetDummyAnalysisData("1", "Student1", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
            AnalysisData analysisData2 = GetDummyAnalysisData("1", "Student2", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
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

        [TestMethod()]
        public async Task RunningAverageOnGivenTestTest()
        {
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
            await FillLotsOfRandomData();
            List<double> averageList = await _insightsClient.RunningAverageOnGivenTest("name1", "Test1");
            Assert.AreEqual(averageList[0], 50);
            Assert.AreEqual(averageList[1], 100);
            Assert.AreEqual(averageList[2], 100);
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
        }

        [TestMethod()]
        public async Task RunningAverageOnGivenStudentTest()
        {
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
            await FillLotsOfRandomData();
            List<double> averageList = await _insightsClient.RunningAverageOnGivenStudent("name2", "Student1");
            Assert.AreEqual(averageList[0], 50);
            Assert.AreEqual(averageList[1], 100);
            Assert.AreEqual(averageList[2], 0);
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
        }

        [TestMethod()]
        public async Task RunningAverageAcrossSessoinsTest()
        {
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
            await FillLotsOfRandomData();
            List<double> averageList = await _insightsClient.RunningAverageAcrossSessoins("name2");
            averageList.Sort();
            Assert.AreEqual(averageList[0], 0);
            Assert.AreEqual(averageList[1], 75);
            Assert.AreEqual(averageList[2], 75);

            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
        }

        [TestMethod()]
        public async Task StudentsWithoutAnalysisTest()
        {
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
            SessionData sessionData = GetDummySessionDataWithStudents("name1", "1", ["Test1", "Test2"], ["Student1", "Student2"]);
            await _uploadClient.PostSessionAsync(sessionData);
            AnalysisData analysisData1 = GetDummyAnalysisData("1", "Student1", new Dictionary<string, int>() { { "Test1", 0 }, { "Test2", 1 } });
            await _uploadClient.PostAnalysisAsync(analysisData1);
            List<string> studentsList = await _insightsClient.UsersWithoutAnalysisGivenSession("1");
            await _downloadClient.DeleteAllAnalysisAsync();
            await _downloadClient.DeleteAllSessionsAsync();
            Assert.AreEqual(studentsList.Count, 1);
            Assert.AreEqual(studentsList[0], "Student2");
        }
    }
}
