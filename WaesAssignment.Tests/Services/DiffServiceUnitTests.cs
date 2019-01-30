using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaesAssignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaesAssignment.Dto;
using WaesAssignment.DataServices;
using Moq;

namespace WaesAssignment.Services.Tests
{
    [TestClass]
    public class DiffServiceUnitTest
    {
        private static Mock<DiffService> _diffService { get; set; }
        const int _id = 1;
        const int _anotherId = 2;
        const string _validBase64Data = "ewogICJtYXkiOiBbCiAgICB0cnVlLAogICAgMTI1MTI2NDc0OS43ODUxODk2LAogICAgImxpcHMiLAogICAgImNhcmVmdWxseSIsCiAgICA0MjI2OTc4NDYuMTg1OTYyMiwKICAgIHsKICAgICAgImdvdCI6IDY0ODQ0MDE3Mi4wMTk4NzQ2LAogICAgICAiZ2xvYmUiOiAxMjM5ODY1NTM4LjE0OTgzNywKICAgICAgImNhdCI6IHRydWUsCiAgICAgICJmZWx0IjogImhheSIsCiAgICAgICJvcmdhbml6ZWQiOiAibG9uZ2VyIiwKICAgICAgInZhbGxleSI6IGZhbHNlCiAgICB9CiAgXSwKICAidW5kZXJsaW5lIjogImNvbmNlcm5lZCIsCiAgIm5lZWRsZSI6IC00NjU0NDQ3NSwKICAiYW50cyI6ICJvbmxpbmV0b29scyIsCiAgIm5vbmUiOiAieWVzIiwKICAidG9kYXkiOiAxNzIzMzczNjM5Cn0=";
        const string _anotherValidBase64Data = "ew0KICAic2t5Ijogew0KICAgICJwb2xpY2UiOiB0cnVlLA0KICAgICJnYXMiOiB7DQogICAgICAicGFydGx5IjogdHJ1ZSwNCiAgICAgICJhbnN3ZXIiOiB0cnVlLA0KICAgICAgImVnZyI6ICJmaW5nZXIiLA0KICAgICAgImdsYXNzIjogInNpbGsiLA0KICAgICAgInN1cHBvcnQiOiAtMjExMzg5Mzk4OC4xNDE5ODM1LA0KICAgICAgImtpdGNoZW4iOiBmYWxzZQ0KICAgIH0sDQogICAgImRpZmZpY3VsdCI6ICJiYXNlIiwNCiAgICAiYWNjZXB0IjogdHJ1ZSwNCiAgICAic3RhbmRhcmQiOiAxODM1MTgzNDA3LA0KICAgICJtb3VudGFpbiI6ICJwZW4iDQogIH0sDQogICJnYXZlIjogMTEyMzUxNDguMzE3MTEwNTM4LA0KICAic2hvd24iOiAxMTQwNzY1NjI4LjUzNzY0NzcsDQogICJhbm90aGVyIjogdHJ1ZSwNCiAgInBsYW5uZWQiOiAiaG9zcGl0YWwiLA0KICAiY2hpY2tlbiI6IHRydWUNCn0=";
        const string _invalidBase64Data = "ewogICJtYXkiOB0cnVlLAogICAgMTI1MTI2NDc0OS43ODUxODk2LAogICAgImxpcHMiLAogICAgImNhcmVmdWxseSIsCiAgICA0MjI2OTc4NDYuMTg1OTYyMiwKICAgIHsKICAgICAgImdvdCI6IDY0ODQ0MDE3Mi4wMTk4NzQ2LAogICAgICAiZ2xvYmUiOiAxMjM5ODY1NTM4LjE0OTgzNywKICAgICAgImNhdCI6IHRydWUsCiAgICAgICJmZWx0IjogImhheSIsCiAgICAgICJvcmdhbml6ZWQiOiAibG9uZ2VyIiwKICAgICAgInZhbGxleSI6IGZhbHNlCiAgICB9CiAgXSwKICAidW5kZXJsaW5lIjogImNvbmNlcm5lZCIsCiAgIm5lZWRsZSI6IC00NjU0NDQ3NSwKICAiYW50cyI6ICJvbmxpbmV0b29scyIsCiAgIm5vbmUiOiAieWVzIiwKICAidG9kYXkiOiAxNzIzMzczNjM5Cn0";

        [TestMethod()]
        public void Add_Valid_JsonBase64Data()
        {
            string leftKey = GetKey(_id, DiffTypes.Left);
            var mockMemoryCacheDataService = new Mock<IMemoryCacheDataService>();
            mockMemoryCacheDataService
                .Setup(x => x.Add(leftKey, _validBase64Data, DateTime.Today.AddYears(1)))
                    .Returns(true);

            Mock<DiffService> diffService = new Mock<DiffService>(mockMemoryCacheDataService.Object);

            var actual = diffService.Object.Add(_id, DiffTypes.Left, _validBase64Data);
            var expected = new ServiceResultWrapper() { Code = ServiceResultCode.Ok };

            Assert.IsTrue(actual != null);
            Assert.AreEqual(expected.Success, actual.Success);

        }


