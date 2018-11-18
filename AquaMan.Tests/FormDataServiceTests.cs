using AquaMan.Models;
using AquaMan.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AquaMan.Tests
{
    [TestClass]
    public class FormDataServiceTests
    {
        private const string feedingXML = @"<FeedingTimes>
                                              <Pinches>3</Pinches>
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
            double expectedPinches = 3;
            List<DateTime> expectedTimes = new List<DateTime>();
            expectedTimes.Add(new DateTime(2018, 10, 10, 1, 0, 0));
            expectedTimes.Add(new DateTime(2018, 10, 10, 14, 1, 0));
            expectedTimes.Add(new DateTime(2018, 10, 10, 17, 0, 0));
            expectedTimes.Add(new DateTime(2018, 10, 10, 2, 0, 0));
            expectedTimes.Add(new DateTime(2018, 10, 10, 11, 0, 0));


            FormDataService formDataService = new FormDataService();
            FeedingTimes feedingTimes = formDataService.DeSerializeFeedingTimes(feedingXML);
            Assert.AreEqual(expectedPinches, feedingTimes.Pinches);
            CollectionAssert.AreEqual(expectedTimes, feedingTimes.Feedings, $"feeding times are not the same");
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
