using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Seleniumtests_Sitnikov;

public class SeleniumTestForPractice
{
    [Test]
    public void Authorization()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
    // - Зайти в хром (с помощью вебдрайвера)
    var driver = new ChromeDriver(options);
    
    // - Перейти по урлу https://staff-testing.testkontur.ru
    driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
    Thread.Sleep(5000);
    
    // - ввести логин и пароль
    var login = driver.FindElement(By.Id("Username"));
    login.SendKeys("viktor0sitnikov@gmail.com");

    var password = driver.FindElement(By.Name("Password"));
    password.SendKeys("1934108vviktorvV.");
    
    Thread.Sleep(3000);
    
    // - нажать на кнопку "войти"
    var enter = driver.FindElement(By.Name("button"));
    enter.Click();
    
    Thread.Sleep(3000);
    
    // - проверяем что мы находимся на нужной странице
    var currenturl = driver.Url;
    Assert.That(currenturl == "https://staff-testing.testkontur.ru/news");
    
    // - закрываем браузер и убиваем процесс драйвера
    driver.Quit();
    }

}