using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.PageObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PageObject
{
    public class PrestashopMainPage
    {
        private readonly IWebDriver driver;
        private readonly string url = @"http://prestashop-automation.qatestlab.com.ua/ru/";

        public PrestashopMainPage(IWebDriver browser)
        {
            this.driver = browser;
            this.driver.Manage().Window.Maximize();
            PageFactory.InitElements(browser, this);
        }

        public void Navigate()
        {
            this.driver.Navigate().GoToUrl(url);
            Console.WriteLine("Site is opened");
        }
    }
}
