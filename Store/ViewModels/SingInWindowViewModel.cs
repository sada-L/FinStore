using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Store.ViewModels;

public class SingInWindowViewModel : ViewModelBase
{
    private string _code = string.Empty;
    private string _captchaCode = string.Empty;
    private string _enteredCaptcha = string.Empty;
    private Bitmap? _captchaImage;
    private bool _showCaptcha ;
    private string _errorMessage = string.Empty;
    private int _attemptCount;
    
    public SingInWindowViewModel()
    {
        OpenWindowShow = new  Interaction<MainWindowViewModel, Unit>();
        
        LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);
    }
    public Interaction<MainWindowViewModel, Unit> OpenWindowShow { get; }
   
    public string Code
    {
        get => _code;
        set => this.RaiseAndSetIfChanged(ref _code, value);
    }

    public string CaptchaCode
    {
        get => _captchaCode;
        private set => this.RaiseAndSetIfChanged(ref _captchaCode, value);
    }

    public string EnteredCaptcha
    {
        get => _enteredCaptcha;
        set => this.RaiseAndSetIfChanged(ref _enteredCaptcha, value);
    }

    public Bitmap CaptchaImage
    {
        get => _captchaImage;
        private set => this.RaiseAndSetIfChanged(ref _captchaImage, value);
    }

    public bool ShowCaptcha
    {
        get => _showCaptcha;
        private set => this.RaiseAndSetIfChanged(ref _showCaptcha, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    
    private async Task LoginAsync()
    {
        if (string.IsNullOrEmpty(Code))
        {
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            mainWindowViewModel.IsAdmin = false;
            
            await OpenWindowShow.Handle(mainWindowViewModel);
        }
        
        if (_attemptCount >= 1)
        {
            ShowCaptcha = true;
            if (EnteredCaptcha != CaptchaCode)
            {
                ErrorMessage = "Invalid CAPTCHA.";
                return;
            }
        }
        
        if (Code == "1234")
        {
            ErrorMessage = "Authorization successful!";
            _attemptCount = 0;
            ShowCaptcha = false;
            
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            mainWindowViewModel.IsAdmin = true;
            
            await OpenWindowShow.Handle(mainWindowViewModel);
        }
        else
        {
            ErrorMessage = "Authorization failed!";
            _attemptCount++;
            if (_attemptCount >= 1)
            {
                GenerateCaptcha();
            }
            await Task.Delay(2000);
        }
    }

    private void GenerateCaptcha()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        CaptchaCode = new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        using (var bmp = new System.Drawing.Bitmap(100, 50))
        using (var gfx = Graphics.FromImage(bmp))
        using (var ms = new MemoryStream())
        {
            gfx.Clear(Color.White);
            var font = new Font("Arial", 20, FontStyle.Bold | FontStyle.Strikeout);
            var brush = new SolidBrush(Color.Black);
            var pen = new Pen(Color.Red, 2);

            for (int i = 0; i < CaptchaCode.Length; i++)
            {
                var charPosition = new PointF(20 * i + random.Next(-5, 5), random.Next(5, 15));
                gfx.DrawString(CaptchaCode[i].ToString(), font, brush, charPosition);
                gfx.DrawLine(pen, charPosition, new PointF(charPosition.X + 15, charPosition.Y + 15));
            }

            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            CaptchaImage = new Bitmap(ms);
        }
    }
}   