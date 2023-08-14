using NUnit.Framework;
using CentralFill;

/* This is an example of a unit test.
 * Normally, this would be much more extensive.
 */

namespace UnitTests
{
    [TestFixture()]
    public class UnitTest1
    {
        clsCentralFillFacility facility = new clsCentralFillFacility(0);
        Program.facilityInfo facInfo;

        [SetUp]
        public void Setup()
        {
            // create facility
            facility.Medication_A_Cost = (decimal)23.40;
            facility.Medication_B_Cost = (decimal)23.30;
            facility.Medication_C_Cost = (decimal)23.50;
            // create facility info record
            facInfo.facility = facility;
        }

        [Test()]
        public void findCheapestMedTest()
        {
            Program.findCheapestMed(ref facInfo);
            Assert.That(facInfo.cheapestMed, Is.EqualTo("B"));
        }

        [Test()]
        public void findCheapestMedTestEqual()
        {
            facility.Medication_A_Cost = (decimal)23.40;
            facility.Medication_B_Cost = (decimal)23.40;
            Program.findCheapestMed(ref facInfo);
            Assert.That(facInfo.cheapestMed, Is.EqualTo("A"));
        }
    }
}