        [TestMethod()]
        public void Add_Invalid_JsonBase64Data()
        {
            string leftKey = GetKey(_id, DiffTypes.Left);
            var mockMemoryCacheDataService = new Mock<IMemoryCacheDataService>();
            mockMemoryCacheDataService
                .Setup(x => x.Add(leftKey, _invalidBase64Data, DateTime.Today.AddYears(1)))
                    .Returns(true);

            Mock<DiffService> diffService = new Mock<DiffService>(mockMemoryCacheDataService.Object);

            var actual = diffService.Object.Add(_id, DiffTypes.Left, _invalidBase64Data);
            var expected = new ServiceResultWrapper() { Code = ServiceResultCode.BadRequest };

            Assert.IsTrue(actual != null);
            Assert.AreEqual(expected.Success, actual.Success);
        }

        [TestMethod()]
        public void ComputeDifference_Match()
        {
            string leftKey = GetKey(_id, DiffTypes.Left);
            string rightKey = GetKey(_id, DiffTypes.Right);

            var mockMemoryCacheDataService = new Mock<IMemoryCacheDataService>();

            mockMemoryCacheDataService
                .Setup(x => x.Any(It.Is<string>(q => q == leftKey))).Returns(true);

            mockMemoryCacheDataService
                .Setup(x => x.Any(It.Is<string>(q => q == rightKey))).Returns(true);

            mockMemoryCacheDataService
                .Setup(x => x.Get(It.Is<string>(q => q == leftKey))).Returns(_validBase64Data);

            mockMemoryCacheDataService
                .Setup(x => x.Get(It.Is<string>(q => q == rightKey))).Returns(_validBase64Data);

            Mock<DiffService> diffService = new Mock<DiffService>(mockMemoryCacheDataService.Object);

            var actual = diffService.Object.ComputeDifference(_id);
            var expected = new ServiceResultWrapper<DiffResultDto>() { Code = ServiceResultCode.Ok, Data = new DiffResultDto() { Code = DiffResultTypes.Match } };

            Assert.IsTrue(actual != null);
            Assert.AreEqual(expected.Success, actual.Success);
            Assert.AreEqual(expected.Data.Code, actual.Data.Code);
        }

        [TestMethod()]
        public void ComputeDifference_NoMatch()
        {
            string leftKey = GetKey(_id, DiffTypes.Left);
            string rightKey = GetKey(_id, DiffTypes.Right);

            var mockMemoryCacheDataService = new Mock<IMemoryCacheDataService>();

            mockMemoryCacheDataService
                .Setup(x => x.Any(It.Is<string>(q => q == leftKey))).Returns(true);

            mockMemoryCacheDataService
                .Setup(x => x.Any(It.Is<string>(q => q == rightKey))).Returns(true);

            mockMemoryCacheDataService
                .Setup(x => x.Get(It.Is<string>(q => q == leftKey))).Returns(_validBase64Data);

            mockMemoryCacheDataService
                .Setup(x => x.Get(It.Is<string>(q => q == rightKey))).Returns(_anotherValidBase64Data);

            Mock<DiffService> diffService = new Mock<DiffService>(mockMemoryCacheDataService.Object);

            var actual = diffService.Object.ComputeDifference(_id);
            var expected = new ServiceResultWrapper<DiffResultDto>() { Code = ServiceResultCode.Ok, Data = new DiffResultDto() { Code = DiffResultTypes.NoMatch } };

            Assert.IsTrue(actual != null);
            Assert.AreEqual(expected.Success, actual.Success);
            Assert.AreEqual(expected.Data.Code, actual.Data.Code);
        }

        [TestMethod()]
        public void ComputeDifference_RightNotFound()
        {
            string leftKey = GetKey(_id, DiffTypes.Left);
            string rightKey = GetKey(_id, DiffTypes.Right);

            var mockMemoryCacheDataService = new Mock<IMemoryCacheDataService>();

            mockMemoryCacheDataService
                .Setup(x => x.Any(It.Is<string>(q => q == leftKey))).Returns(true);

            mockMemoryCacheDataService
                .Setup(x => x.Any(It.Is<string>(q => q == rightKey))).Returns(false);

            Mock<DiffService> diffService = new Mock<DiffService>(mockMemoryCacheDataService.Object);

            var actual = diffService.Object.ComputeDifference(_id);
            var expected = new ServiceResultWrapper<DiffResultDto>() { Code = ServiceResultCode.NotFound, Data = new DiffResultDto() { Code = DiffResultTypes.Unavailable } };

            Assert.IsTrue(actual != null);
            Assert.AreEqual(expected.Success, actual.Success);
            Assert.AreEqual(expected.Data.Code, actual.Data.Code);
        }


        private string GetKey(int id, string diffType)
        {
            return $"{diffType}_{id}";
        }
    }
}