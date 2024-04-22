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
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        Authorize();
    }

    [Test]
    // Проверить переход в раздел "Сообщества" через боковое меню
    public void sideMenuCommunityButton()
    {
        driver.Manage().Window.Size = new Size(1200, 600);
        
        // 1. Переходим в раздел "Сообщества"
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        var communityButton = driver.FindElements(By.CssSelector("[data-tid='Community']"))[1];
        communityButton.Click();
        
        // 2. Проверяем что переход совершен
        var communitySectionTitleText = driver.FindElement(By.CssSelector("[data-tid='Title']")).Text;
        communitySectionTitleText.Should().Be("Сообщества");
    }

    [Test]
    // Проверить создание нового сообщества
    
    public void createNewCommunity()
    
    {
        // 1. Переходим в меню сообществ
        sideMenuCommunityButton();
        
        // 2. Создаём новое сообщество
        var newCommunityButton = driver.FindElements(By.TagName("button"))[3];
        newCommunityButton.Click();
        
        // 3. Генерируем имя
        var typeCommunityName = driver.FindElement(By.CssSelector("[data-tid='Name']"));
        var communityName = Guid.NewGuid().ToString("N");
        typeCommunityName.SendKeys(communityName);
        
        // 4. Сохраняем сообщество
        var createCommunity = driver.FindElement(By.CssSelector("[data-tid='CreateButton']"));
        createCommunity.Click();
        var closeSettingsButton = driver.FindElements(By.TagName("button"))[4];
        closeSettingsButton.Click();
        
        // 5. Проверяем, что сообщество создано
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities?activeTab=isAdministrator");
        var communitiesCounter = driver.FindElement(By.CssSelector("[data-tid='CommunitiesCounter']")).Text;
        communitiesCounter.Should().Be("1 сообщество");
    }

    [Test]
    // Добавляем новый комментарий жирным шрифтом в ленту сообщества

    public void addNewComment()
    {
        // 1. Переходим в сообщество
        openMyCommunity();
        
        // 2. Создаём комментарий
        var writeComment = driver.FindElement(By.CssSelector("[data-tid='AddButton']"));
        writeComment.Click();
        var commentContent = driver.FindElement(By.CssSelector("[role='textbox']"));
        commentContent.SendKeys("Автотесты - это уважаемо");
        
        // 3. Выделяем и применяем жирное начертание к тексту
        commentContent.SendKeys(Keys.Control+"A");
        var boldFont = driver.FindElements(By.CssSelector("[class='RichEditor-styleButton']"))[4];
        boldFont.Click();
        
        // 4. Сохраняем комментарий
        var saveCommentButton = driver.FindElement(By.CssSelector("[data-tid='SendButton']"));
        saveCommentButton.Click();
        wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[data-tid='NewsText']")));
        
        // 5. Проверяем наличие комментария в ленте
        var CommentText = driver.FindElement(By.CssSelector("[data-tid='NewsText']")).Text;
        CommentText.Should().Be("Автотесты - это уважаемо");
    }
    
    [Test]
    // Проверить удаление сообщества

    public void deleteNewCommunity()
    {
        // 1. Переходим в настройки сообщества
        openMyCommunity();
        var dropDownList = driver.FindElements(By.CssSelector("[data-tid='DropdownButton']"))[1];
        dropDownList.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Settings']")));
        var enterSettings = driver.FindElement(By.CssSelector("[data-tid='Settings']"));
        enterSettings.Click();
        
        // 2. Удаляем сообщество
        var deleteCommunity = driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        deleteCommunity.Click();
        var acceptDelete = driver.FindElement(By.ClassName("react-ui-aivml8"));
        acceptDelete.Click();
        
        // 3. Проверяем удаление сообщества
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities?activeTab=isAdministrator");
        var emptyCommunityPageHolder = driver.FindElement(By.CssSelector("[data-tid='Feed']")).Text;
        emptyCommunityPageHolder.Should().Be("Подходящих сообществ нет\r\nПоэтому показали вам котика");
    }

    [Test]
    // Проверить невозможность сохранения дополнительного email на кириллице
    
    public void savingAdditionalEmailInCyrillic()
    {
        // 1. Переходим в раздел "Редактирование профиля"
        var profilePopupMenuButton = driver.FindElement(By.CssSelector("[data-tid='PopupMenu__caption']"));
        profilePopupMenuButton.Click();
        var profileSettingsButton = driver.FindElement(By.CssSelector("[data-tid='ProfileEdit']"));
        profileSettingsButton.Click();

        // 2. Вводим дополнительный email на кириллице
        var additionalEmailInput = driver.FindElements(By.ClassName("react-ui-huu6sg"))[1];
        additionalEmailInput.SendKeys("тестовая@почта.рф");
        additionalEmailInput.SendKeys(Keys.PageUp);
        
        // 3. Пробуем сохранить настройки
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[class='sc-juXuNZ kVHSha']")));
        var saveChanges = driver.FindElement(By.CssSelector("[class='sc-juXuNZ kVHSha']"));
        saveChanges.Click();
        
        // 4. Проверяем наличие сообщения об ошибке
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[class='sc-iqAclL gpMmYS']")));
        var validationFailedMessage = driver.FindElement(By.CssSelector("[class='sc-iqAclL gpMmYS']")).Text;
        validationFailedMessage.Should().Be("Некорректно заполненны поля: «Дополнительный»");
    }

    public void openMyCommunity()
    // Переход на страницу настроек сообщества
    
    {
        // 1. Переходим в администрируемые сообщества
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities?activeTab=isAdministrator");
        
        // 2. Переходим в сообщество
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Link']")));
        var selectCreatedCommunity = driver.FindElement(By.CssSelector("[data-tid='Link']"));
        selectCreatedCommunity.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Members']")));
    }

    public void Authorize()
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