using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneBox_DataAccess.Repositories.Azure;
using OneBox_DataAccess.Infrastucture.Azure.Storage;
using System.Collections;
using OneBox_Infrastructure.DataTransferObjects;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class UnitTestBlobkBlobRepository
    {
        [TestMethod]
        public void TestGetFiles()
        {
            IAzureRepository fakeRepo = new BlockBlobRepository(new MockCloudBlobContainerServices());
            string containerName = "test_container";
            fakeRepo.ConfigureContainer(containerName);
            List<FileDto> list = fakeRepo.GetFiles(containerName);

            string fullPath_1 = "/" + containerName + "/folder1/";
            Assert.AreEqual(fullPath_1, list[0].fullPath);
            string fullPath_2 = "/" + containerName + "/file1.txt/";
            Assert.AreEqual(fullPath_2,list[1].fullPath);

            list = fakeRepo.GetFiles(containerName + "/folder1");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("/"+containerName + "/folder1" + "/file1.txt/", list[0].fullPath);

        }
    }
}
