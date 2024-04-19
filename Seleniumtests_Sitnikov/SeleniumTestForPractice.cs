using System.Configuration;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V121.SystemInfo;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Size = System.Drawing.Size;

namespace Seleniumtests_Sitnikov;

public class SeleniumTestForPractice
{
    public ChromeDriver driver;
    public WebDriverWait wait;
    
    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(4);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
        MethodAuthorization();
    }

    [Test]
    // Проверить переход в раздел "Сообщества" через боковое меню
    public void SideMenuCommunityButton()
    {
        driver.Manage().Window.Size = new Size(1200, 600);
        
        // 1. Переходим в раздел "Сообщества"
        var SideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        SideMenu.Click();
        var CommunityButton = driver.FindElements(By.CssSelector("[data-tid='Community']"))[1];
        CommunityButton.Click();
        
        // 2. Проверяем что переход совершен
        
        driver.FindElement(By.CssSelector("[data-tid='Title']")).Text.Should().Be("Сообщества");
    }

    [Test]
    // Проверить создание нового сообщества
    
    public void CreateNewCommunity()
    
    {
        // 1. Переходим в меню сообществ
        SideMenuCommunityButton();
        
        // 2. Создаём новое сообщество
        var NewCommunityButton = driver.FindElement(By.CssSelector("[class='sc-juXuNZ sc-ecQkzk WTxfS vPeNx']"));
        NewCommunityButton.Click();
        var TypeCommunityName = driver.FindElement(By.CssSelector("[data-tid='Name']"));
        TypeCommunityName.SendKeys("Тест создания сообщества");
        var CreateCommunity = driver.FindElement(By.CssSelector("[data-tid='CreateButton']"));
        CreateCommunity.Click();
        var CloseSettingsButton = driver.FindElement(By.CssSelector("[class='sc-juXuNZ kVHSha']"));
        CloseSettingsButton.Click();
        
        // 3. Проверяем, что сообщество создано
        driver.FindElement(By.CssSelector("[data-tid='Title']")).Text.Should().Be("Тест создания сообщества");
    }

    [Test]
    // Проверить возможность удаления последнего участника (себя) из сообщества

    public void DeleteLastMember()
    {
        // 1. Переходим в настройки сообщества
        MethodCommunitySettings();
        
        // 2. Переходим на вкладку "Модераторы"
        var CommunityAdmins = driver.FindElement(By.CssSelector("[data-tid='Managers']"));
        CommunityAdmins.Click();
        
        // 3. Пробуем удалить последнего участника
        try
        {
            var DeleteMemberButton = driver.FindElement(By.CssSelector("[data-tid='Delete']"));
            DeleteMemberButton.Click();
            var AcceptDeleteMember = driver.FindElement(By.ClassName("react-ui-17axm2e"));
            AcceptDeleteMember.Click();
        }
        catch (Exception e)
        {
            Assert.Fail("You cannot delete last member, test passed");
        }
    }
    
    [Test]
    // Проверить удаление сообщества

    public void DeleteNewCommunity()
    {
        // 1. Переходим в настройки сообщества
        MethodCommunitySettings();
        
        // 2. Удаляем сообщество
        var DeleteCommunity = driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        DeleteCommunity.Click();
        var AcceptDelete = driver.FindElement(By.ClassName("react-ui-aivml8"));
        AcceptDelete.Click();
        
        // 3. Проверяем удаление сообщества
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities?activeTab=isAdministrator");
        driver.FindElement(By.CssSelector("[class='sc-bdnxRM sc-jcwpoC sc-carFqZ eYSGDY hqvkmw']")).Text.Should()
            .Be("Поэтому показали вам котика");
    }

    [Test]
    // Проверить невозможность сохранения дополнительного email на кириллице
    
    public void SavingAdditionalEmailInCyrillic()
    {
        // 1. Переходим в раздел "Редактирование профиля"
        var ProfilePopupMenuButton = driver.FindElement(By.CssSelector("[data-tid='PopupMenu__caption']"));
        ProfilePopupMenuButton.Click();
        var ProfileSettingsButton = driver.FindElement(By.CssSelector("[data-tid='ProfileEdit']"));
        ProfileSettingsButton.Click();

        // 2. Вводим дополнительный email на кириллице
        var AdditionalEmailInput = driver.FindElements(By.ClassName("react-ui-huu6sg"))[1];
        AdditionalEmailInput.SendKeys("тестовая@почта.рф");
        AdditionalEmailInput.SendKeys(Keys.PageUp);
        
        // 3. Пробуем сохранить настройки
        var SaveChanges = driver.FindElement(By.CssSelector("[class='sc-juXuNZ kVHSha']"));
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[class='sc-juXuNZ kVHSha']")));
        SaveChanges.Click();
        
        // 4. Проверяем наличие сообщения об ошибке
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sc-iqAclL gpMmYS']")));
        driver.FindElement(By.CssSelector("[class='sc-iqAclL gpMmYS']")).Text.Should()
            .Be("Некорректно заполненны поля: «Дополнительный»");
    }

    public void MethodCommunitySettings()
    // Переход на страницу настроек сообщества
    
    {
        // 1. Переходим в администрируемые сообщества
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities?activeTab=isAdministrator");
        
        // 2. Переходим в сообщество
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Link']")));
        var SelectCreatedCommunity = driver.FindElement(By.CssSelector("[data-tid='Link']"));
        SelectCreatedCommunity.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Members']")));
        
        // 3. Переходим в настройки сообщества
        var DropDownList = driver.FindElements(By.CssSelector("[data-tid='DropdownButton']"))[1];
        DropDownList.Click();
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='Settings']")));
        var EnterSettings = driver.FindElement(By.CssSelector("[data-tid='Settings']"));
        EnterSettings.Click();
    }

    public void MethodAuthorization()
    // Авторизация в сервисе
    
    {
        // 1. Переходим на страницу входа
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        // 2. Вводим данные для входа
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("viktor0sitnikov@gmail.com");
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("1934108vviktorvV.");
        
        // 3. Нажимаем "Войти"
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        // 4. Проверяем, что мы находимся на стартовой странице "Новости"
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    }

    [TearDown]
    public void TearDown()
    {
        driver.Close();
        driver.Quit();
    }
}