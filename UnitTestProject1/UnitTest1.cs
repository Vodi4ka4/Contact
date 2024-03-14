using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Contact;

namespace UnitTestProject1
{
    [TestClass]
    public class Cont
    {
        [TestMethod]
        public void TestMethod_Check_phone_length_true()
        {
            ContactClass contactClass = new ContactClass();
            string phone = "89507704874";
            bool result = contactClass.Check_phone_length(phone);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestMethod_Check_phone_number_true()
        {
            ContactClass contactClass = new ContactClass();
            string phone = "89507704878";
            bool result = contactClass.Check_phone_number(phone);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestMethod_Check_phone_length_false()
        {
            ContactClass contactClass = new ContactClass();
            string phone = "8950770487";
            bool result = contactClass.Check_phone_length(phone);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void TestMethod_Check_phone_number_false()
        {
            ContactClass contactClass = new ContactClass();
            string phone = "8950770487f";
            bool result = contactClass.Check_phone_number(phone);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void TestMethod_Check_phone_null()
        {
            ContactClass contactClass = new ContactClass();
            string phone = null;
            bool result = contactClass.Check_phone_null(phone);
            Assert.IsFalse(result);
        }
    }
}
