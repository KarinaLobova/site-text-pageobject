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
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
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
            Console.WriteLine("3. Установлено показ цены в USD");

            //4.Выполнить поиск в каталоге по слову “dress”.

            prestaMain.ClickElement("//*[@id='search_widget']/form/input[2]");

            prestaMain.SearchElement("//*[@id='search_widget']/form/input[2]").SendKeys("dress");

            prestaMain.ClickElement("//*[@id='search_widget']/form/button/i");

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);

            Console.WriteLine("4. Выполнен поиск в каталоге по слову \"dress\"");

            /*5.Выполнить проверку, что страница "Результаты поиска" содержит надпись 
             * "Товаров: x", где x -количество действительно найденных товаров.*/

            int counted = Convert.ToInt32(Regex.Replace(prestaMain.SearchElement("//*[@id='js-product-list-top']/div[1]/p").Text, @"[^\d]+", ""));

            int counted_find = prestaMain.SearchElementsByClass("thumbnail-container").Count;

            if(counted == counted_find)
                Console.WriteLine("5. Количество найденных товаров соответствует числу в надписи \"Товаров: x\"");
            else
                Console.WriteLine("5. Количество найденных товаров НЕ соответствует числу в надписи \"Товаров: x\"");

            //6.Проверить, что цена всех показанных результатов отображается в долларах.
            
            IList<IWebElement> cur_price = prestaMain.SearchElementsByClass("price");

            for (int i = 0; i < cur_price.Count; i++)
            {
                if (cur_price[i].Text.Remove(0, cur_price[i].Text.Length - 1) == "$")
                    Console.WriteLine("6. Цена отображена в долларах");
                else
                {
                    Console.WriteLine("6. Цена отображена НЕ в долларах");
                }
            }

            //7.Установить сортировку "от высокой к низкой."

            prestaMain.ClickElement("//*[@id='js-product-list-top']/div[2]/div/div/a/i");
            prestaMain.ClickElement("//*[@id='js-product-list-top']/div[2]/div/div/div[2]/a[5]");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("7. Установлена сортировка \"от высокой к низкой\"");

            /*8.Проверить, что товары отсортированы по цене, при этом некоторые товары могут 
             быть со скидкой, и при сортировке используется цена без скидки.*/

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
            IList<IWebElement> current_price = prestaMain.SearchElementsByXpath("//*[@class='product-price-and-shipping']/span[1]");
            Console.WriteLine("8. Проверка сортировки \"от высокой к низкой\"");
            for (int i = 0, j = 1; i <= current_price.Count-2 && j <= current_price.Count-1; i++, j++)
            {
                double a = Convert.ToDouble(current_price[i].Text.Remove(current_price[i].Text.Length-2));
                double b = Convert.ToDouble(current_price[j].Text.Remove(current_price[j].Text.Length - 2));
                if (a < b)
                {
                    Console.WriteLine("Сортировка не верна!");
                }
                else
                    Console.WriteLine(a + "$ >= " + b + "$");
            }

            //9.Для товаров со скидкой указана скидка в процентах вместе с ценой до и после скидки.
            System.Threading.Thread.Sleep(500);

            Console.WriteLine("9. Проверка наличия цен для товаров со скидками");
            IList<IWebElement> arr = prestaMain.SearchElementsByClass("product-price-and-shipping");

            for(int i = 0; i < arr.Count; i++)
            {
                try
                {
                    if (arr[i].FindElement(By.XPath("span[2]")).Text != null)
                    {
                        if (arr[i].FindElement(By.XPath("span[3]")).Text != null)
                        {
                            int a = i + 1;
                            Console.WriteLine("Для товара " + a +" со скидкой указана скидка вместе с ценой до и после скидки");
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            /*10. Необходимо проверить, что цена до и после скидки совпадает с указанным 
             размером скидки.*/
            Console.WriteLine("10. Проверка соответствия указанного размера скидки");

            for (int i = 0; i < arr.Count; i++)
            {
                try
                {
                    if (arr[i].FindElement(By.XPath("span[2]")).Text != null)
                    {
                        string discount_s = arr[i].FindElement(By.XPath("span[2]")).Text;
                        discount_s = discount_s.Remove(discount_s.Length - 1);
                        double discount = Convert.ToDouble(discount_s) * (-1) / 100;
                        if (arr[i].FindElement(By.XPath("span[3]")).Text != null)
                        {
                            string after_discount_s = arr[i].FindElement(By.XPath("span[3]")).Text;
                            after_discount_s = after_discount_s.Remove(after_discount_s.Length - 2);
                            double after_discount = Convert.ToDouble(after_discount_s);

                            string before_discount_s = arr[i].FindElement(By.XPath("span[1]")).Text;
                            before_discount_s = before_discount_s.Remove(before_discount_s.Length - 2);
                            double before_discount = Convert.ToDouble(before_discount_s);

                            if(Math.Round(before_discount-(before_discount*discount),2) == after_discount)
                            {
                                Console.WriteLine("Цена до и после скидки совпадает с указанным размером скидки");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}
