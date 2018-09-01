using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace PageObject
{
    [TestClass]
    public class PrestaTests
    {
        public IWebDriver driver { get; set; }
        public WebDriverWait wait { get; set; }

        [TestInitialize]
        public void SetupTest()
        {
            this.driver = new ChromeDriver();
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }
        [TestCleanup]
        public void TeardownTest()
        {
            this.driver.Quit();
        }
        [TestMethod]
        public void OpenSite()
        {
            PrestashopMainPage prestaMain = new PrestashopMainPage(this.driver);
            prestaMain.Navigate();
        }
    }
}
