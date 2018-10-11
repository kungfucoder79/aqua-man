using AquaMan.Models;
using AquaMan.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AquaMan.Tests
{
    [TestClass]
    public class FormDataServiceTests
    {
        private const string feedingXML = @"<FeedingTimes>
                                              <Time1>10/10/2018 1:00:00 AM</Time1>
                                              <Time2>10/10/2018 2:01:00 PM</Time2>
                                              <Time3>10/10/2018 5:00:00 PM</Time3>
                                              <Time4>10/10/2018 2:00:00 AM</Time4>
                                              <Time5>10/10/2018 11:00:00 AM</Time5>
                                            </FeedingTimes>";

        private const string tankSpecsXML = @"<TankSpecs>
                                              <Height>6</Height>
                                              <Width>12</Width>
                                              <Depth>22</Depth>
                                            </TankSpecs>";

        [TestMethod]
        public void FormDataService_DeSerializeFeedingTimes_Deserializes()
        {
            DateTime expectedTime1 = new DateTime(2018, 10, 10, 1, 0, 0);
            DateTime expectedTime2 = new DateTime(2018, 10, 10, 14, 1, 0);
            DateTime expectedTime3 = new DateTime(2018, 10, 10, 17, 0, 0);
            DateTime expectedTime4 = new DateTime(2018, 10, 10, 2, 0, 0);
            DateTime expectedTime5 = new DateTime(2018, 10, 10, 11, 0, 0);


            FormDataService formDataService = new FormDataService();
            FeedingTimes feedingTimes = formDataService.DeSerializeFeedingTimes(feedingXML);

            Assert.AreEqual(expectedTime1, feedingTimes.Time1, $"{nameof(feedingTimes.Time1)} is not the same");
            Assert.AreEqual(expectedTime2, feedingTimes.Time2, $"{nameof(feedingTimes.Time2)} is not the same");
            Assert.AreEqual(expectedTime3, feedingTimes.Time3, $"{nameof(feedingTimes.Time3)} is not the same");
            Assert.AreEqual(expectedTime4, feedingTimes.Time4, $"{nameof(feedingTimes.Time4)} is not the same");
            Assert.AreEqual(expectedTime5, feedingTimes.Time5, $"{nameof(feedingTimes.Time5)} is not the same");
        }

        [TestMethod]
        public void FormDataService_DeSerializeTankSpecs_Deserializes()
        {
            double expectedHeight = 6;
            double expectedWidth = 12;
            double expectedDepth = 22;


            FormDataService formDataService = new FormDataService();
            TankSpecs tankSpecs = formDataService.DeSerializeTankSpecs(tankSpecsXML);

            Assert.AreEqual(expectedDepth, tankSpecs.Depth, $"{nameof(tankSpecs.Depth)} is not the same");
            Assert.AreEqual(expectedWidth, tankSpecs.Width, $"{nameof(tankSpecs.Width)} is not the same");
            Assert.AreEqual(expectedHeight, tankSpecs.Height, $"{nameof(tankSpecs.Height)} is not the same");
        }
    }
}
