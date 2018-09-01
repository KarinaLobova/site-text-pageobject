using System;
using System.Text.RegularExpressions;
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
        public void TestSite()
        {
            //1.Открыть главную страницу сайта

            PrestashopMainPage prestaMain = new PrestashopMainPage(this.driver);
            prestaMain.Navigate();

            /*2. Выполнить проверку, что цена продуктов в секции "Популярные товары" указана в 
            соответствии с установленной валютой в шапке сайта (USD, EUR, UAH).*/

            string currency_top = prestaMain.SearchElement("//*[@id='_desktop_currency_selector']/div/span[2]").Text;

            IList<IWebElement> currency_price = prestaMain.SearchElementsByClass("price");

            for (int i = 0;i<currency_price.Count;i++)
            {
                if (currency_top.Remove(0, currency_top.Length - 2) == currency_price[i].Text.Remove(0, currency_price[i].Text.Length - 2))
                    Console.WriteLine("2. Валюта в шапке совпадает с ценой товара");
                else
                    Console.WriteLine("2. Валюта в шапке НЕ совпадает с ценой товара");
            }

            //3.Установить показ цены в USD используя выпадающий список в шапке сайта.

            prestaMain.ClickElement("//*[@id='_desktop_currency_selector']/div/a/i");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='_desktop_currency_selector']/div/ul/li[3]/a")));
            prestaMain.ClickElement("//*[@id='_desktop_currency_selector']/div/ul/li[3]/a");
            Console.WriteLine("3.Установлено показ цены в USD");

            //4.Выполнить поиск в каталоге по слову “dress”.

            prestaMain.ClickElement("//*[@id='search_widget']/form/input[2]");

            prestaMain.SearchElement("//*[@id='search_widget']/form/input[2]").SendKeys("dress");

            prestaMain.ClickElement("//*[@id='search_widget']/form/button/i");

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);

            Console.WriteLine("4.Выполнен поиск в каталоге по слову \"dress\"");

            /*5.Выполнить проверку, что страница "Результаты поиска" содержит надпись 
             * "Товаров: x", где x -количество действительно найденных товаров.*/

            int counted = Convert.ToInt32(Regex.Replace(prestaMain.SearchElement("//*[@id='js-product-list-top']/div[1]/p").Text, @"[^\d]+", ""));

            int counted_find = prestaMain.SearchElementsByClass("thumbnail-container").Count;

            if(counted == counted_find)
                Console.WriteLine("5.Количество найденных товаров соответствует числу в надписи \"Товаров: x\"");
            else
                Console.WriteLine("5.Количество найденных товаров НЕ соответствует числу в надписи \"Товаров: x\"");

            //6.Проверить, что цена всех показанных результатов отображается в долларах.

            //7.Установить сортировку "от высокой к низкой."

            /*8.Проверить, что товары отсортированы по цене, при этом некоторые товары могут 
             быть со скидкой, и при сортировке используется цена без скидки.*/

            //9.Для товаров со скидкой указана скидка в процентах вместе с ценой до и после скидки.

            /*10. Необходимо проверить, что цена до и после скидки совпадает с указанным 
             размером скидки.*/
        }
    }
}
