using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Xunit.Sdk;

namespace HotelBooking.UnitTests
{
    public class CustomData : DataAttribute
    {
        private readonly string _filePath;

        public CustomData(string relativePath)
        {
            //Absolute path
            string testProjectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filePath = Path.Combine(testProjectPath, relativePath);
        }

        public override IEnumerable<object[]> GetData(MethodInfo testmethod)
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Failed in finding the path: {_filePath}");
            }
            var jsonData = File.ReadAllText(_filePath);
            //JSON should be a collection of test cases, each test case is an array of objects
            var data = JsonConvert.DeserializeObject<List<object[]>>(jsonData);
            return data;
        }
    }
}
        
