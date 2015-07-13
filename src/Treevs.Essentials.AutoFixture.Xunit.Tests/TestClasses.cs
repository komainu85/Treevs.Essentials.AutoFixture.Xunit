namespace Treevs.Essentials.AutoFixture.Xunit.Tests
{
    public abstract class BaseSut
    {
        protected BaseSut()
        {
            Setup = false;
        }

        public bool Setup { get; set; }
    }

    public class AutoSetupGlobalSut : BaseSut { }

    public class AutoSetupMethodSut : BaseSut { }

    public class AutoSetupPropertySut : BaseSut { }

    public class AutoSetupPlainMethodSut : BaseSut { }
}
