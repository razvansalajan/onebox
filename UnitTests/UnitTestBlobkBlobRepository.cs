using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneBox_DataAccess.Repositories.Azure;
using OneBox_DataAccess.Infrastucture.Azure.Storage;
using System.Collections;
using OneBox_Infrastructure.DataTransferObjects;
using System.Collections.Generic;
using System.IO;
using OneBox_DataAccess.Utilities.Mocks;

namespace UnitTests
{
    [TestClass]
    public class UnitTestBlobkBlobRepository
    {
        [TestMethod]
        public void TestGetFiles()
        {
            IAzureRepository fakeRepo = new BlockBlobRepository(new MockCloudBlobContainerServices());
            string containerName = "root";
            fakeRepo.ConfigureContainer(containerName);
            fakeRepo.CreateNewFolder("root", "poze");
            fakeRepo.AddNewFile("root/poze", "ceva.jpg", new FileStreamMock(45) );
            fakeRepo.AddNewFile("root", "ceva.txt", new FileStreamMock(66));
            fakeRepo.AddNewFile("root/poze/poze2", "ceva2.jpg", new FileStreamMock(5));
            List<FileDto> list = fakeRepo.GetFiles(containerName);
            Assert.AreEqual(2, list.Count);


            List<string> expected = new List<string>();
            expected.Add("ceva.jpg");
            expected.Add("poze2");
            list = fakeRepo.GetFiles(containerName + "/poze");
            Assert.AreEqual(expected.Count,list.Count);
            for(int i=0; i<list.Count; ++i)
            {
                Assert.AreEqual(list[i].name, expected[i]);
            }
        }
    }
}
